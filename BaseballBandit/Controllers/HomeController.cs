using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BaseballBandit.Classes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;

namespace BaseballBandit.Controllers
{
    public class HomeController : Controller
    {

        private readonly BaseballBanditContext _context;

        public HomeController(BaseballBanditContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (Classes.User.UserName == null)
            {
                TempData["errorMessage"] = "You must login first";
                return RedirectToAction("Login", "User");
            }
            else
            {
                string sql = "Select * from Inventory Order By ProductID";
                var inventory = _context.Inventories.FromSqlRaw(sql).ToList();

                return View(inventory);
            }
        }
        [HttpPost]
        public IActionResult Index(string? searchString)
        {
            if (Classes.User.UserName == null)
            {
                TempData["errorMessage"] = "You must login first";
                return RedirectToAction("Login", "User");
            }
            else if (!searchString.IsNullOrEmpty())
            {
                string sql = $"Select * from Inventory WHERE Name LIKE '{searchString}%' OR Brand LIKE '{searchString}%'";
                var searchedProducts = _context.Inventories.FromSqlRaw(sql).ToList();

                return View(searchedProducts);
            }
            else
            {
                string sql = "Select * from Inventory Order By ProductID";
                var inventory = _context.Inventories.FromSqlRaw(sql).ToList();

                return View(inventory);
            }
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
            if (Classes.User.UserName == null)
            {
                TempData["errorMessage"] = "You must login first";
                return RedirectToAction("Login", "User");
            }
            else
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

        [HttpGet]
        public IActionResult PaymentSelection()
        {

            return View();
        }

        [HttpPost]
        public IActionResult PaymentSelection(int? PaymentID)
        {
            if (PaymentID != null)
            {
                return RedirectToAction("FinalizeOrder", new { PaymentID });
            }
            else
            {
                TempData["errorMessage"] = "Payment Selection Failed";
                return View();
            }
        }

        public IActionResult FinalizeOrder(int PaymentID)
        {
            bool success = Order.FinalizeOrder(PaymentID, _context);
            if (success)
            {
                TempData["successMessage"] = "Order Placed Successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Order Failed To Be Placed";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult OrderScreen()
        {
            if (Classes.User.UserName == null)
            {
                TempData["errorMessage"] = "You must login first";
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
        }
        public IActionResult RefundOrder(int OrderNum)
        {
            bool success = Order.RefundOrder(OrderNum, _context);
            if (success)
            {
                TempData["successMessage"] = "Refund Successful";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Refund Failed";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult OrderDetails(int OrderNum)
        {
            string sql = $"Exec OrderedProductsDetails {OrderNum}";
            var orderedProducts = _context.details.FromSqlRaw(sql).ToList();

            int paymentID = 0;

            for (int i = 0; i < Order.OrderNum.Count; i++)
            {
                if (Order.OrderNum[i] == OrderNum)
                {
                    ViewBag.Index = i;
                    paymentID = Order.PaymentID[i];
                    break;
                }
            }
            for (int i = 0; i < Classes.User.PaymentID.Count(); i++)
            {
                if (Classes.User.PaymentID[i] == paymentID)
                {
                    ViewBag.CardNumber = Classes.User.CardNumber[i] % 10000;
                }
            }

            return View(orderedProducts);
        }
        [HttpGet]
        public IActionResult AddPayment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPayment([Bind] PaymentInformation data)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        Insert into PaymentInformation(UserID, CardNumber, ExpirationMonth, ExpirationYear, CardCVC)
	                    Values(@UserID, @CardNumber, @ExpirationMonth, @ExpirationYear, @CardCVC)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", Classes.User.UserID);
                        cmd.Parameters.AddWithValue("@CardNum", data.CardNumber);
                        cmd.Parameters.AddWithValue("@ExpirationMonth", data.ExpirationMonth);
                        cmd.Parameters.AddWithValue("@ExpirationYear", data.ExpirationYear);
                        cmd.Parameters.AddWithValue("@CardCVC", data.CardCVC);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                if(Classes.User.AddPayment(data.CardNumber, data.ExpirationMonth, data.ExpirationYear, data.CardCVC))
                {
                    return RedirectToAction("PaymentSelection", "Home");
                }
                else
                {
                    TempData["errorMessage"] = "Payment Failed To Be Added";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Payment Failed To Be Added";
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View();
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
