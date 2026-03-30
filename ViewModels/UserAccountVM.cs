namespace BankingSystemweb.ViewModels
{
    public class UserAccountVM
    {
        public string AccountNumber { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public decimal Balance { get; set; }
        public double? InterestRate { get; set; }
        public DateTime OpeningDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
