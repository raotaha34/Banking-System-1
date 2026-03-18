using BankingSystemweb.Models;
using Microsoft.AspNetCore.Identity;

namespace BankingSystemweb.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Starting database seeding...");
            
            // Create roles
            var roles = new[] { "Admin", "User" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    Console.WriteLine($"Creating role: {roleName}");
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        Console.WriteLine($"Role {roleName} created successfully");
                    }
                }
                else
                {
                    Console.WriteLine($"Role {roleName} already exists");
                }
            }

            // Create admin user
            var adminEmail = "raotaha34@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                Console.WriteLine($"Creating admin user: {adminEmail}");
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine("Admin user created successfully");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin user added to Admin role");
                }
            }
            else
            {
                Console.WriteLine($"Admin user {adminEmail} already exists");
            }

            // Create regular user
            var userEmail = "raotaha33@gmail.com";
            var normalUser = await userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                Console.WriteLine($"Creating regular user: {userEmail}");
                normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FullName = "Normal User",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(normalUser, "User@123");
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create regular user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine("Regular user created successfully");
                    await userManager.AddToRoleAsync(normalUser, "User");
                    Console.WriteLine("Regular user added to User role");
                }
            }
            else
            {
                Console.WriteLine($"Regular user {userEmail} already exists");
            }
            
            Console.WriteLine("Database seeding completed!");
        }
    }
}