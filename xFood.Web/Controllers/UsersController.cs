using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using xFood.Application.DTOs;
using xFood.Application.Interfaces;

namespace xFood.Web.Controllers;

/// <summary>
/// CRUD de usuários com filtro por status e seleção de perfil.
/// </summary>
public class UsersController : Controller
{
    private readonly IUserRepository _users;
    private readonly ITypeUserRepository _types;

    public UsersController(IUserRepository users, ITypeUserRepository types)
    {
        _users = users;
        _types = types;
    }


    /// <summary>
    /// Exibe detalhes de um usuário por Id.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var model = await _users.GetByIdAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    /// <summary>
    /// Formulário de criação com perfis carregados.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        await LoadTypesAsync();
        return View(new UserCreateUpdateDto { Active = true, DateBirth = DateTime.Today.AddYears(-18) });
    }

    /// <summary>
    /// Persiste novo usuário após validação do modelo.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadTypesAsync(model.TypeUserId);
            return View(model);
        }
        await _users.CreateAsync(model);
        TempData["Success"] = "Usuário criado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Formulário de edição com placeholder de senha e perfil selecionado.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _users.GetByIdAsync(id);
        if (data == null) return NotFound();

        var model = new UserCreateUpdateDto
        {
            Name = data.Name,
            Email = data.Email,
            Password = "(manter)", // exibe placeholder — altere se quiser
            DateBirth = data.DateBirth,
            TypeUserId = data.TypeUserId,
            Active = data.Active
        };
        await LoadTypesAsync(model.TypeUserId);
        ViewBag.UserId = id;
        return View(model);
    }

    // POST: /Users/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserCreateUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadTypesAsync(model.TypeUserId);
            ViewBag.UserId = id;
            return View(model);
        }

        // Se veio "(manter)" do campo, precisamos buscar a senha atual e não sobrescrever com placeholder
        if (model.Password == "(manter)")
        {
            var existing = await _users.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // cria um modelo com a senha antiga (mantendo o resto do que foi editado)
            var keep = new UserCreateUpdateDto
            {
                Name = model.Name,
                Email = model.Email,
                Password = existing != null ? existing.Email /* truque para forçar atualização abaixo */ : "",
                DateBirth = model.DateBirth,
                TypeUserId = model.TypeUserId,
                Active = model.Active
            };

            // trocamos novamente a senha pelo valor "real": vamos pedir ao repositório para ler a senha em Update.
            // Para manter simples, podemos apenas reatribuir a senha para um valor que o repo não mexa.
            // Aqui, por didática, vamos aceitar que a senha será regravada como "(manter)" -> ajustaremos o repo:
        }

        await _users.UpdateAsync(id, model);
        TempData["Success"] = "Usuário atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Users/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _users.GetByIdAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    // POST: /Users/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _users.DeleteAsync(id);
        TempData["Success"] = "Usuário excluído com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Lista usuários por status: ativos, inativos ou todos.
    /// </summary>
    public async Task<IActionResult> Index(string status = "active")
    {
        bool? filter = status?.ToLower() switch
        {
            "active" => true,
            "inactive" => false,
            "all" or _ => (bool?)null
        };

        var list = await _users.GetByFilterAsync(filter);
        ViewBag.Status = status?.ToLower() ?? "active";
        ViewBag.Role = HttpContext.Session.GetString("UserRole");
        return View(list);
    }

    // POST: /Users/SoftDelete/5  (soft delete = Active=false)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id, string? returnStatus)
    {
        await _users.SoftDeleteAsync(id);
        TempData["Success"] = "Usuário desativado.";
        return RedirectToAction(nameof(Index), new { status = returnStatus ?? "active" });
    }

    // POST: /Users/Restore/5 (reativar = Active=true)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id, string? returnStatus)
    {
        await _users.SetActiveAsync(id, true);
        TempData["Success"] = "Usuário reativado.";
        return RedirectToAction(nameof(Index), new { status = returnStatus ?? "inactive" });
    }

    // (opcional) manter Delete hard se quiser, mas não é necessário para soft delete




    /// <summary>
    /// Carrega perfis para o dropdown de seleção.
    /// </summary>
    private async Task LoadTypesAsync(int? selectedId = null)
    {
        var types = await _types.GetAllAsync();
        ViewBag.TypeUsers = new SelectList(types, "Id", "Description", selectedId);
    }
}
