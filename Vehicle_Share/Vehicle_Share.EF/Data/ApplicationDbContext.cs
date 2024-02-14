﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.EF.Data
{
    public class ApplicationDbContext :IdentityDbContext<User>
    {

        public DbSet<UserData> UserData { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<LicenseUser> LicenseUser { get; set; }
        public DbSet<Request> Request { get; set; }
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
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
