using DierenManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DierenManagement.DbContextFile
{
    public class AnimalManagementDbContext : DbContext
    {

        public AnimalManagementDbContext(DbContextOptions<AnimalManagementDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Animal> Animals { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Animal>().HasKey(a => a.id);
        //    ;
        //}
    }
}
