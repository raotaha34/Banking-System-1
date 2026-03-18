using Microsoft.AspNetCore.Identity;

namespace BankingSystemweb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

        // Navigation property
        public ICollection<Account> Accounts { get; set; } = null!;
    }
}