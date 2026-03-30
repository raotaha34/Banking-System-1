using BankingSystemweb.ViewModels;

namespace BankingSystemweb.Service.Interface
{
    public interface IUserService
    {
        Task<UserDashboardVM?> GetUserDashboardAsync(string userId);
    }
}
