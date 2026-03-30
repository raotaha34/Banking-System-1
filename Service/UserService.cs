using BankingSystemweb.Data;
using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace BankingSystemweb.Service
{

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDashboardVM?> GetUserDashboardAsync(string userId)
        {
            var user = await _context.Users!
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var vm = new UserDashboardVM
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,

                Accounts = user.Accounts.Select(a => new UserAccountVM
                {
                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType,
                    Balance = a.Balance,
                    InterestRate = a.InterestRate,
                    OpeningDate = a.OpeningDate,
                    Status = a.Status
                }).ToList()
            };
            return vm;
        }
    }
}
