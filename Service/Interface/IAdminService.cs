using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemweb.Service.Interface
{
    public interface IAdminService
    {
        Task<bool> CreateUserAndAccount(CreateUserAccountVM model);

        // New method to count all users
        Task<int> GetTotalUsersAsync();

        // ✅ New method to get admin info
        Task<AdminDashboardVM> GetAdminInfoAsync(string adminId);

        Task<List<UserListVM>> GetAllUsersAsync();
        Task<List<UserListVM>> GetUsersByStatusAsync(string status); // "Active" or "Inactive
        Task<int> GetTotalAdminsAsync();

        // ✅ New Edit & Delete Methods
        Task<UserListVM?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserAsync(EditUserVM model);
        Task<bool> DeleteUserAsync(string userId);

        Task<EditUserVM?> GetEditUserByIdAsync(string userId);
    }
}
