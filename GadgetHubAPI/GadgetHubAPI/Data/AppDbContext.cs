using Microsoft.EntityFrameworkCore;
using GadgetHubAPI.Models;

namespace GadgetHubAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        // Existing DbSets
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure decimal precision for money-related fields
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

            // (Optional) enforce GlobalId uniqueness
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.GlobalId)
                .IsUnique();
        }
    }
}
