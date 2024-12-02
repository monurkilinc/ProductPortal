using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.DataAccess.Contexts
{
    public class ProductPortalContext : DbContext
    {
        public ProductPortalContext(DbContextOptions<ProductPortalContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Product Tablosu
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Stock)
                   .IsRequired();

                entity.Property(e => e.Price)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ImageURL)
                      .IsRequired()
                      .HasMaxLength(500);

            });

            //User Tablosu
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Department)
                    .HasMaxLength(20);
                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                entity.Property(e => e.PasswordSalt)
                      .IsRequired();

                entity.Property(e => e.Role)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.IsActive)
                      .IsRequired();
            });

            modelBuilder.Entity<Customer>()
                      .HasMany(c => c.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                      .HasMany(o => o.OrderItems)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<SupportTicket>()
                     .HasMany(t => t.Messages)
                     .WithOne(m => m.Ticket)
                     .HasForeignKey(m => m.TicketId);
        }
    }
}
