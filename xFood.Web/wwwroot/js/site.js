// ===== Tema (dark/light) =====
(function () {
    const KEY = "xfood.theme";
    const body = document.body;
    const btn = document.getElementById("themeToggle");

    function apply(theme) {
        body.setAttribute("data-theme", theme);
        if (btn) btn.textContent = theme === "dark" ? "🌙 Escuro" : "☀️ Claro";
    }
    function init() {
        const saved = localStorage.getItem(KEY) || "dark";
        apply(saved);
    }
    btn?.addEventListener("click", () => {
        const cur = body.getAttribute("data-theme") || "dark";
        const next = cur === "dark" ? "light" : "dark";
        localStorage.setItem(KEY, next);
        apply(next);
    });
    init();
})();

// ===== Scroll suave para âncoras =====
document.addEventListener("click", (e) => {
    const a = e.target.closest('a[href^="/#"]');
    if (!a) return;
    const id = a.getAttribute("href").slice(2);
    const el = document.getElementById(id);
    if (el) {
        e.preventDefault();
        el.scrollIntoView({ behavior: "smooth", block: "start" });
        history.replaceState(null, "", "/#" + id);
    }
});

// ===== Catálogo (Categorias + Produtos) =====
document.addEventListener("DOMContentLoaded", () => {
    const elCatGrid = document.getElementById("cat-grid");
    const elProdGrid = document.getElementById("prod-grid");
    const elProdAlert = document.getElementById("prod-alert");
    const elQ = document.getElementById("q");
    const btnAllCats = document.getElementById("btnAllCats");

    if (!elCatGrid || !elProdGrid) return;

    const state = { categoryId: null, q: "" };
    const money = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
    const esc = (s) => (s ?? "").toString().replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));

    // --- API helpers ---
    async function getJson(url) {
        const r = await fetch(url, { headers: { Accept: "application/json" } });
        if (!r.ok) throw new Error(url + " -> HTTP " + r.status);
        return await r.json();
    }

    // --- CATEGORIES ---
    async function loadCategories() {
        try {
            const cats = await getJson("/api/categories");
            if (!Array.isArray(cats)) throw new Error("Formato inválido de categorias");
            renderCategories(cats);
        } catch (err) {
            console.error("Categorias:", err);
            elCatGrid.innerHTML = `<div class="col-12"><div class="alert alert-danger">Falha ao carregar categorias.</div></div>`;
        }
    }
    function renderCategories(cats) {
        elCatGrid.innerHTML = cats.map(c => `
      <div class="col-6 col-sm-4 col-md-3 col-lg-2">
        <div class="card cat-card h-100" data-id="${c.id}">
          <div class="card-body">
            <div class="fw-semibold">${esc(c.name)}</div>
            <small class="text-muted">${esc(c.description ?? "")}</small>
          </div>
        </div>
      </div>
    `).join("");
        elCatGrid.querySelectorAll(".cat-card").forEach(card => {
            card.addEventListener("click", () => {
                elCatGrid.querySelectorAll(".cat-card").forEach(x => x.classList.remove("active"));
                card.classList.add("active");
                state.categoryId = Number(card.dataset.id);
                loadProducts();
                document.getElementById("products")?.scrollIntoView({ behavior: "smooth" });
            });
        });
        btnAllCats?.addEventListener("click", () => {
            state.categoryId = null;
            elCatGrid.querySelectorAll(".cat-card").forEach(x => x.classList.remove("active"));
            loadProducts();
            document.getElementById("products")?.scrollIntoView({ behavior: "smooth" });
        });
    }

    // --- PRODUCTS ---
    async function loadProducts() {
        try {
            elProdAlert.classList.add("d-none");
            elProdGrid.innerHTML = `<div class="col-12 text-center text-muted py-3">Carregando…</div>`;

            const url = new URL("/api/products", location.origin);
            if (state.categoryId) url.searchParams.set("categoryId", state.categoryId);
            if (state.q) url.searchParams.set("q", state.q);
            url.searchParams.set("page", 1);
            url.searchParams.set("size", 100);

            const data = await getJson(url.toString());
            const items = Array.isArray(data.items) ? data.items : Array.isArray(data) ? data : [];
            if (!items.length) {
                elProdGrid.innerHTML = `<div class="col-12"><div class="alert alert-warning">Nada encontrado.</div></div>`;
                return;
            }
            elProdGrid.innerHTML = items.map(p => productCard(p)).join("");
        } catch (err) {
            console.error("Produtos:", err);
            elProdAlert.classList.remove("d-none");
            elProdGrid.innerHTML = "";
        }
    }
    function productCard(p) {
        const role = (window._role || "").toLowerCase();
        const canEdit = role === "admin" || role === "manager";
        const canDelete = role === "admin";
        const ret = encodeURIComponent("/#products");

        const img = p.imageUrl
            ? `<img class="thumb w-100" src="${esc(p.imageUrl)}" alt="${esc(p.name)}">`
            : `<div class="thumb w-100 d-flex align-items-center justify-content-center text-muted">sem imagem</div>`;

        return `
    <div class="col-12 col-sm-6 col-md-4 col-lg-3">
      <div class="card prod-card h-100">
        <div class="p-2">${img}</div>
        <div class="card-body pt-0">
          <div class="d-flex justify-content-between align-items-start mb-1">
            <h3 class="h6 mb-0">${esc(p.name)}</h3>
            <span class="badge bg-secondary">${esc(p.categoryName ?? "")}</span>
          </div>
          <div class="small text-muted mb-2">${esc(p.description ?? "")}</div>
          <div class="d-flex justify-content-between align-items-center">
            <span class="price">${money.format(Number(p.price))}</span>
            <small class="text-muted">Estoque: ${Number(p.stock)}</small>
          </div>
        </div>
        <div class="card-footer bg-transparent border-0 pt-0 pb-3">
          <div class="d-flex gap-2">
            <a class="btn btn-sm btn-info" href="/Products/Details/${p.id}?return=${ret}">Details</a>
            ${canEdit ? `<a class="btn btn-sm btn-warning" href="/Products/Edit/${p.id}?return=${ret}">Editar</a>` : ""}
            ${canDelete ? `<a class="btn btn-sm btn-danger" href="/Products/Delete/${p.id}?return=${ret}">Excluir</a>` : ""}
          </div>
        </div>
      </div>
    </div>`;
    }


    // Busca
    let t;
    elQ?.addEventListener("input", () => {
        clearTimeout(t);
        t = setTimeout(() => { state.q = elQ.value.trim(); loadProducts(); }, 300);
    });

    // Init
    (async () => {
        await loadCategories();
        await loadProducts();
    })();
});
