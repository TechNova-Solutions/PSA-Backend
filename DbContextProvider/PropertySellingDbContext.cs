using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSA_Business_Logic.Model;

namespace PSA_Business_Logic.DbContextProvider
{
    public class PropertySellingDbContext : IdentityDbContext
    {
        public PropertySellingDbContext(DbContextOptions options) : base(options) { }


        public new DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //SeedRoles(builder);
            // Add other customizations to the model if needed
        }


        //private static void SeedRoles(ModelBuilder builder)
        //{
        //    builder.Entity<IdentityRole>().HasData
        //        (
        //        new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
        //        new IdentityRole() { Name = "Customer", ConcurrencyStamp = "2", NormalizedName = "Customer" });       
        //}
    }
}
