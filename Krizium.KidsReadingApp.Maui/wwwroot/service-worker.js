
// Cache names
const STATIC_CACHE_NAME = 'kids-reading-static-v1';
const DYNAMIC_CACHE_NAME = 'kids-reading-dynamic-v1';
const AUDIO_CACHE_NAME = 'kids-reading-audio-v1';

// Resources to cache immediately on installation
const STATIC_RESOURCES = [
    '/',
    '/index.html',
    '/css/app.css',
    '/js/app.js',
    '/js/readingHelpers.js',
    '/images/app-icon.png',
    '/offline.html',
    '/favicon.ico'
];

// Install event - cache static resources
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(STATIC_CACHE_NAME)
            .then(cache => {
                console.log('Caching static resources');
                return cache.addAll(STATIC_RESOURCES);
            })
    );
});

// Activate event - clean up old caches
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.filter(cacheName => {
                    return (
                        cacheName.startsWith('kids-reading-') &&
                        cacheName !== STATIC_CACHE_NAME &&
                        cacheName !== DYNAMIC_CACHE_NAME &&
                        cacheName !== AUDIO_CACHE_NAME
                    );
                }).map(cacheName => {
                    return caches.delete(cacheName);
                })
            );
        })
    );
});

// Fetch event - serve from cache if available, otherwise fetch from network
self.addEventListener('fetch', event => {
    const url = new URL(event.request.url);

    // Handle audio file requests differently
    if (url.pathname.startsWith('/audio/')) {
        event.respondWith(handleAudioRequest(event.request));
        return;
    }

    // Handle API requests
    if (url.pathname.startsWith('/api/')) {
        event.respondWith(handleApiRequest(event.request));
        return;
    }

    // Handle static resources
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }

                return fetch(event.request)
                    .then(fetchResponse => {
                        // Cache successfully fetched resources
                        if (fetchResponse && fetchResponse.status === 200) {
                            const clonedResponse = fetchResponse.clone();
                            caches.open(DYNAMIC_CACHE_NAME)
                                .then(cache => {
                                    cache.put(event.request, clonedResponse);
                                });
                        }
                        return fetchResponse;
                    })
                    .catch(() => {
                        // If the request is for a page, return the offline page
                        if (event.request.headers.get('accept').includes('text/html')) {
                            return caches.match('/offline.html');
                        }
                    });
            })
    );
});

// Handle audio file requests
function handleAudioRequest(request) {
    return caches.match(request)
        .then(response => {
            if (response) {
                return response;
            }

            return fetch(request)
                .then(fetchResponse => {
                    if (fetchResponse && fetchResponse.status === 200) {
                        const clonedResponse = fetchResponse.clone();
                        caches.open(AUDIO_CACHE_NAME)
                            .then(cache => {
                                cache.put(request, clonedResponse);
                            });
                    }
                    return fetchResponse;
                });
        });
}

// Handle API requests
function handleApiRequest(request) {
    // Try network first for API requests
    return fetch(request)
        .then(response => {
            // Clone the response to store in cache
            if (response && response.status === 200) {
                const clonedResponse = response.clone();

                // Only cache GET requests
                if (request.method === 'GET') {
                    caches.open(DYNAMIC_CACHE_NAME)
                        .then(cache => {
                            cache.put(request, clonedResponse);
                        });
                }
            }
            return response;
        })
        .catch(() => {
            // If network fails, try from cache
            return caches.match(request);
        });
}

// Listen for messages from the main thread
self.addEventListener('message', event => {
    if (event.data.action === 'cacheBook') {
        const bookId = event.data.bookId;
        // This would trigger caching of book-specific resources
        cacheBookResources(bookId);
    }
});

// Function to cache all resources for a specific book
function cacheBookResources(bookId) {
    const urlsToCache = [
        `/api/books/${bookId}`,
        `/books/${bookId}/details`
    ];

    caches.open(DYNAMIC_CACHE_NAME)
        .then(cache => {
            return cache.addAll(urlsToCache);
        })
        .catch(error => {
            console.error('Error caching book resources:', error);
        });
}