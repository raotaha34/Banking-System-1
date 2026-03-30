
using System.ComponentModel.DataAnnotations;

namespace BankingSystemweb.ViewModels
{
    public class TransactionVM
    {

        public int AccountId { get; set; }

        public int? ReceiverAccountId { get; set; }

        [Required]
        public decimal Amount { get; set; } = 0;

        [Required]
        public string? TransactionType { get; set; }

        public string? Description { get; set; }
    }

}