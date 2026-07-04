// Dev (dotnet run): unregister service workers so published offline SW cannot interfere.
// Published/static hosting: register the offline service worker.
(function () {
    const devPorts = new Set(['5176', '7086']);
    const isDotnetDevServer =
        (location.hostname === 'localhost' || location.hostname === '127.0.0.1') &&
        devPorts.has(location.port);

    window.__pwaServiceWorkerReady = (async function () {
        if (!('serviceWorker' in navigator)) {
            return;
        }

        if (isDotnetDevServer) {
            const registrations = await navigator.serviceWorker.getRegistrations();
            await Promise.all(registrations.map((registration) => registration.unregister()));
            return;
        }

        await navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' });
    })();
})();