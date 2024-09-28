using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Controllers
{
    public class ShortUrlsController : Controller
    {
        private readonly ILogger<ShortUrlsController> _logger;

        public ShortUrlsController(ILogger<ShortUrlsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
