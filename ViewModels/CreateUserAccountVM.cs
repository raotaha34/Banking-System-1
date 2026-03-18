using System.ComponentModel.DataAnnotations;

namespace BankingSystemweb.ViewModels
{
    public class CreateUserAccountVM
    {
        // User
        [Required]
        public string? FullName { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        // Account
        public string AccountNumber { get; set; } = null!;

        public string AccountType { get; set; } = null!;

        public double Balance { get; set; }

        public double? InterestRate { get; set; }
    }
}