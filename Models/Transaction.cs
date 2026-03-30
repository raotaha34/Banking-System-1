namespace BankingSystemweb.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; } = null!;

        public decimal Amount { get; set; }

        public string? TransactionType { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Description { get; set; }
        public int? ReceiverAccountId { get; set; }
        public string? ReceiverName { get; set; }
    }
}
