using System.ComponentModel.DataAnnotations;

namespace BankingSystemweb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;  // User email for login
        [Required]          
        public string Password { get; set; } = string.Empty;   // User password
        public bool RememberMe { get; set; }   // Optional "Remember me" checkbox
    }
}