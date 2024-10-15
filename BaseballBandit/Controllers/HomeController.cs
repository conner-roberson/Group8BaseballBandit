using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaseballBandit.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly BaseballBanditContext _context;

        public HomeController(BaseballBanditContext context)
        {
            _context = context;
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
