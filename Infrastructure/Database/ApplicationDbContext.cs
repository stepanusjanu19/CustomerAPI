using Microsoft.EntityFrameworkCore;
using Domain.Model;

namespace Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customer");
                e.HasKey(a => a.CustomerId);
                e.Property(a => a.CustomerId)
                    .HasColumnName("customerId")
                    .HasDefaultValueSql("gen_random_uuid()");
                e.Property(a => a.CustomerCode).HasColumnName("customerCode").HasMaxLength(50).IsRequired();
                e.Property(a => a.CustomerName).HasColumnName("customerName").HasMaxLength(255).IsRequired();
                e.Property(a => a.CustomerAddress).HasColumnName("customerAddress").HasDefaultValue("").IsRequired();
                e.Property(a => a.CreatedBy).HasColumnName("createdBy").IsRequired();
                e.Property(a => a.CreatedAt).HasColumnName("createdAt").IsRequired();
                e.Property(a => a.ModifiedBy).HasColumnName("modifiedBy");
                e.Property(a => a.ModifiedAt).HasColumnName("modifiedAt");
                e.HasIndex(a => a.CustomerCode).IsUnique();
            });
        }
    }
}