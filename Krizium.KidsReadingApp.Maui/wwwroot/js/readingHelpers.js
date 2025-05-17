// Speech synthesis helper functions
function isSpeechSynthesisAvailable() {
    return 'speechSynthesis' in window && 'SpeechSynthesisUtterance' in window;
}

// Cache for audio elements to avoid creating new ones for each word
const audioCache = {};

// Speak a word using browser's built-in speech synthesis
function speakWord(word) {
    if (!isSpeechSynthesisAvailable()) {
        console.error('Speech synthesis not available');
        return;
    }

    // Stop any current speech
    window.speechSynthesis.cancel();

    const utterance = new SpeechSynthesisUtterance(word);

    // Configure for a child-friendly voice
    // Try to use a female voice which tends to be clearer for children
    const voices = window.speechSynthesis.getVoices();
    const preferredVoice = voices.find(voice =>
        voice.name.includes('Female') ||
        voice.name.includes('Girl') ||
        voice.name.includes('Lisa') ||
        voice.name.includes('Samantha')
    );

    if (preferredVoice) {
        utterance.voice = preferredVoice;
    }

    // Slow down speech rate slightly for clarity
    utterance.rate = 0.9;
    utterance.pitch = 1.1;

    window.speechSynthesis.speak(utterance);

    return new Promise((resolve) => {
        utterance.onend = () => resolve();
    });
}

// Play audio from a URL (for server-side TTS)
function playAudio(audioUrl) {
    return new Promise((resolve, reject) => {
        // Reuse audio element if it exists for this URL
        if (!audioCache[audioUrl]) {
            audioCache[audioUrl] = new Audio(audioUrl);
        }

        const audio = audioCache[audioUrl];

        // Stop and reset any currently playing audio
        audio.pause();
        audio.currentTime = 0;

        // Set up event listeners
        const onEnded = () => {
            audio.removeEventListener('ended', onEnded);
            audio.removeEventListener('error', onError);
            resolve();
        };

        const onError = (err) => {
            audio.removeEventListener('ended', onEnded);
            audio.removeEventListener('error', onError);
            reject(err);
        };

        audio.addEventListener('ended', onEnded);
        audio.addEventListener('error', onError);

        // Start playing
        audio.play().catch(onError);
    });
}

// Highlight a word element
function highlightWord(elementId) {
    // Remove previous highlights
    document.querySelectorAll('.word.highlight').forEach(el => {
        el.classList.remove('highlight');
    });

    // Add highlight to current word
    const wordElement = document.getElementById(elementId);
    if (wordElement) {
        wordElement.classList.add('highlight');

        // Scroll into view if needed
        const rect = wordElement.getBoundingClientRect();
        const isInView = (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );

        if (!isInView) {
            wordElement.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
        }
    }
}

// Clear all highlights
function clearHighlights() {
    document.querySelectorAll('.word.highlight').forEach(el => {
        el.classList.remove('highlight');
    });
}

// Check for offline capability
function checkOfflineCapability() {
    const status = {
        speechSynthesisAvailable: isSpeechSynthesisAvailable(),
        serviceWorkerSupported: 'serviceWorker' in navigator,
        storageAvailable: 'localStorage' in window && 'indexedDB' in window
    };

    status.canWorkOffline = status.speechSynthesisAvailable && status.serviceWorkerSupported;
    return status;
}

// Cache management for offline use
function cacheBookForOffline(bookId) {
    return new Promise(async (resolve, reject) => {
        try {
            // Register service worker if not already registered
            if ('serviceWorker' in navigator) {
                const registration = await navigator.serviceWorker.register('/service-worker.js');
                console.log('Service Worker registered with scope:', registration.scope);
            }

            // Request book caching on the server
            const response = await fetch(`/api/books/${bookId}/cache-words`, {
                method: 'POST'
            });

            if (!response.ok) {
                throw new Error('Failed to cache book words');
            }

            const result = await response.json();
            resolve(result);
        } catch (error) {
            console.error('Error caching book for offline use:', error);
            reject(error);
        }
    });
}

// Initialize reading helpers
function initReadingHelpers() {
    // Pre-load voices if available
    if (isSpeechSynthesisAvailable()) {
        window.speechSynthesis.getVoices();
    }

    // Log offline capability
    const offlineStatus = checkOfflineCapability();
    console.log('Offline capability:', offlineStatus);

    return offlineStatus;
}

// Export functions for use in Blazor via JSInterop
window.readingHelpers = {
    isSpeechSynthesisAvailable,
    speakWord,
    playAudio,
    highlightWord,
    clearHighlights,
    checkOfflineCapability,
    cacheBookForOffline,
    initReadingHelpers
};