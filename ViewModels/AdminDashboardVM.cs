namespace BankingSystemweb.ViewModels
{
    public class AdminDashboardVM
    {
        public string AdminName { get; set; } = string.Empty;
        public double AdminBalance { get; set; }

        public List<RecentActivityVM> RecentActivities { get; set; } = new();
    }
}