// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

globalThis.importScripts('./service-worker-assets.js');
globalThis.addEventListener('install', event => event.waitUntil(onInstall(event)));
globalThis.addEventListener('activate', event => event.waitUntil(onActivate(event)));
globalThis.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${globalThis.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, globalThis.origin);
const manifestUrlList = new Set(globalThis.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href));

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = globalThis.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !manifestUrlList.has(event.request.url);

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    // If we have a cached response, return it
    if (cachedResponse) {
        return cachedResponse;
    }

    // For assets not in cache, fetch from network with Cloudflare CDN support
    // Clone the request to avoid consuming it
    const fetchRequest = event.request.clone();
    
    try {
        const response = await fetch(fetchRequest);
        
        // Check if we received a valid response
        if (response?.status === 200 && response?.type === 'basic') {
            // Cache successful responses for future offline use
            const responseToCache = response.clone();
            const cache = await caches.open(cacheName);
            
            // Only cache assets that match our patterns
            if (offlineAssetsInclude.some(pattern => pattern.test(event.request.url)) &&
                !offlineAssetsExclude.some(pattern => pattern.test(event.request.url))) {
                cache.put(event.request, responseToCache);
            }
        }
        
        return response;
    } catch (error) {
        // Network request failed, try to serve from cache as fallback
        const cache = await caches.open(cacheName);
        const cachedFallback = await cache.match(event.request);
        
        if (cachedFallback) {
            return cachedFallback;
        }
        
        // If no cache available and it's a navigation request, serve index.html
        if (event.request.mode === 'navigate') {
            return cache.match('index.html');
        }
        
        throw error;
    }
}
