using DierenManagement.Models;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DierenManagement.DbContextFile
{
    public class AnimalManagementDbContext : IdentityDbContext<User>
    {

        public AnimalManagementDbContext(DbContextOptions<AnimalManagementDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var Admin = new IdentityRole("Admin");
            Admin.NormalizedName = "Admin";

            var Client = new IdentityRole("Client");
            Client.NormalizedName = "Client";

            builder.Entity<IdentityRole>().HasData(Admin,Client);
        }
    }
}
