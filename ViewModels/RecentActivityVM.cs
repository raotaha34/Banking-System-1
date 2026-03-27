namespace BankingSystemweb.ViewModels
    {
        public class RecentActivityVM
        {
            // Title of the activity
            public string Title { get; set; } = "";

            // Description, e.g., "$5000 — Account #12345" or "John Doe — john@example.com"
            public string Description { get; set; } = "";

            // Icon to display in UI (U = User, D = Deposit, W = Withdraw, T = Transfer, A = Account)
            public string Icon { get; set; } = "";

            // Color for UI dot (blue, teal, orange, purple, green)
            public string Color { get; set; } = "";

            // Date/time of activity
            public DateTime Time { get; set; }
        }
    }

