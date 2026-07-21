// The published app replaces this file with service-worker.published.js.
// During development, unregister this worker so it cannot survive a debug session
// and interfere with the next Blazor WebAssembly debugger launch.
self.addEventListener('install', () => self.skipWaiting());

self.addEventListener('activate', event => {
    event.waitUntil(self.registration.unregister());
});
