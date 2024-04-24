using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.EF.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public DbSet<UserData> UserData { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<License> License { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }

        /*   
         *   protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);
                    modelBuilder.Entity<IdentityUser>().Ignore(c => c.AccessFailedCount)
                                                       .Ignore(c => c.LockoutEnabled)
                                                       .Ignore(c => c.Email)
                                                       .Ignore(c => c.EmailConfirmed)
                                                       .Ignore(c => c.NormalizedEmail)
                                                       .Ignore(c => c.TwoFactorEnabled);//and so on...

                    modelBuilder.Entity<IdentityUser>().ToTable("Users");//to change the name of table.

                }
        */
    }
}
