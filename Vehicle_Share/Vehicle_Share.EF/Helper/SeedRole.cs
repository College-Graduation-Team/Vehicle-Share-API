using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
                        PhoneNumber = "+201234567890", // Set admin email here
                        PhoneNumberConfirmed = true
                    };

                    var password = "@Abdo123"; // Set admin password here

                    var userStore = new UserStore<User>(context);
                    var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);

                    var result = await userManager.CreateAsync(adminUser, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        // Handle errors if user creation or role assignment fails
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
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