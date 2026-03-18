using System.ComponentModel.DataAnnotations;

namespace BankingSystemweb.ViewModels
{
    public class TransactionVM
    {

        [Required]
        public int AccountId { get; set; }

        [Required]
        public int? ReceiverAccountId { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string? TransactionType { get; set; }

        [Required]
        public string? Description { get; set; }
    }

}