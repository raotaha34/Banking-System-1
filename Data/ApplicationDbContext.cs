using BankingSystemweb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemweb.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Transaction>? Transactions { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal properties to avoid truncation warnings
            builder.Entity<Account>()
                .Property(a => a.InterestRate)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            builder.Entity<Account>()
                .Property(a => a.MonthlyLimit)
                .HasPrecision(18, 2);

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // Additional configurations (if needed) can go here
        }
    }
}