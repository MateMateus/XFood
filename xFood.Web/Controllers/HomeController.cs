using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using xFood.Web.Models;

namespace xFood.Web.Controllers;

/// <summary>
/// Páginas básicas (Home, Privacy) e tela de erro.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>Landing page da aplicação.</summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>Página de política de privacidade.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    /// <summary>Exibe informações do erro atual com RequestId.</summary>
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
