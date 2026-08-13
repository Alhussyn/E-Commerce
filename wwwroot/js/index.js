document.addEventListener("DOMContentLoaded", () => {

    const form = document.querySelector('form');
    const searchInput = document.querySelector('input[name="searchTerm"]');
    const sortSelect = document.querySelector('select[name="sortOrder"]');
    let timeout;

    // 🔍 Search (Debounce)
    if (searchInput) {
        searchInput.addEventListener("input", () => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                form.submit();
            }, 500);
        });
    }

    // 🔃 Sort
    if (sortSelect) {
        sortSelect.addEventListener("change", () => {
            form.submit();
        });
    }

    // ✅ Toast from TempData (after redirect)
    const toast = document.getElementById('toast');
    if (toast) {
        // Double rAF ensures browser renders the element before adding 'show'
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                toast.classList.add('show');
            });
        });

        // Hide after 5 seconds
        setTimeout(() => {
            toast.classList.remove('show');
        }, 5000);
    }

});