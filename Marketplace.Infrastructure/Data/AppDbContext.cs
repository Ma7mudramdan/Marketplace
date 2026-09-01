using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Marketplace.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ShoppingCart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureProduct(modelBuilder);
            ConfigureOrder(modelBuilder);
            ConfigureShoppingCart(modelBuilder);
            ConfigureFavorite(modelBuilder);
            ConfigureReview(modelBuilder);

            new SeedData(modelBuilder);

        }

        private void ConfigureShoppingCart(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(s => s.Id);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(ci => ci.Quantity)
                      .IsRequired();

                entity.HasOne(ci => ci.ShoppingCart)
                      .WithMany(s => s.Items)
                      .HasForeignKey(ci => ci.ShoppingCartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                      .WithMany(p => p.CartItems)
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ci => ci.ProductId)
                      .HasDatabaseName("XI_CartItems_ProductId");

                entity.HasIndex(ci => ci.ShoppingCartId)
                      .HasDatabaseName("XI_CartItems_ShoppingCartId");

            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(e => e.Quantity)
                  .IsRequired();

                entity.Property(e => e.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountApplied)
                    .HasPrecision(18, 2);

                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(oi => oi.OrderId)
                      .HasDatabaseName("IX_OrderItems_OrderId");

                entity.HasIndex(oi => oi.ProductId)
                      .HasDatabaseName("IX_OrderItems_ProductId");
            });
        }

        private void ConfigureOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(e => e.ShippingAddress)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShippingCity)
                    .HasMaxLength(50);

                entity.Property(e => e.ShippingCountry)
                    .HasMaxLength(50);

                entity.Property(e => e.ShippingPostalCode)
                    .HasMaxLength(20);

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100);

                entity.Property(e => e.TrackingNumber)
                    .HasMaxLength(100);

                entity.Property(e => e.Notes)
                    .HasMaxLength(500);

                entity.Property(e => e.Subtotal)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TaxAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.ShippingCost)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalAmount)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.HasOne(o => o.Customer)
                      .WithMany(o => o.Orders)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.CustomerId)
                      .HasDatabaseName("IX_Orders_CustomerId");

                entity.HasIndex(o => o.IsActive)
                      .HasDatabaseName("IX_Orders_IsActive");

                entity.HasIndex(o => o.Status)
                      .HasDatabaseName("IX_Orders_Status");

                entity.HasIndex(o => o.OrderDate)
                      .HasDatabaseName("IX_Orders_OrderDate");

            });
        }

        private void ConfigureProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Description)
                      .HasMaxLength(2000)
                      .IsRequired();

                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);

                entity.Property(p => p.DiscountedPrice)
                     .HasPrecision(18, 2);

                entity.Property(e => e.Location)
                   .HasMaxLength(100);

                entity.Property(e => e.Condition)
                    .IsRequired();

                
                entity.Property(e => e.RowVersion)
                    .IsRowVersion();

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Seller)
                      .WithMany(s => s.Products)
                      .HasForeignKey(p => p.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);


                entity.HasIndex(p => p.CategoryId)
                      .HasDatabaseName("IX_Products_CategoryId");

                entity.HasIndex(p => p.SellerId)
                      .HasDatabaseName("IX_Products_SellerId");

                entity.HasIndex(p => p.IsActive)
                      .HasDatabaseName("IX_Products_IsActive");

                entity.HasIndex(p => p.CreatedAt)
                      .HasDatabaseName("IX_Products_CreatedAt");
            });
        }

        private void ConfigureCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(entity => entity.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(c => c.Name)
                      .IsUnique();

                entity.Property(c => c.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.Icon)
                    .HasMaxLength(50);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200);

                entity.HasOne( c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Index for performance
                entity.HasIndex(e => e.ParentCategoryId)
                    .HasDatabaseName("IX_Categories_ParentCategoryId");
            });
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PhoneNumber)
                      .HasMaxLength(11);

                entity.Property(u => u.Bio)
                      .HasMaxLength(500);

                entity.Property(u => u.Address)
                      .HasMaxLength(200);
                
                entity.Property(u => u.City)
                      .HasMaxLength(50);

                entity.Property(u => u.Country)
                      .HasMaxLength(50); 


                entity.HasMany(u => u.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationUser>()
                      .WithOne(u => u.UserProfile)
                      .HasForeignKey<ApplicationUser>(u => u.BusinessUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(e => e.IdentityUser);

            });
                
        }

        private void ConfigureFavorite(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AddedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Favorites)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(e => e.Favorites)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint - one favorite per product per user
                entity.HasIndex(e => new { e.UserId, e.ProductId })
                    .IsUnique()
                    .HasDatabaseName("IX_Favorites_UserId_ProductId");
            });
        }

        private void ConfigureReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.Comment)
                    .HasMaxLength(1000);

                // Relationships
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint - one review per product per user
                entity.HasIndex(e => new { e.ProductId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("IX_Reviews_ProductId_UserId");
            });
        }

        

    }
}

