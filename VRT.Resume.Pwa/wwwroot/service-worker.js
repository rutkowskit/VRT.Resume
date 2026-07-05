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
const shellUrls = ['./index.html', './js/pwa-boot.js', './js/pwa-db-backup.js', './manifest.webmanifest'];

const base = "/";
const baseUrl = new URL(base, self.location.origin);

function isAppNavigation(url) {
    if (url.origin !== baseUrl.origin) {
        return false;
    }

    const path = url.pathname;
    return !path.startsWith('/_framework/')
        && !path.startsWith('/_content/');
}

async function onInstall(event) {
    console.info('Service worker: Install');

    const cache = await caches.open(cacheName);

    await cacheShellAssets(cache);

    const assets = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)));

    const results = await Promise.allSettled(assets.map(asset => cacheAsset(cache, asset)));
    const failed = results.filter(r => r.status === 'rejected').length;
    if (failed > 0) {
        console.warn(`Service worker: ${failed} asset(s) failed to cache during install`);
    }

    self.skipWaiting();
}

async function cacheAsset(cache, asset) {
    const request = new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' });

    try {
        await cache.add(request);
        return;
    } catch (error) {
        console.warn(`Service worker: integrity precache failed for ${asset.url}`, error);
    }

    const response = await fetch(asset.url, { cache: 'no-cache' });
    if (!response.ok) {
        throw new Error(`Failed to fetch ${asset.url}: ${response.status}`);
    }

    await cache.put(asset.url, response);
    await cache.put(new URL(asset.url, baseUrl).href, response.clone());
}

async function cacheShellAssets(cache) {
    for (const url of shellUrls) {
        try {
            const response = await fetch(url);
            if (!response.ok) {
                continue;
            }

            await cache.put(url, response);
            const absolute = new URL(url, baseUrl).href;
            await cache.put(absolute, response.clone());

            if (url === './index.html') {
                await cache.put(baseUrl.href, response.clone());
            }
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
    const requestUrl = new URL(event.request.url);
    const isAppRoute = isAppNavigation(requestUrl);
    const cachedIndex = isAppRoute ? await matchCache(cache, 'index.html') : undefined;

    // Cache-first for SPA routes: offline refresh must not hit the network.
    if (isAppRoute && cachedIndex) {
        if (navigator.onLine) {
            revalidateNavigation(cache, event.request).catch(() => { });
        }

        return cachedIndex;
    }

    try {
        const response = await fetch(event.request);
        if (response.ok) {
            return response;
        }
    } catch (error) {
        console.warn('Service worker: navigation fetch failed', event.request.url, error);
    }

    const cached = await matchCache(cache, event.request);
    if (cached) {
        return cached;
    }

    console.warn('Service worker: navigation not available offline', event.request.url);
    return Response.error();
}

async function revalidateNavigation(cache, request) {
    const response = await fetch(request);
    if (!response.ok) {
        return;
    }

    await cache.put(request, response.clone());
    await cache.put(baseUrl.href, response.clone());
    await cache.put('./index.html', response.clone());
    await cache.put(new URL('./index.html', baseUrl).href, response.clone());
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