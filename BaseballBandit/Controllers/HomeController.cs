using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BaseballBandit.Classes;

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

        public IActionResult ProductPage(int ProductId)
        {
            string sql = $"Select * from Inventory WHERE ProductID = {ProductId}";
            var ProductDetails = _context.Inventories.FromSqlRaw(sql).ToList();

            return View(ProductDetails);
        }

        public IActionResult AddToCart(int ProductId)
        {
            bool success = CartClass.AddToCart(ProductId, _context);
            if (success)
            {
                TempData["successMessage"] = "Add To Cart Successful";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Add To Cart Failed";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Cart()
        {
            string sql = $"Select * From Inventory";
            var products = _context.Inventories.FromSqlRaw(sql).ToList();

            List<Inventory> cart = new List<Inventory>();
            
            int j = 0;
            for (int i = 0; i < products.Count && j < CartClass.productIds.Count; i++)
            {
                if (products[i].ProductId == CartClass.productIds[j])
                {
                    cart.Add(products[i]);
                    i = -1;
                    j++; 
                }
            }
            return View(cart);
        }

        public IActionResult RemoveFromCart(int ProductId)
        {
            bool success = CartClass.RemoveFromCart(ProductId, _context);
            if (success)
            {
                TempData["successMessage"] = "Remove From Cart Successful";
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Remove From Cart Failed";
                return RedirectToAction("Cart", "Home");
            }
        }
        public IActionResult RemoveAllFromCart(int ProductId)
        {
            bool success = CartClass.RemoveAllFromCart(ProductId, _context);
            if (success)
            {
                TempData["successMessage"] = "Remove From Cart Successful";
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Remove From Cart Failed";
                return RedirectToAction("Cart", "Home");
            }
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
