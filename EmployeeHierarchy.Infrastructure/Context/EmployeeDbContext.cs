using EmployeeHierarchy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Infrastructure.Context
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<EmployeeData> Employees { get; set; }

        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<EmployeeData>()
            .HasOne(e => e.Manager)
            .WithMany(m => m.ManagedEmployees)
            .HasForeignKey(e => e.ManagerEmployeeId)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}