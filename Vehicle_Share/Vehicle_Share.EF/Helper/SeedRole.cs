using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vehicle_Share.EF.Data;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.EF.Helper
{
    public static class SeedRole
    {
        public static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            if (!context.Roles.Any())
            {
                // Create the roles
                var userRole = new IdentityRole { Name = "User", NormalizedName = "User".ToUpper() };
                var adminRole = new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() };

                context.Roles.Add(userRole);
                context.Roles.Add(adminRole);

                try
                {
                    await context.SaveChangesAsync();

                    // Seed admin user
                    var adminUser = new User
                    {
                        UserName = "Admin", // Set admin username/email here
                        PhoneNumber = "+201234567890", // Set admin phone number here
                        PhoneNumberConfirmed = true,
                        CreatedOn=DateTime.UtcNow,
                    };

                    var password = "@Abdo123"; // Set admin password here

                    var userManager = new UserStore<User>(context);
                    // Hash the password
                    var passwordHasher = new PasswordHasher<User>();
                    var hashedPassword = passwordHasher.HashPassword(adminUser, password);

                    adminUser.PasswordHash = hashedPassword;

                    var result = await userManager.CreateAsync(adminUser);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        // Handle errors if user creation or role assignment fails
                        foreach (var error in result.Errors)
                        {
                            await Console.Out.WriteLineAsync(error.Description);
                        }
                    }
                }

                catch (Exception ex)
                {
                    // Handle any errors that may occur during seeding
                    Console.WriteLine(ex.Message);
                    throw;
                }
           
            
            }
        }

    }
}