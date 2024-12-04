using BaseballBandit.Classes;
using BaseballBandit.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseballBandit.Controllers
{
    public class UserController : Controller
    {
        private readonly BaseballBanditContext _context;

        public UserController(BaseballBanditContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([Bind] Models.User user)
        {
            //Classes.User user1 = new(_context);
            bool loginSuccess = Classes.User.Login(user.UserName, user.HashedPass, _context);
            if(loginSuccess)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errorMessage"] = "Login Failed";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([Bind] Models.User user)
        {
            
            bool registered = Classes.User.Register(user, _context);

            if (registered)
            {
                return RedirectToAction("Login", "User");
            }    
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            CartClass.LogoutCart();
            Order.LogoutOrders();
            Classes.User.LogoutUser();
            return RedirectToAction("Login", "User");
        }
    }
}
