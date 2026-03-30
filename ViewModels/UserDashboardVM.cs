namespace BankingSystemweb.ViewModels
{
    public class UserDashboardVM
    {
        // User Info
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        // Accounts
        public List<UserAccountVM> Accounts { get; set; } = new();
    }
}
