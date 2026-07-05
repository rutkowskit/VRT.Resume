// PWA bootstrap: service worker, single-tab OPFS lock, deferred Blazor load.
(function () {
    const cultureStorageKey = 'VRT.Resume.Culture';
    const tabLockName = 'vrt-resume-opfs';
    const tabChannelName = 'vrt-resume-opfs-tab';

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

        try {
            await navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' });
            await navigator.serviceWorker.ready;
        } catch (error) {
            console.warn('Service worker registration failed:', error);
        }
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

    function delay(ms) {
        return new Promise((resolve) => setTimeout(resolve, ms));
    }

    function isPageReload() {
        const navigation = performance.getEntriesByType('navigation')[0];
        return navigation?.type === 'reload';
    }

    function tryAcquireTabLockOnce() {
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

    async function tryAcquireTabLock() {
        if (!navigator.locks?.request) {
            return true;
        }

        if (isPageReload()) {
            await delay(400);
        }

        const maxAttempts = isPageReload() ? 20 : 6;
        for (let attempt = 0; attempt < maxAttempts; attempt++) {
            if (await tryAcquireTabLockOnce()) {
                return true;
            }

            await delay(250);
        }

        return false;
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