
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Marketplace.Infrastructure.Models;

namespace Marketplace.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                // Check if roles exist, if not create them
                string[] roleNames = { "Admin", "Seller", "Customer" };

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        logger.LogInformation("Role {RoleName} created", roleName);
                    }
                }

                // Create default Admin user if it doesn't exist
                var adminEmail = "admin@marketplace.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123456");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        logger.LogInformation("Admin user created with email: {Email}", adminEmail);
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // Ensure admin user has Admin role
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Admin role assigned to existing user: {Email}", adminEmail);
                    }
                }

                // Create default Seller user (optional)
                var sellerEmail = "seller@marketplace.com";
                var sellerUser = await userManager.FindByEmailAsync(sellerEmail);

                if (sellerUser == null)
                {
                    var seller = new ApplicationUser
                    {
                        UserName = sellerEmail,
                        Email = sellerEmail,
                        FirstName = "Demo",
                        LastName = "Seller",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(seller, "Seller@123456");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(seller, "Seller");
                        logger.LogInformation("Seller user created with email: {Email}", sellerEmail);
                    }
                }

                // Create default Customer user (optional)
                var customerEmail = "customer@marketplace.com";
                var customerUser = await userManager.FindByEmailAsync(customerEmail);

                if (customerUser == null)
                {
                    var customer = new ApplicationUser
                    {
                        UserName = customerEmail,
                        Email = customerEmail,
                        FirstName = "Demo",
                        LastName = "Customer",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(customer, "Customer@123456");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(customer, "Customer");
                        logger.LogInformation("Customer user created with email: {Email}", customerEmail);
                    }
                }

                logger.LogInformation("Database initialization completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }
    }
}