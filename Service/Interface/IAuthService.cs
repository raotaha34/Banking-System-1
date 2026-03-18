using BankingSystemweb.ViewModels;

namespace BankingSystemweb.Service.Interface
{

    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}

