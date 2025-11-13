using GadgetCentralService.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetCentralService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Quotation> Quotations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18,2)");
            builder.Entity<Order>().Property(o => o.Discount).HasColumnType("decimal(18,2)");
            builder.Entity<Quotation>().Property(q => q.Price).HasColumnType("decimal(18,2)");
        }
    }
}
