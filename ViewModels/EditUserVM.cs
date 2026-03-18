using System.ComponentModel.DataAnnotations;

namespace BankingSystemweb.ViewModels
{
    public class EditUserVM
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string? Password { get; set; } // optional

        // Account info
        public string AccountNumber { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public double Balance { get; set; }
        public double? InterestRate { get; set; }

        // New properties to fix your errors
        public string Role { get; set; } = "Customer"; // or Admin, Manager
        public string Status { get; set; } = "Inactive"; // Active/Inactive
    }
}