using BankingSystemweb.Data;
using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BankingSystemweb.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        //create account
        public async Task<bool> CreateUserAndAccount(CreateUserAccountVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName ?? ""
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var account = new Account
            {
                AccountNumber = model.AccountNumber ?? "",
                AccountType = model.AccountType ?? "Checking",
                Balance = model.Balance,
                InterestRate = model.InterestRate ?? 0,
                OpeningDate = DateTime.UtcNow,
                Status = "Active",
                UserId = user.Id
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return true;
        }

        //total user no 
        public async Task<int> GetTotalUsersAsync()
        {
            // Count all users in the database
            return await _userManager.Users.CountAsync();
        }


      
        // ✅ Get admin name and balance
        public async Task<AdminDashboardVM> GetAdminInfoAsync(string adminId)
        {
            var adminUser = await _userManager.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Id == adminId);

            if (adminUser == null)
            {
                return new AdminDashboardVM
                {
                    AdminName = "Admin",
                    AdminBalance = 0
                };
            }

            var adminAccount = adminUser.Accounts.FirstOrDefault(); // first account is admin's account

            return new AdminDashboardVM
            {
                AdminName = adminUser.FullName ?? "Admin",
                AdminBalance = adminAccount?.Balance ?? 0
            };
        }

        //get all users with role and status
        public async Task<List<UserListVM>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Accounts)
                .ToListAsync();

            var userList = new List<UserListVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // get roles
                var account = user.Accounts.FirstOrDefault();

                userList.Add(new UserListVM
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email!,
                    Role = string.Join(", ", roles),
                    Status = account?.Status ?? "Inactive"
                });
            }

            return userList;
        }
        //get user by status
        public async Task<List<UserListVM>> GetUsersByStatusAsync(string status)
        {
            var allUsers = await GetAllUsersAsync();
            return allUsers.Where(u => u.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }
      
        public async Task<int> GetTotalAdminsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Count;
        }

        public async Task<UserListVM?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var account = user.Accounts.FirstOrDefault();

            return new UserListVM
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email!,
                Role = string.Join(", ", roles),
                Status = account?.Status ?? "Inactive"
            };
        }

        public async Task<bool> UpdateUserAsync(EditUserVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!passResult.Succeeded) return false;
            }

            // Update role
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            var result = await _userManager.UpdateAsync(user);

            // Update account fields
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == user.Id);
            if (account != null)
            {
                account.Status = model.Status;
                account.AccountNumber = model.AccountNumber;
                account.AccountType = model.AccountType;
                account.Balance = model.Balance;
                account.InterestRate = model.InterestRate;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }

            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Delete accounts first
            var accounts = _context.Accounts.Where(a => a.UserId == user.Id);
            _context.Accounts.RemoveRange(accounts);

            var result = await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            return result.Succeeded;
        }

        //edit
        public async Task<EditUserVM?> GetEditUserByIdAsync(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var account = user.Accounts.FirstOrDefault();

            return new EditUserVM
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "Viewer",
                Status = account?.Status ?? "Inactive",
                AccountNumber = account?.AccountNumber ?? "",
                AccountType = account?.AccountType ?? "",
                Balance = account?.Balance ?? 0,
                InterestRate = account?.InterestRate ?? 0
            };
        }
    }
}