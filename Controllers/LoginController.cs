
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemweb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var role = await _authService.LoginAsync(model);

            if (role == null)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (role == "User" || role == "Customer") // Customer goes to same page as User
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                // Fallback if some unexpected role exists
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Login");
        }
    }
}