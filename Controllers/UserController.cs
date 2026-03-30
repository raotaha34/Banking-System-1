using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystemweb.Controllers
{
    [Authorize(Roles = "User,Customer")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionService _transactionService;
        public UserController(IUserService userService, UserManager<ApplicationUser> userManager , ITransactionService transactionService)
        {
            _userService = userService;
            _userManager = userManager;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Login");

            var data = await _userService.GetUserDashboardAsync(user.Id);

            return View(data);
        }


        public async Task<IActionResult> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Login");

            var data = await _userService.GetUserDashboardAsync(user.Id);

            return View(data);
        }


    
    }
}