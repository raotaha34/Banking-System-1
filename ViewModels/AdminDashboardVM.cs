namespace BankingSystemweb.ViewModels
{
    public class AdminDashboardVM
    {
        internal int TotalUsers;

        public string AdminName { get; set; } = string.Empty;
        public double AdminBalance { get; set; }
        public int TotalInActiveUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int InactiveAccounts { get; set; }
        public decimal TodaysDeposits { get; set; }   // changed from double
        public decimal TodaysWithdrawals { get; set; } // changed from double
        public List<RecentActivityVM> RecentActivities { get; set; } = new();
        public int TotalTransactions { get; set; }
    }
}