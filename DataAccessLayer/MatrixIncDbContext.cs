using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class MatrixIncDbContext : DbContext
    {
        public MatrixIncDbContext(DbContextOptions<MatrixIncDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer-Order relatie (one-to-many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired();

            // Product-Order many-to-many (met expliciete join table configuratie)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Orders)
                .WithMany(o => o.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderProduct", // Naam van de join table
                    j => j.HasOne<Order>().WithMany().HasForeignKey("OrdersId"),
                    j => j.HasOne<Product>().WithMany().HasForeignKey("ProductsId"),
                    j =>
                    {
                        j.HasKey("OrdersId", "ProductsId"); // Samengestelde primary key
                        j.ToTable("OrderProduct"); // Expliciete tabelnaam
                    });

            // Part-Product many-to-many (indien je deze blijft gebruiken)
            modelBuilder.Entity<Part>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Parts)
                .UsingEntity(j => j.ToTable("PartProduct"));

            base.OnModelCreating(modelBuilder);
        }

    }
}
