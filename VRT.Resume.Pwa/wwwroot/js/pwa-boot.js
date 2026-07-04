// PWA bootstrap: service worker (dev vs published), single-tab OPFS lock, deferred Blazor load.
(function () {
    const cultureStorageKey = 'VRT.Resume.Culture';
    const tabLockName = 'vrt-resume-opfs';
    const tabChannelName = 'vrt-resume-opfs-tab';
    const devPorts = new Set(['5176', '7086']);

    const messages = {
        pl: {
            title: 'Aplikacja jest już otwarta',
            body: 'VRT Resume działa tylko w jednej karcie przeglądarki. Zamknij inną kartę z tą aplikacją, a ta strona odświeży się automatycznie.',
            retry: 'Odśwież teraz',
        },
        en: {
            title: 'App already open',
            body: 'VRT Resume can only run in one browser tab. Close the other tab with this app and this page will refresh automatically.',
            retry: 'Refresh now',
        },
    };

    function getCulture() {
        const stored = localStorage.getItem(cultureStorageKey);
        return stored && stored.toLowerCase().startsWith('en') ? 'en' : 'pl';
    }

    function getMessages() {
        return messages[getCulture()];
    }

    function showBlockedPanel() {
        const panel = document.getElementById('opfs-tab-blocked');
        const app = document.getElementById('app');
        if (!panel) {
            return;
        }

        const text = getMessages();
        const title = document.getElementById('opfs-tab-blocked-title');
        const body = document.getElementById('opfs-tab-blocked-body');
        const retry = document.getElementById('opfs-tab-blocked-retry');

        if (title) {
            title.textContent = text.title;
        }

        if (body) {
            body.textContent = text.body;
        }

        if (retry) {
            retry.textContent = text.retry;
        }

        panel.hidden = false;

        if (app) {
            app.style.display = 'none';
        }
    }

    function loadBlazorScripts() {
        const template = document.getElementById('pwa-blazor-scripts');
        if (!template) {
            return;
        }

        document.body.appendChild(template.content.cloneNode(true));
    }

    async function configureServiceWorker() {
        if (!('serviceWorker' in navigator)) {
            return;
        }

        const isDotnetDevServer =
            (location.hostname === 'localhost' || location.hostname === '127.0.0.1') &&
            devPorts.has(location.port);

        if (isDotnetDevServer) {
            const registrations = await navigator.serviceWorker.getRegistrations();
            await Promise.all(registrations.map((registration) => registration.unregister()));
            return;
        }

        await navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' });
    }

    function listenForLeaderRelease() {
        if (typeof BroadcastChannel === 'undefined') {
            return;
        }

        const channel = new BroadcastChannel(tabChannelName);
        channel.onmessage = (event) => {
            if (event.data === 'leader-closed') {
                location.reload();
            }
        };
    }

    function notifyLeaderClosed() {
        if (typeof BroadcastChannel === 'undefined') {
            return;
        }

        const channel = new BroadcastChannel(tabChannelName);
        channel.postMessage('leader-closed');
        channel.close();
    }

    function tryAcquireTabLock() {
        if (!navigator.locks?.request) {
            return Promise.resolve(true);
        }

        return new Promise((resolve) => {
            navigator.locks.request(
                tabLockName,
                { mode: 'exclusive', ifAvailable: true },
                (lock) => {
                    if (!lock) {
                        resolve(false);
                        return;
                    }

                    window.addEventListener('pagehide', notifyLeaderClosed, { once: true });
                    resolve(true);

                    // Keep the exclusive lock until this tab closes.
                    return new Promise(() => { });
                });
        });
    }

    async function boot() {
        listenForLeaderRelease();
        await configureServiceWorker();

        const acquired = await tryAcquireTabLock();
        if (!acquired) {
            showBlockedPanel();
            return;
        }

        loadBlazorScripts();
    }

    document.getElementById('opfs-tab-blocked-retry')?.addEventListener('click', () => location.reload());

    window.__pwaBootReady = boot();
})();