using Microsoft.AspNetCore.Mvc;

namespace xFood.Web.Controllers;

/// <summary>
/// Autenticação de demonstração usando sessão para perfis pré-definidos.
/// </summary>
public class AccountController : Controller
{
    /// <summary>Exibe o formulário de login.</summary>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    /// <summary>
    /// Realiza login com credenciais fixas (Admin/Manager/User) e grava sessão.
    /// </summary>
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        // ⚠️ Aqui é só demonstração — você pode conectar depois ao banco.
        // Admin
        if (email == "admin@xfood.com" && password == "123")
        {
            HttpContext.Session.SetString("UserName", "Administrador");
            HttpContext.Session.SetString("UserRole", "Admin");
            return RedirectToAction("Index", "Home");
        }

        // Gerente
        if (email == "gerente@xfood.com" && password == "123")
        {
            HttpContext.Session.SetString("UserName", "Gerente");
            HttpContext.Session.SetString("UserRole", "Manager");
            return RedirectToAction("Index", "Home");
        }

        // Usuário
        if (email == "user@xfood.com" && password == "123")
        {
            HttpContext.Session.SetString("UserName", "Usuário");
            HttpContext.Session.SetString("UserRole", "User");
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Credenciais inválidas.";
        return View();
    }

    /// <summary>
    /// Efetua logout limpando a sessão atual e redireciona para a Home.
    /// </summary>
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
