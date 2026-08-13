(() => {
    const announce = (message, error = false) => {
        let toast = document.getElementById('toast');
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'toast';
            toast.className = 'toast';
            toast.setAttribute('role', 'status');
            toast.setAttribute('aria-live', 'polite');
            document.body.appendChild(toast);
        }
        toast.textContent = message;
        toast.classList.toggle('error', error);
        requestAnimationFrame(() => toast.classList.add('show'));
        clearTimeout(toast._hideTimer);
        toast._hideTimer = setTimeout(() => toast.classList.remove('show'), 3500);
    };

    document.querySelectorAll('.js-add-cart').forEach(form => {
        form.addEventListener('submit', async event => {
            event.preventDefault();
            const button = form.querySelector('button[type="submit"]');
            if (button.disabled) return;
            button.disabled = true;
            button.classList.add('is-loading');

            try {
                const response = await fetch(form.action, {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Accept': 'application/json'
                    },
                    body: new FormData(form)
                });

                if (!response.ok) {
                    const errorData = await response.json().catch(() => ({}));
                    throw new Error(errorData.message || `Server error (${response.status})`);
                }

                const result = await response.json();
                if (!result.success) throw new Error(result.message || 'Unable to add this product.');

                // Update all cart count indicators
                document.querySelectorAll('[data-cart-count]').forEach(el => {
                    el.textContent = result.cartCount;
                    // Pulse animation on count change
                    el.style.transform = 'scale(1.3)';
                    el.style.transition = 'transform 200ms ease';
                    setTimeout(() => {
                        el.style.transform = 'scale(1)';
                    }, 200);
                });

                announce(result.message);
            } catch (error) {
                announce(error.message, true);
            } finally {
                button.disabled = false;
                button.classList.remove('is-loading');
            }
        });
    });
})();
