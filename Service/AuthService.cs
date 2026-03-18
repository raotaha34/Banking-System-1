using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BankingSystemweb.Service
{

    public class AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginViewModel model)
        {
            if (model?.Email == null || model?.Password == null)
                return null;

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return null;

            var result = await signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                false);

            if (!result.Succeeded)
                return null;

            var roles = await userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}