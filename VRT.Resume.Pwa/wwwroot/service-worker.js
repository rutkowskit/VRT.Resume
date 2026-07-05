// Offline-capable service worker for Blazor WASM (dev + published).
// See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') {
        return;
    }

    event.respondWith(onFetch(event));
});

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff2?$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];
const shellUrls = ['./index.html', './js/pwa-boot.js', './manifest.webmanifest'];

const base = "/";
const baseUrl = new URL(base, self.location.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    const cache = await caches.open(cacheName);

    await cacheShellAssets(cache);

    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));

    const results = await Promise.allSettled(assetsRequests.map(request => cache.add(request)));
    const failed = results.filter(r => r.status === 'rejected').length;
    if (failed > 0) {
        console.warn(`Service worker: ${failed} asset(s) failed to cache during install`);
    }

    self.skipWaiting();
}

async function cacheShellAssets(cache) {
    for (const url of shellUrls) {
        try {
            const response = await fetch(url);
            if (!response.ok) {
                continue;
            }

            await cache.put(url, response);
            await cache.put(new URL(url, baseUrl).href, response.clone());
        } catch (error) {
            console.warn(`Service worker: shell precache failed for ${url}`, error);
        }
    }
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));

    await self.clients.claim();
}

async function onFetch(event) {
    if (event.request.mode === 'navigate') {
        return handleNavigate(event);
    }

    return handleAsset(event);
}

async function handleNavigate(event) {
    const cache = await caches.open(cacheName);
    const isSpaRoute = !manifestUrlList.some(url => url === event.request.url);
    const cachedIndex = isSpaRoute ? await matchCache(cache, 'index.html') : undefined;

    try {
        return await fetch(event.request);
    } catch {
        if (cachedIndex) {
            return cachedIndex;
        }

        throw new Error('Offline and index.html is not cached.');
    }
}

async function handleAsset(event) {
    const cache = await caches.open(cacheName);
    const cached = await matchCache(cache, event.request);
    if (cached) {
        return cached;
    }

    try {
        return await fetch(event.request);
    } catch (error) {
        const fallback = await matchCache(cache, event.request);
        if (fallback) {
            return fallback;
        }

        console.warn('Service worker: asset not available offline', event.request.url, error);
        return Response.error();
    }
}

function unfingerprintFileName(fileName) {
    return fileName.replace(/^(.+)\.([a-z0-9]{6,})\.(js|css|wasm|json|dll|pdb|dat)$/i, '$1.$3');
}

function getUnfingerprintedUrl(url) {
    try {
        const parsed = new URL(url, baseUrl);
        const parts = parsed.pathname.split('/');
        const fileName = parts.pop();
        if (!fileName) {
            return null;
        }

        const unfingerprintedFile = unfingerprintFileName(fileName);
        if (unfingerprintedFile === fileName) {
            return null;
        }

        parts.push(unfingerprintedFile);
        return new URL(parts.join('/'), parsed.origin).href;
    } catch {
        return null;
    }
}

async function matchCache(cache, request) {
    const candidates = [];

    if (typeof request === 'string') {
        candidates.push(request, new URL(request, baseUrl).href);
    } else {
        candidates.push(request, request.url);
        try {
            candidates.push(new URL(request.url).pathname);
        } catch {
            // ignore invalid URLs
        }

        const unfingerprinted = getUnfingerprintedUrl(request.url);
        if (unfingerprinted) {
            candidates.push(unfingerprinted, new URL(unfingerprinted).pathname);
        }
    }

    for (const key of candidates) {
        const hit = await cache.match(key);
        if (hit) {
            return hit;
        }
    }

    return undefined;
}