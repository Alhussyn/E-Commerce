const cards = () => [...document.querySelectorAll('.card[data-name]')];

// ── Stats ──
const total = cards().length;
const stock = cards().reduce((s, c) => s + parseInt(c.dataset.qty), 0);
const avg = total ? Math.round(cards().reduce((s, c) => s + parseFloat(c.dataset.price), 0) / total) : 0;

document.getElementById('totalCount').textContent = total;
document.getElementById('totalStock').textContent = stock.toLocaleString();
document.getElementById('avgPrice').textContent = avg.toLocaleString();

// ── Search ──
document.getElementById('searchInput').addEventListener('input', function () {
    const q = this.value.toLowerCase();
    cards().forEach(c => {
        c.style.display = c.dataset.name.includes(q) ? '' : 'none';
    });
});

// ── Sort ──
document.getElementById('sortSelect').addEventListener('change', function () {
    const grid = document.getElementById('productsGrid');
    const items = cards();

    items.sort((a, b) => {
        if (this.value === 'price-asc') return parseFloat(a.dataset.price) - parseFloat(b.dataset.price);
        if (this.value === 'price-desc') return parseFloat(b.dataset.price) - parseFloat(a.dataset.price);
        if (this.value === 'name') return a.dataset.name.localeCompare(b.dataset.name);
        if (this.value === 'stock') return parseInt(b.dataset.qty) - parseInt(a.dataset.qty);
        return 0;
    });

    items.forEach(c => grid.appendChild(c));
});

// ── Staggered animation ──
cards().forEach((c, i) => { c.style.animationDelay = (i * 60) + 'ms'; });