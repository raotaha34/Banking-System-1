using BankingSystemweb.Service.Interface;

using BankingSystemweb.ViewModels;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;



namespace BankingSystemweb.Controllers

{



    [Authorize(Roles = "Admin")]

    public class AdminController : Controller

    {



        private readonly IAdminService _adminService;

        private readonly ITransactionService _transactionService;



        public AdminController(

            IAdminService adminService,

            ITransactionService transactionService)

        {

            _adminService = adminService;

            _transactionService = transactionService;

        }   

        public async Task<IActionResult> Index  ()

        {
            //1. Call service to get total users

            int totalUsers = await _adminService.GetTotalUsersAsync();

            // 2. Get current admin ID from claims

            string adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            // 3. Get admin name and balance

            var adminInfo = await _adminService.GetAdminInfoAsync(adminId);



            int tt = await _transactionService.GetTotalTransactionsAsync(); //tt = totaltransaction

            var activeUsers = await _adminService.GetUsersByStatusAsync("Active");

            int totalActive = activeUsers.Count;



            ViewData["AdminName"] = adminInfo.AdminName;

            ViewData["AdminBalance"] = adminInfo.AdminBalance;

            ViewData["TotalUsers"] = totalUsers;

            ViewData["TotalTransactions"] = tt;

            ViewData["TotalActiveUsers"] = totalActive;


            var recentActivities = await _transactionService.GetRecentActivitiesAsync(adminId);



            var dashboardVM = new AdminDashboardVM

            {

                RecentActivities = recentActivities,

            };



            return View(dashboardVM);

        }





        [HttpGet]

        public IActionResult CreateUserAccount()

        {

            return View(); // will automatically look for Views/Admin/CreateUserAccount.cshtml

        }





        [HttpPost]
        public async Task<IActionResult> CreateUserAccount(CreateUserAccountVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                 await _adminService.CreateUserAndAccount(model);
                TempData["SuccessMessage"] = "User account created successfully!";
                return RedirectToAction("CreateUserAccount", "Admin");
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                if (message.Contains("password", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("Password", message);
                else if (message.Contains("email", StringComparison.OrdinalIgnoreCase) ||
                         message.Contains("username", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("Email", message);
                else
                    ModelState.AddModelError(string.Empty, message);

                return View(model);
            }
        }
        //public async Task<IActionResult> CreateUserAccount(CreateUserAccountVM model)

        //{

        //    if (ModelState.IsValid)

        //    {

        //        var result = await _adminService.CreateUserAndAccount(model);



        //        if (result)

        //        {

        //            // Pass success message using TempData

        //            TempData["SuccessMessage"] = "User account created successfully!";

        //            return RedirectToAction("CreateUserAccount", "Admin");

        //        }



        //        // Pass error message if creation failed

        //        TempData["ErrorMessage"] = "User creation failed.";

        //    }

        //    return View(model);

        //}





        // =========================

        // ✅ TRANSACTION PAGE

        // =========================



        [HttpGet]

        public async Task<IActionResult> Transaction  ()

        {

            var transactions = await _transactionService.GetAllTransactions();

            return View(transactions);

        }





        [HttpPost]

        public async Task<IActionResult> Transaction(TransactionVM model)

        {

            if (!ModelState.IsValid)

            {

                var data = await _transactionService.GetAllTransactions();

                return View(data);

            }



            try

            {

                await _transactionService.CreateTransaction(model);



                TempData["SuccessMessage"] = "Transaction completed successfully!";

            }

            catch (Exception ex)

            {

                TempData["ErrorMessage"] = ex.Message;

            }



            return RedirectToAction(nameof(Transaction));

        }





        [HttpGet]
        public async Task<IActionResult> ManageUser()

        {

            int totalUsers = await _adminService.GetTotalUsersAsync();

            // Active users

            var activeUsers = await _adminService.GetUsersByStatusAsync("Active");

            int totalActive = activeUsers.Count;



            // Inactive users

            var inactiveUsers = await _adminService.GetUsersByStatusAsync("Inactive");

            int totalInactive = inactiveUsers.Count;



            var allUsers = await _adminService.GetAllUsersAsync(); // pass to view

            var totaladmins =await _adminService.GetTotalAdminsAsync();



            ViewData["TotalUsers"] = totalUsers;

            ViewData["TotalActiveUsers"] = totalActive;

            ViewData["TotalInactiveUsers"] = totalInactive;

            ViewData["TotalAdmins"] = totaladmins;

            return View(allUsers);

        }







        [HttpGet]

        public async Task<IActionResult> EditUser(string id)

        {

            if (string.IsNullOrEmpty(id)) return NotFound();



            var vm = await _adminService.GetEditUserByIdAsync(id);

            if (vm == null) return NotFound();



            return View(vm); // correctly passes EditUserVM

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditUser(EditUserVM model)

        {

            if (!ModelState.IsValid) return View(model);



            var result = await _adminService.UpdateUserAsync(model);



            if (result)

            {

                TempData["SuccessMessage"] = "User updated successfully!";

                return RedirectToAction("EditUser", new { id = model.Id });

            }



            TempData["ErrorMessage"] = "Failed to update user.";

            return View(model);

        }


        [HttpGet]
       
        public async Task<IActionResult> ManageAccounts()

        {

            int totalUsers = await _adminService.GetTotalUsersAsync();

            // Active users

            var activeUsers = await _adminService.GetUsersByStatusAsync("Active");

            int totalActive = activeUsers.Count;



            // Inactive users

            var inactiveUsers = await _adminService.GetUsersByStatusAsync("Inactive");

            int totalInactive = inactiveUsers.Count;



            var allUsers = await _adminService.GetAllUsersAsync(); // pass to view

            var totaladmins = await _adminService.GetTotalAdminsAsync();



            ViewData["TotalUsers"] = totalUsers;

            ViewData["TotalActiveUsers"] = totalActive;

            ViewData["TotalInactiveUsers"] = totalInactive;

            ViewData["TotalAdmins"] = totaladmins;

            return View(allUsers);

        }

        public async Task<IActionResult> Report()

        {
            //1. Call service to get total users

            int totalUsers = await _adminService.GetTotalUsersAsync();

            // 2. Get current admin ID from claims

            string adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            // 3. Get admin name and balance

            var adminInfo = await _adminService.GetAdminInfoAsync(adminId);

            int tt = await _transactionService.GetTotalTransactionsAsync(); //tt = totaltransaction

            var activeUsers = await _adminService.GetUsersByStatusAsync("Active");

            int totalActive = activeUsers.Count;


            ViewData["AdminName"] = adminInfo.AdminName;

            ViewData["AdminBalance"] = adminInfo.AdminBalance;

            ViewData["TotalUsers"] = totalUsers;

            ViewData["TotalTransactions"] = tt;

            ViewData["TotalActiveUsers"] = totalActive;

            var recentActivities = await _transactionService.GetRecentActivitiesAsync(adminId);


            var dashboardVM = new AdminDashboardVM

            {

                RecentActivities = recentActivities,

            };

            return View(dashboardVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID is required");

            // Call your service method to delete the user
            var result = await _adminService.DeleteUserAsync(id);

            if (!result)
                return NotFound("User not found or could not be deleted");

            return Ok();
        }



    }

}

