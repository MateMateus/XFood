using Microsoft.AspNetCore.Mvc;

using xFood.Application.DTOs;
using xFood.Application.Interfaces;

namespace xFood.Web.Controllers
{
    /// <summary>
    /// CRUD de categorias na UI MVC, com validação de vínculo de produtos.
    /// </summary>
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _repo;
        public CategoriesController(ICategoryRepository repo) => _repo = repo;

        /// <summary>
        /// Lista categorias ordenadas para exibição.
        /// </summary>
        public async Task<IActionResult> Index()
            => View(await _repo.GetAllAsync());

        /// <summary>
        /// Exibe formulário de criação de categoria.
        /// </summary>
        public IActionResult Create() => View(new CategoryDto(0, "", null));

        /// <summary>
        /// Cria nova categoria após validar o modelo.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe formulário de edição para categoria existente.
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _repo.GetByIdAsync(id);
            if (dto is null) return NotFound();
            return View(dto);
        }

        /// <summary>
        /// Persiste alterações da categoria após validação.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Confirmação de exclusão; alerta se houver produtos vinculados.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _repo.GetByIdAsync(id);
            if (dto is null) return NotFound();

            var hasProducts = await _repo.AnyProductsAsync(id);
            ViewBag.HasProducts = hasProducts;
            return View(dto);
        }

        /// <summary>
        /// Executa exclusão quando não há produtos associados à categoria.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (await _repo.AnyProductsAsync(id))
            {
                TempData["Error"] = "Não é possível excluir: existem produtos vinculados.";
                return RedirectToAction(nameof(Index));
            }
            await _repo.DeleteAsync(id);
            TempData["Success"] = "Categoria excluída com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
