using System.Diagnostics; // Necessário para obter informações sobre a atividade atual (ex: RequestId)
using Microsoft.AspNetCore.Mvc; // Namespace base do ASP.NET MVC
using WebApplication1.Models; // Onde está definido o modelo ErrorViewModel usado no método Error

namespace WebApplication1.Controllers;

// Controlador responsável pelas páginas principais da aplicação (ex: Home, Privacidade, Erros)
public class HomeController : Controller
{
    // Logger para registar eventos e mensagens (pode ser útil para debugging ou análise de erros)
    private readonly ILogger<HomeController> _logger;

    // Construtor que recebe uma instância de ILogger via injeção de dependência
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Ação que devolve a página principal (Home Page)
    public IActionResult Index()
    {
        return View();
    }

    // Ação que devolve a página de privacidade (usada por convenção, pode ser adaptada)
    public IActionResult Privacy()
    {
        return View();
    }

    // Ação que devolve a página de erro em caso de falha na aplicação
    // A cache é desativada para garantir que os erros são sempre mostrados de forma atualizada
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Cria um modelo de erro com o ID da requisição atual
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}