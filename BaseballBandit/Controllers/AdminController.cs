using BaseballBandit.Classes;
using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseballBandit.Controllers
{
    public class AdminController : Controller
    {
        private readonly BaseballBanditContext _context;

        public AdminController(BaseballBanditContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Buyers()
        {
            string sql = "Select * from UserData WHERE Admin=0 AND SELLER=0";
            var userAccounts = _context.Users.FromSqlRaw(sql).ToList();
            return View(userAccounts);
        }
        public IActionResult DeleteBuyer(int UserId)
        {
            bool success = Classes.User.DeleteUser(UserId, _context);
            if (success)
            {
                TempData["successMessage"] = "Account Deleted";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["errorMessage"] = "Account Deletion Unsuccessful";
                return RedirectToAction("Index", "Admin");
            }
        }
        public IActionResult Sellers()
        {
            string sql = "Select * from UserData WHERE SELLER=1";
            var sellerAccounts = _context.Users.FromSqlRaw(sql).ToList();
            return View(sellerAccounts);
        }
        public IActionResult DeleteSeller(int UserId)
        {
            bool UserSuccess = Classes.User.DeleteUser(UserId, _context);
            bool ProductSuccess = Classes.User.DeleteSeller(UserId, _context);
            if (UserSuccess && ProductSuccess)
            {
                TempData["successMessage"] = "Account Deleted";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["errorMessage"] = "Account Deletion Unsuccessful";
                return RedirectToAction("Index", "Admin");
            }
        }
        public IActionResult Products()
        {
            string sql = "Select * from Inventory Order By ProductID";
            var products = _context.Inventories.FromSqlRaw(sql).ToList();
            return View(products);
        }
        public IActionResult DeleteProduct(int ProductId)
        {
            bool success = Classes.User.DeleteProduct(ProductId, _context);
            if (success)
            {
                TempData["successMessage"] = "Product Deleted";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["errorMessage"] = "Product Deletion Unsuccessful";
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpGet]
        public IActionResult EditProduct(int ProductId)
        {
            string sql = $"Select * from Inventory WHERE ProductID={ProductId}";
            var product = _context.Inventories.FromSqlRaw(sql).ToList();

            ViewBag.ProductId = product[0].ProductId;
            ViewBag.ProductPrice = product[0].ProductPrice;
            ViewBag.ProductType = product[0].ProductType;
            ViewBag.ProductColor = product[0].ProductColor;
            ViewBag.ProductEquipmentSize = product[0].ProductEquipmentSize;
            ViewBag.ProductApparelSize = product[0].ProductApparelSize;
            ViewBag.ImagePath = product[0].ImagePath;
            ViewBag.Name = product[0].Name;
            ViewBag.Brand = product[0].Brand;
            ViewBag.SellerId = product[0].SellerId;


            return View();
        }
        [HttpPost]
        public IActionResult EditProduct([Bind] Inventory product)
        {
            bool success = Product.EditProduct(product, _context);

            if (success)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
        }
        public IActionResult Orders()
        {
            string sql = "Select * from OrderLog";
            var orders = _context.OrderLogs.FromSqlRaw(sql).ToList();

            return View(orders);
        }
        public IActionResult RefundOrder(int OrderNum)
        {
            bool success = Order.RefundOrder(OrderNum, _context);
            if (success)
            {
                TempData["successMessage"] = "Order Refunded";
                return RedirectToAction("Orders", "Admin");
            }
            else
            {
                TempData["errorMessage"] = "Refund Unsuccessful";
                return RedirectToAction("Orders", "Admin");
            }
        }
    }
}
