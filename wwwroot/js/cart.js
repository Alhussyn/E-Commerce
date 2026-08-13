(() => {
    const page = document.getElementById('cart-page');
    if (!page) return;

    const feedback = document.getElementById('cart-feedback');

    const showFeedback = (message, isError = false) => {
        if (!feedback) return;
        feedback.textContent = message;
        feedback.className = `cart-feedback show ${isError ? 'error' : ''}`;
        clearTimeout(feedback._hideTimer);
        feedback._hideTimer = setTimeout(() => feedback.classList.remove('show'), 3500);
    };

    document.querySelectorAll('.js-cart-form').forEach(form => {
        form.addEventListener('submit', async event => {
            event.preventDefault();
            if (form.dataset.loading === 'true') return;
            form.dataset.loading = 'true';

            const controls = [...form.querySelectorAll('button')];
            const submittedQuantity = event.submitter?.value;
            controls.forEach(button => {
                button.disabled = true;
                button.classList.add('is-loading');
            });

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

                const payload = await response.json();
                if (!payload.success) throw new Error(payload.message || 'We could not update your cart.');

                const rowId = form.dataset.row;

                if (rowId && (form.action.includes('RemoveCartItem') || Number(submittedQuantity) <= 0)) {
                    const row = document.getElementById(rowId);
                    if (row) {
                        row.style.transition = 'all 300ms ease';
                        row.style.opacity = '0';
                        row.style.transform = 'translateX(-20px)';
                        setTimeout(() => row.remove(), 300);
                    }
                } else if (submittedQuantity) {
                    const row = document.getElementById(rowId);
                    row?.querySelector('[data-quantity]')?.replaceChildren(document.createTextNode(submittedQuantity));
                    const buttons = row?.querySelectorAll('button[name="quantity"]') || [];
                    buttons.forEach(button => {
                        button.value = Number(submittedQuantity) + (button.textContent.trim() === '+' ? 1 : -1);
                    });
                }

                // Update total with animation
                const totalEl = document.querySelector('.total-value');
                if (totalEl) {
                    totalEl.style.transition = 'all 200ms ease';
                    totalEl.style.transform = 'scale(1.05)';
                    totalEl.replaceChildren(document.createTextNode(`${Number(payload.subtotal).toLocaleString()} EGP`));
                    setTimeout(() => totalEl.style.transform = 'scale(1)', 200);
                }

                // Update cart count indicators
                document.querySelectorAll('[data-cart-count]').forEach(el => {
                    el.textContent = payload.cartCount;
                    el.style.transform = 'scale(1.3)';
                    el.style.transition = 'transform 200ms ease';
                    setTimeout(() => el.style.transform = 'scale(1)', 200);
                });

                showFeedback(payload.message);

                if (payload.cartEmpty) {
                    setTimeout(() => window.location.reload(), 500);
                }
            } catch (error) {
                showFeedback(error.message, true);
            } finally {
                form.dataset.loading = 'false';
                controls.forEach(button => {
                    button.disabled = false;
                    button.classList.remove('is-loading');
                });
            }
        });
    });
})();
