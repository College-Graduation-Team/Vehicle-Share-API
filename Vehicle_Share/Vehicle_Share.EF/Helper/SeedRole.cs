using Microsoft.AspNetCore.Identity;
using Vehicle_Share.EF.Data;

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