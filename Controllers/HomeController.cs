using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIBQ.Models;
using System.Diagnostics;

namespace SIBQ.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /Home/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Retorna a view de login
            return View();
        }

        // GET: /Home/Privacy
        [HttpGet]
        public IActionResult Privacy()
        {
            // Retorna a view de política de privacidade
            return View();
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Error()
        {
            // Retorna a view de erro com o RequestId para rastreamento
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(errorModel);
        }
    }
}
