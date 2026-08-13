/**
 * Image Fallback System
 * Tries product gallery images first, then falls back to local placeholder.
 */
(function () {
    'use strict';

    var FALLBACK = '/images/catalog/fallback.svg';

    function handleError(img) {
        if (img.dataset.fallbackApplied) return;
        img.dataset.fallbackApplied = '1';

        // Try to find another image from the same product's gallery
        var gallery = img.closest('.gallery, .related-grid, .card, .card-image');
        if (gallery) {
            var siblings = gallery.querySelectorAll('img:not([data-fallback-applied])');
            for (var i = 0; i < siblings.length; i++) {
                if (siblings[i] !== img && siblings[i].naturalWidth > 0 && siblings[i].src !== FALLBACK) {
                    img.src = siblings[i].src;
                    img.onerror = function () { applyFallback(img); };
                    return;
                }
            }
        }

        applyFallback(img);
    }

    function applyFallback(img) {
        img.onerror = null;
        img.src = FALLBACK;
        img.alt = 'Image unavailable';
        img.style.objectFit = 'contain';
        img.style.padding = '24px';
        img.style.background = '#f0f4ff';
    }

    // Attach to all product images
    document.addEventListener('DOMContentLoaded', function () {
        var images = document.querySelectorAll('img[src]');
        for (var i = 0; i < images.length; i++) {
            var img = images[i];
            // Skip fallback itself
            if (img.src.indexOf('fallback.svg') !== -1) continue;
            // Skip data: URIs
            if (img.src.indexOf('data:') === 0) continue;

            img.addEventListener('error', function () {
                handleError(this);
            });

            // Check if already broken (cached 404)
            if (img.complete && img.naturalWidth === 0) {
                handleError(img);
            }
        }
    });
})();
