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
            Classes.User user1 = new(_context);
            bool loginSuccess = user1.Login(user.UserName, user.HashedPass);
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
            Classes.User NewUser = new(_context)
            {
                UserName = user.UserName,
                Password = user.HashedPass,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                AddressCity = user.AddressCity,
                AddressState = user.AddressState,
                AddressZip = user.AddressZip,
                Admin = user.Admin,
                Seller = user.Seller,
            };
            bool registered = NewUser.Register(NewUser);

            if (registered)
            {
                return RedirectToAction("Login", "User");
            }    
            else
            {
                return View();
            }
        }
    }
}
