using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Globalization;

using xFood.Application.DTOs;
using xFood.Application.Interfaces;

namespace xFood.Web.Controllers
{
    /// <summary>
    /// Gerencia produtos na UI MVC, com restrições por perfil (Admin/Manager).
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IWebHostEnvironment _env;

        public ProductsController(IProductRepository products, ICategoryRepository categories, IWebHostEnvironment env)
            => (_products, _categories, _env) = (products, categories, env);

        //Isso já bloqueia toda a área /Products se não estiver logado.

        /// <summary>Obtém o papel do usuário a partir da sessão.</summary>
        private string? GetRole() => HttpContext.Session.GetString("UserRole");
        /// <summary>Verifica se o papel atual é Admin.</summary>
        private bool IsAdmin() => string.Equals(GetRole(), "Admin", StringComparison.OrdinalIgnoreCase);
        /// <summary>Verifica se o papel atual é Manager.</summary>
        private bool IsManager() => string.Equals(GetRole(), "Manager", StringComparison.OrdinalIgnoreCase);
        /// <summary>Verifica se o papel atual é User.</summary>
        private bool IsUser() => string.Equals(GetRole(), "User", StringComparison.OrdinalIgnoreCase);
        /// <summary>Indica se existe usuário logado (sessão com papel).</summary>
        private bool IsLogged() => !string.IsNullOrEmpty(GetRole());

        /// <summary>
        /// Bloqueia acesso às ações se não estiver logado, redirecionando para Login.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsLogged())
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
            base.OnActionExecuting(context);
        }

        /// <summary>Retorna <c>403 Forbid</c> para acessos negados por perfil.</summary>
        private IActionResult Denied() => Forbid(); // simples




        /// <summary>
        /// Lista produtos com filtro por categoria/termo e popula dropdown de categorias.
        /// </summary>
        public async Task<IActionResult> Index(int? categoryId, string? q, int page = 1, int size = 50)
        {
            var cats = await _categories.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name", categoryId);

            var (total, items) = await _products.GetAllAsync(categoryId, q, page, size);
            return View(items);
        }

        /// <summary>
        /// Exibe detalhes de um produto por Id.
        /// </summary>
        public async Task<IActionResult> Details(int id, string? @return = null)
        {
            var dto = await _products.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        /// <summary>
        /// Exibe formulário de criação de produto (somente Admin/Manager).
        /// </summary>
        public async Task<IActionResult> Create()
        {

            if (!(IsAdmin() || IsManager())) return Denied();

            var cats = await _categories.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name");
            return View(new ProductCreateUpdateDto { Stock = 0, Price = 0m });
        }

        /// <summary>
        /// Cria produto após normalizar preço, validar modelo e fazer upload opcional.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateUpdateDto model, IFormFile? imageFile, string? @return)
        {
            if (!(IsAdmin() || IsManager())) return Denied();

            // --- normalização robusta para vírgula/ponto ---
            if (Request.Form.TryGetValue("Price", out var rawPrice))
            {
                var s = rawPrice.ToString().Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    var hasComma = s.Contains(',');
                    var hasDot = s.Contains('.');

                    string normalized;
                    if (hasComma && hasDot)
                    {
                        // "1.234,56" -> "1234.56"
                        normalized = s.Replace(".", "").Replace(",", ".");
                    }
                    else if (hasComma)
                    {
                        // "1234,56" -> "1234.56"
                        normalized = s.Replace(",", ".");
                    }
                    else
                    {
                        // "12.5" -> "12.5"
                        normalized = s;
                    }

                    if (decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                                         CultureInfo.InvariantCulture, out var parsed))
                    {
                        model.Price = parsed;

                        // limpa erro prévio de validação, se existir
                        if (ModelState.ContainsKey(nameof(model.Price)))
                            ModelState[nameof(model.Price)]!.Errors.Clear();
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                var cats = await _categories.GetAllAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name", model.CategoryId);
                return View(model);
            }

            // 🔹 Upload
            if (imageFile is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);
                using var fs = System.IO.File.Create(Path.Combine(folder, fileName));
                await imageFile.CopyToAsync(fs);
                model.ImageUrl = $"/uploads/{fileName}";
            }

            await _products.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Exibe formulário de edição (Admin/Manager) com categoria selecionada.
        /// </summary>
        public async Task<IActionResult> Edit(int id, string? @return = null)
        {
            if (!(IsAdmin() || IsManager())) return Denied();
            var dto = await _products.GetByIdAsync(id);
            if (dto is null) return NotFound();

            var cats = await _categories.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name", dto.CategoryId);

            var vm = new ProductCreateUpdateDto
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
            ViewBag.ProductId = dto.Id;
            return View(vm);
        }

        /// <summary>
        /// Atualiza produto com normalização de preço e upload opcional.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductCreateUpdateDto model, IFormFile? imageFile, string? @return)
        {

            if (!(IsAdmin() || IsManager())) return Denied();
            // --- normalização robusta para vírgula/ponto ---
            if (Request.Form.TryGetValue("Price", out var rawPrice))
            {
                var s = rawPrice.ToString().Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    var hasComma = s.Contains(',');
                    var hasDot = s.Contains('.');

                    string normalized;
                    if (hasComma && hasDot)
                    {
                        // "1.234,56" -> "1234.56"
                        normalized = s.Replace(".", "").Replace(",", ".");
                    }
                    else if (hasComma)
                    {
                        // "1234,56" -> "1234.56"
                        normalized = s.Replace(",", ".");
                    }
                    else
                    {
                        // "12.5" -> "12.5"
                        normalized = s;
                    }

                    if (decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                                         CultureInfo.InvariantCulture, out var parsed))
                    {
                        model.Price = parsed;

                        // limpa erro prévio de validação, se existir
                        if (ModelState.ContainsKey(nameof(model.Price)))
                            ModelState[nameof(model.Price)]!.Errors.Clear();
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                var cats = await _categories.GetAllAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name", model.CategoryId);
                return View(model);
            }

            // 🔹 Upload (mantém imagem antiga se não enviar nova)
            if (imageFile is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);
                using var fs = System.IO.File.Create(Path.Combine(folder, fileName));
                await imageFile.CopyToAsync(fs);
                model.ImageUrl = $"/uploads/{fileName}";
            }
            else
            {
                var existing = await _products.GetByIdAsync(id);
                model.ImageUrl = existing?.ImageUrl;
            }

            await _products.UpdateAsync(id, model);
            TempData["Success"] = "Produto atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Exibe confirmação de exclusão (apenas Admin).
        /// </summary>
        public async Task<IActionResult> Delete(int id, string? @return = null)
        {
            if (!IsAdmin()) return Denied();
            var dto = await _products.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        /// <summary>
        /// Executa exclusão do produto (apenas Admin).
        /// </summary>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id, string? @return = null)
        {
            if (!IsAdmin()) return Denied();
            await _products.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }




    }
}
