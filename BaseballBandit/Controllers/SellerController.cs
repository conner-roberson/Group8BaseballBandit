using Microsoft.AspNetCore.Mvc;
using BaseballBandit.Classes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BaseballBandit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseballBandit.Controllers
{
    public class SellerController : Controller
    {
        private readonly BaseballBanditContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SellerController(IWebHostEnvironment webHostEnvironment, BaseballBanditContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddNewProducts()
        {
            string sql = $"Select * from Inventory WHERE SellerId={Classes.User.UserID}";
            var products = _context.Inventories.FromSqlRaw(sql).ToList();

            ViewBag.Brand = products[0].Brand;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewProducts([Bind] Inventory product, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string imagePath = "";
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        imagePath = Path.Combine(uniqueFileName);

                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imagePath);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                    }

                    product.ImagePath = imagePath;

                    using (var con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                    {
                        var cmd = new SqlCommand("AddProduct", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@ProductType", product.ProductType);
                        cmd.Parameters.AddWithValue("@ProductColor", product.ProductColor);
                        cmd.Parameters.AddWithValue("@ProductEquipmentSize", product.ProductEquipmentSize);
                        cmd.Parameters.AddWithValue("@ProductApparelSize", product.ProductApparelSize ?? "");
                        cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? "");
                        cmd.Parameters.AddWithValue("@Brand", product.Brand);
                        cmd.Parameters.AddWithValue("@Name", product.Name);
                        cmd.Parameters.AddWithValue("@SellerId", product.SellerId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    TempData["successMessage"] = "Product added successfully!";
                    return RedirectToAction("AllProducts", "Seller");
                }
                else
                {
                    TempData["errorMessage"] = "Product could not be added.";
                    return RedirectToAction("AllProducts", "Seller");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("AllProducts", "Seller");
            }
        }
        public IActionResult AllProducts()
        {
            string sql = $"Select * from Inventory WHERE SellerId={Classes.User.UserID}";
            var products = _context.Inventories.FromSqlRaw(sql).ToList();

            return View(products);
        }
        public IActionResult DeleteProduct(int productId)
        {
            bool success = Classes.User.DeleteProduct(productId, _context);
            if (success)
            {
                TempData["successMessage"] = "Product Deleted";
                return RedirectToAction("AllProducts", "Seller");
            }
            else
            {
                TempData["errorMessage"] = "Product Deletion Unsuccessful";
                return RedirectToAction("AllProducts", "Seller");
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
            try
            {
                if (ModelState.IsValid)
                {
                    SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True");
                    SqlCommand cmd = new SqlCommand("EditProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                    cmd.Parameters.AddWithValue("@ProductType", product.ProductType);
                    cmd.Parameters.AddWithValue("@ProductColor", product.ProductColor);
                    cmd.Parameters.AddWithValue("@ProductEquipmentSize", product.ProductEquipmentSize);
                    cmd.Parameters.AddWithValue("@ProductApparelSize", product.ProductApparelSize ?? "");
                    cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? "");
                    cmd.Parameters.AddWithValue("@Brand", product.Brand);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@SellerId", product.SellerId);
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    TempData["successMessage"] = "Edit Successful";
                    return RedirectToAction("AllProducts", "Seller");
                }
                else
                {
                    TempData["errorMessage"] = "Edit Could Not Be Processed";
                    return View(new { product.ProductId });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("AllProducts", "Seller");
            }
        }

        public IActionResult Sales()
        {
            string sql = $"Exec SoldProducts {Classes.User.UserID}";
            var products = _context.details.FromSqlRaw(sql).ToList();

            var groupedProducts = products
                                .GroupBy(p => p.ProductId) 
                                .Select(g => new
                                {
                                    ProductId = g.Key,
                                    TotalQuantity = g.Sum(p => p.Quantity), 
                                    ProductDetails = g.First() 
                                })
                                .ToList();

            var result = groupedProducts.Select(g => new OrderedProductsDetails
            {
                ProductId = g.ProductId,
                Quantity = g.TotalQuantity,
                Name = g.ProductDetails.Name,
                ProductPrice = g.ProductDetails.ProductPrice,
                ProductType = g.ProductDetails.ProductType,
                ProductColor = g.ProductDetails.ProductColor,
                ImagePath = g.ProductDetails.ImagePath,
                Brand = g.ProductDetails.Brand,
                SellerId = g.ProductDetails.SellerId,
                ProductEquipmentSize = g.ProductDetails.ProductEquipmentSize,
                ProductApparelSize = g.ProductDetails.ProductApparelSize
            }).ToList();
            double revenue = 0;
            int quantity = 0;

            for(int i = 0; i < result.Count; i++)
            {
                revenue += (result[i].Quantity * result[i].ProductPrice);
                quantity += (result[i].Quantity);
            }

            ViewBag.TotalRevenue = revenue;
            ViewBag.TotalProductsSold = quantity;

            return View(result);
        }
    }
}
