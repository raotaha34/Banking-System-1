using BankingSystemweb.Models;
using BankingSystemweb.Service;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingSystemweb.Controllers
{
    [Authorize(Roles = "User,Customer")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionService _transactionService;
        public UserController(IUserService userService, UserManager<ApplicationUser> userManager, ITransactionService transactionService)
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

        [HttpGet]
        public async Task<IActionResult> Transactions()
        {
            // 1️⃣ Get logged-in user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // 2️⃣ Fetch transactions for the current user
            var transactions = await _transactionService.GetUserTransactions(userId);

            // 3️⃣ Pass the transactions to the view
            return View(transactions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transactions(TransactionVM model)
        {
            // 1️⃣ Get the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // 2️⃣ Validate model
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
                return View(model);
            }

            try
            {
                // 3️⃣ Call service to create the transaction
                await _transactionService.CreateTransactions(model, userId);
                TempData["SuccessMessage"] = "Transaction completed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }

            // 4️⃣ Redirect to GET Transactions page to show updated list
            return RedirectToAction(nameof(Transactions));
        }


        //Notification
        [HttpGet]
        public async Task<IActionResult> Notification()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Login");

            var data = await _userService.GetUserDashboardAsync(user.Id);

            return View(data);
        }

        //Forget Password
        [HttpGet]
        public async Task<IActionResult> Forgetpassword()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Login");

            var data = await _userService.GetUserDashboardAsync(user.Id);

            return View(data);
        }
    }
}