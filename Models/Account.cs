namespace BankingSystemweb.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public string AccountNumber { get; set; } = null!;

        public string AccountType { get; set; } = null!;

        public decimal Balance { get; set; }

        public double? InterestRate { get; set; }

        public DateTime OpeningDate { get; set; }

        public string Status { get; set; } = null!;

        public double? DailyLimit { get; set; }

        public double? MonthlyLimit { get; set; }

        public string? Notes { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now; // ✅ add this


    }
}
