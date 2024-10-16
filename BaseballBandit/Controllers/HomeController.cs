using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            string sql = "Select * from Inventory";
            var inventory = _context.Inventories.FromSqlRaw(sql).ToList();


            return View(inventory);
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
