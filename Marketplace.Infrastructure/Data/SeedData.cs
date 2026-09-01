using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Data
{
    public class SeedData
    {

        private DateTime baseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public SeedData(ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
            UserData(modelBuilder);
            CategoryData(modelBuilder);
            ProductData(modelBuilder);
            ProductImageData(modelBuilder);
            ReviewsData(modelBuilder);
            
        }
        private void CategoryData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics",
                    Description = "Electronic devices and accessories",
                    Icon = "bi-laptop",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 2,
                    Name = "Clothing & Fashion",
                    Description = "Apparel, shoes, and accessories",
                    Icon = "bi-tags",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 3,
                    Name = "Books & Media",
                    Description = "Books, movies, and music",
                    Icon = "bi-book",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 3
                },
                new Category
                {
                    Id = 4,
                    Name = "Home & Garden",
                    Description = "Furniture, decor, and gardening",
                    Icon = "bi-house",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 4
                }
            );

            // Seed Subcategories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 5,
                    Name = "Phones",
                    Description = "Smartphones and accessories",
                    ParentCategoryId = 1,
                    Icon = "bi-phone",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 6,
                    Name = "Laptops",
                    Description = "Laptops and notebooks",
                    ParentCategoryId = 1,
                    Icon = "bi-laptop",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 7,
                    Name = "Men's Clothing",
                    Description = "Clothing for men",
                    ParentCategoryId = 2,
                    Icon = "bi-person",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 8,
                    Name = "Women's Clothing",
                    Description = "Clothing for women",
                    ParentCategoryId = 2,
                    Icon = "bi-person-fill",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 08, 10),
                    DisplayOrder = 2
                }
            );
        }

        private void ProductData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().HasData(
                // ===== PHONES (CategoryId: 5, SellerId: 1 = Ahmed) =====
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro Max",
                    Description = "Latest iPhone with titanium body, A17 Pro chip, 48MP camera",
                    Price = 1199.99m,
                    DiscountedPrice = 1099.99m,
                    StockQuantity = 25,
                    SoldQuantity = 10,
                    Condition = ProductCondition.New,
                    Location = "Cairo, Egypt",
                    CategoryId = 5,     // Phones
                    SellerId = 1,       // Ahmed Seller
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(10),
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null,
                    Views = 350,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S24 Ultra",
                    Description = "Samsung's flagship with AI features, 200MP camera, S Pen",
                    Price = 1199.99m,
                    DiscountedPrice = 1099.99m,
                    StockQuantity = 30,
                    SoldQuantity = 8,
                    Condition = ProductCondition.New,
                    Location = "Cairo, Egypt",
                    CategoryId = 5,     // Phones
                    SellerId = 1,       // Ahmed Seller
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(8),
                    CreatedAt = baseDate.AddDays(3),
                    UpdatedAt = null,
                    Views = 280,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 3,
                    Name = "Google Pixel 8 Pro",
                    Description = "Pure Android experience with advanced AI camera features",
                    Price = 999.99m,
                    DiscountedPrice = null,
                    StockQuantity = 20,
                    SoldQuantity = 5,
                    Condition = ProductCondition.New,
                    Location = "Alexandria, Egypt",
                    CategoryId = 5,     // Phones
                    SellerId = 5,       // Omar TechGuy
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(12),
                    CreatedAt = baseDate.AddDays(7),
                    UpdatedAt = null,
                    Views = 150,
                    RowVersion = Array.Empty<byte>()
                },

                // ===== LAPTOPS (CategoryId: 6, SellerId: 1 = Ahmed) =====
                new Product
                {
                    Id = 4,
                    Name = "MacBook Pro 16-inch M3 Max",
                    Description = "Ultimate performance with M3 Max chip, 36GB RAM, 1TB SSD",
                    Price = 3499.99m,
                    DiscountedPrice = 3299.99m,
                    StockQuantity = 15,
                    SoldQuantity = 4,
                    Condition = ProductCondition.New,
                    Location = "Cairo, Egypt",
                    CategoryId = 6,     // Laptops
                    SellerId = 1,       // Ahmed Seller
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(15),
                    CreatedAt = baseDate.AddDays(10),
                    UpdatedAt = null,
                    Views = 420,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 5,
                    Name = "Dell XPS 15",
                    Description = "Premium Windows laptop with 4K OLED display, i9 processor",
                    Price = 2499.99m,
                    DiscountedPrice = 2299.99m,
                    StockQuantity = 10,
                    SoldQuantity = 3,
                    Condition = ProductCondition.New,
                    Location = "Alexandria, Egypt",
                    CategoryId = 6,     // Laptops
                    SellerId = 5,       // Omar TechGuy
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(13),
                    CreatedAt = baseDate.AddDays(8),
                    UpdatedAt = null,
                    Views = 200,
                    RowVersion = Array.Empty<byte>()

                },

                // ===== MEN'S CLOTHING (CategoryId: 7, SellerId: 2 = Sara) =====
                new Product
                {
                    Id = 6,
                    Name = "Classic Leather Jacket",
                    Description = "Premium genuine leather jacket, perfect for any occasion",
                    Price = 299.99m,
                    DiscountedPrice = 249.99m,
                    StockQuantity = 40,
                    SoldQuantity = 15,
                    Condition = ProductCondition.New,
                    Location = "Alexandria, Egypt",
                    CategoryId = 7,     // Men's Clothing
                    SellerId = 2,       // Sara Fashion
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(6),
                    CreatedAt = baseDate.AddDays(2),
                    UpdatedAt = null,
                    Views = 180,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 7,
                    Name = "Vintage Denim Jeans",
                    Description = "Classic vintage-style denim jeans, comfortable and stylish",
                    Price = 89.99m,
                    DiscountedPrice = null,
                    StockQuantity = 60,
                    SoldQuantity = 25,
                    Condition = ProductCondition.Good,
                    Location = "Alexandria, Egypt",
                    CategoryId = 7,     // Men's Clothing
                    SellerId = 2,       // Sara Fashion
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(4),
                    CreatedAt = baseDate.AddDays(1),
                    UpdatedAt = null,
                    Views = 130,
                    RowVersion = Array.Empty<byte>()
                },

                // ===== WOMEN'S CLOTHING (CategoryId: 8, SellerId: 2 = Sara) =====
                new Product
                {
                    Id = 8,
                    Name = "Floral Summer Dress",
                    Description = "Beautiful floral print dress, perfect for summer",
                    Price = 79.99m,
                    DiscountedPrice = 59.99m,
                    StockQuantity = 50,
                    SoldQuantity = 20,
                    Condition = ProductCondition.New,
                    Location = "Alexandria, Egypt",
                    CategoryId = 8,     // Women's Clothing
                    SellerId = 2,       // Sara Fashion
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(5),
                    CreatedAt = baseDate.AddDays(2),
                    UpdatedAt = null,
                    Views = 160,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 9,
                    Name = "Elegant Evening Gown",
                    Description = "Stunning evening gown for special occasions",
                    Price = 199.99m,
                    DiscountedPrice = 179.99m,
                    StockQuantity = 25,
                    SoldQuantity = 8,
                    Condition = ProductCondition.New,
                    Location = "Cairo, Egypt",
                    CategoryId = 8,     // Women's Clothing
                    SellerId = 2,       // Sara Fashion
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(7),
                    CreatedAt = baseDate.AddDays(3),
                    UpdatedAt = null,
                    Views = 200,
                    RowVersion = Array.Empty<byte>()
                },

                // ===== BOOKS & MEDIA (CategoryId: 3, SellerId: 3 = Mohamed) =====
                new Product
                {
                    Id = 10,
                    Name = "The Midnight Library",
                    Description = "The New York Times bestselling novel about choices and regrets",
                    Price = 24.99m,
                    DiscountedPrice = 19.99m,
                    StockQuantity = 100,
                    SoldQuantity = 45,
                    Condition = ProductCondition.New,
                    Location = "Giza, Egypt",
                    CategoryId = 3,     // Books & Media
                    SellerId = 3,       // Mohamed BookLover
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(3),
                    CreatedAt = baseDate.AddDays(1),
                    UpdatedAt = null,
                    Views = 95,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 11,
                    Name = "Atomic Habits",
                    Description = "The best-selling guide to building good habits and breaking bad ones",
                    Price = 27.99m,
                    DiscountedPrice = 22.99m,
                    StockQuantity = 90,
                    SoldQuantity = 38,
                    Condition = ProductCondition.New,
                    Location = "Giza, Egypt",
                    CategoryId = 3,     // Books & Media
                    SellerId = 3,       // Mohamed BookLover
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(4),
                    CreatedAt = baseDate.AddDays(2),
                    UpdatedAt = null,
                    Views = 125,
                    RowVersion = Array.Empty<byte>()
                },

                // ===== HOME & GARDEN (CategoryId: 4, SellerId: 4 = Laila) =====
                new Product
                {
                    Id = 12,
                    Name = "Modern Coffee Table",
                    Description = "Elegant modern coffee table with glass top and wooden frame",
                    Price = 349.99m,
                    DiscountedPrice = 299.99m,
                    StockQuantity = 15,
                    SoldQuantity = 5,
                    Condition = ProductCondition.New,
                    Location = "Cairo, Egypt",
                    CategoryId = 4,     // Home & Garden
                    SellerId = 4,       // Laila Customer
                    IsActive = true,
                    IsFeatured = true,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(9),
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null,
                    Views = 150,
                    RowVersion = Array.Empty<byte>()
                },
                new Product
                {
                    Id = 13,
                    Name = "Indoor Plant Collection",
                    Description = "Set of 5 indoor plants with decorative pots",
                    Price = 89.99m,
                    DiscountedPrice = null,
                    StockQuantity = 30,
                    SoldQuantity = 12,
                    Condition = ProductCondition.New,
                    Location = "Alexandria, Egypt",
                    CategoryId = 4,     // Home & Garden
                    SellerId = 4,       // Laila Customer
                    IsActive = true,
                    IsFeatured = false,
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(8),
                    CreatedAt = baseDate.AddDays(4),
                    UpdatedAt = null,
                    Views = 110,
                    RowVersion = Array.Empty<byte>()
                }
               );
        }
        private void UserData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Seller",
                    Email = "john.seller@marketplace.com",
                    PhoneNumber = "+1234567890",
                    Bio = "Professional seller with 5 years experience",
                    Address = "123 Main Street",
                    City = "New York",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(30),  // Jan 31, 2024
                    UpdatedAt = null
                },
                new User
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Customer",
                    Email = "jane.customer@marketplace.com",
                    PhoneNumber = "+0987654321",
                    Bio = "Loves shopping for electronics and books",
                    Address = "456 Oak Avenue",
                    City = "Los Angeles",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(45),  // Feb 15, 2024
                    UpdatedAt = null
                },
                new User
                {
                    Id = 3,
                    FirstName = "Mike",
                    LastName = "TechSeller",
                    Email = "mike.tech@marketplace.com",
                    PhoneNumber = "+1122334455",
                    Bio = "Tech enthusiast selling gadgets",
                    Address = "789 Tech Park",
                    City = "San Francisco",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(60),  // Mar 1, 2024
                    UpdatedAt = null
                },
                new User
                {
                    Id = 4,
                    FirstName = "Sarah",
                    LastName = "FashionSeller",
                    Email = "sarah.fashion@marketplace.com",
                    PhoneNumber = "+5544332211",
                    Bio = "Fashion designer and vintage collector",
                    Address = "321 Fashion Street",
                    City = "Miami",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(75),  // Mar 16, 2024
                    UpdatedAt = null
                },
                new User
                {
                    Id = 5,
                    FirstName = "David",
                    LastName = "BookLover",
                    Email = "david.books@marketplace.com",
                    PhoneNumber = "+6677889900",
                    Bio = "Book collector and seller",
                    Address = "555 Library Lane",
                    City = "Chicago",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(90),  // Mar 31, 2024
                    UpdatedAt = null
                }
            );
        }

        private void ProductImageData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ProductImage>().HasData(
                // iPhone 15 Pro Max (ProductId: 1)
                new ProductImage
                {
                    Id = 1,
                    ProductId = 1,
                    ImageUrl = "/images/products/iphone15-main.jpg",
                    AltText = "iPhone 15 Pro Max - Front view",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 2450000,
                    FileName = "iphone15-main.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = 1,
                    ImageUrl = "/images/products/iphone15-back.jpg",
                    AltText = "iPhone 15 Pro Max - Back view",
                    IsPrimary = false,
                    DisplayOrder = 2,
                    FileSize = 2300000,
                    FileName = "iphone15-back.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null
                },

                // Samsung Galaxy S24 Ultra (ProductId: 2)
                new ProductImage
                {
                    Id = 3,
                    ProductId = 2,
                    ImageUrl = "/images/products/samsung-main.jpg",
                    AltText = "Samsung Galaxy S24 Ultra - Main view",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 2600000,
                    FileName = "samsung-main.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(3),
                    UpdatedAt = null
                },

                // MacBook Pro (ProductId: 4)
                new ProductImage
                {
                    Id = 4,
                    ProductId = 4,
                    ImageUrl = "/images/products/macbook-main.jpg",
                    AltText = "MacBook Pro 16-inch - Front view",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 3200000,
                    FileName = "macbook-main.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(10),
                    UpdatedAt = null
                },
                new ProductImage
                {
                    Id = 5,
                    ProductId = 4,
                    ImageUrl = "/images/products/macbook-keyboard.jpg",
                    AltText = "MacBook Pro 16-inch - Keyboard",
                    IsPrimary = false,
                    DisplayOrder = 2,
                    FileSize = 2800000,
                    FileName = "macbook-keyboard.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(10),
                    UpdatedAt = null
                },

                // Leather Jacket (ProductId: 6)
                new ProductImage
                {
                    Id = 6,
                    ProductId = 6,
                    ImageUrl = "/images/products/jacket-main.jpg",
                    AltText = "Classic Leather Jacket - Front",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 1900000,
                    FileName = "jacket-main.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(2),
                    UpdatedAt = null
                },

                // The Midnight Library (ProductId: 10)
                new ProductImage
                {
                    Id = 7,
                    ProductId = 10,
                    ImageUrl = "/images/products/midnight-library.jpg",
                    AltText = "The Midnight Library book cover",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 1500000,
                    FileName = "midnight-library.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(1),
                    UpdatedAt = null
                },

                // Modern Coffee Table (ProductId: 12)
                new ProductImage
                {
                    Id = 8,
                    ProductId = 12,
                    ImageUrl = "/images/products/coffee-table.jpg",
                    AltText = "Modern Coffee Table",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    FileSize = 2200000,
                    FileName = "coffee-table.jpg",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null
                }
            );
        }

        private void ReviewsData(ModelBuilder modelBuilder)
        {
            var baseDate = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Review>().HasData(
                // iPhone 15 Pro Max Reviews (ProductId: 1)
                new Review
                {
                    Id = 1,
                    ProductId = 1,
                    UserId = 4,      // Laila Customer
                    Rating = 5,
                    Comment = "Amazing phone! The camera is incredible and the battery life is outstanding.",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(12),
                    CreatedAt = baseDate.AddDays(10),
                    UpdatedAt = null,
                    IsActive = true
                },
                new Review
                {
                    Id = 2,
                    ProductId = 1,
                    UserId = 3,      // Mohamed BookLover
                    Rating = 4,
                    Comment = "Great phone but very expensive. The performance is top notch though.",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(13),
                    CreatedAt = baseDate.AddDays(11),
                    UpdatedAt = null,
                    IsActive = true
                },

                // Samsung Galaxy S24 Ultra Review (ProductId: 2)
                new Review
                {
                    Id = 3,
                    ProductId = 2,
                    UserId = 4,      // Laila Customer
                    Rating = 5,
                    Comment = "Excellent phone with amazing camera features!",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(10),
                    CreatedAt = baseDate.AddDays(8),
                    UpdatedAt = null,
                    IsActive = true
                },

                // MacBook Pro Review (ProductId: 4)
                new Review
                {
                    Id = 4,
                    ProductId = 4,
                    UserId = 4,      // Laila Customer
                    Rating = 5,
                    Comment = "Best laptop I've ever owned. The M3 Max chip is a beast!",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(15),
                    CreatedAt = baseDate.AddDays(12),
                    UpdatedAt = null,
                    IsActive = true
                },

                // Leather Jacket Reviews (ProductId: 6)
                new Review
                {
                    Id = 5,
                    ProductId = 6,
                    UserId = 4,      // Laila Customer
                    Rating = 5,
                    Comment = "Beautiful jacket, great quality leather. Fits perfectly!",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(7),
                    CreatedAt = baseDate.AddDays(5),
                    UpdatedAt = null,
                    IsActive = true
                },
                new Review
                {
                    Id = 6,
                    ProductId = 6,
                    UserId = 3,      // Mohamed BookLover
                    Rating = 4,
                    Comment = "Nice jacket but a bit expensive. Good quality though.",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(8),
                    CreatedAt = baseDate.AddDays(6),
                    UpdatedAt = null,
                    IsActive = true
                },

                // The Midnight Library Review (ProductId: 10)
                new Review
                {
                    Id = 7,
                    ProductId = 10,
                    UserId = 4,      // Laila Customer
                    Rating = 5,
                    Comment = "Life-changing book! Couldn't put it down.",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(4),
                    CreatedAt = baseDate.AddDays(2),
                    UpdatedAt = null,
                    IsActive = true
                },

                // Modern Coffee Table Review (ProductId: 12)
                new Review
                {
                    Id = 8,
                    ProductId = 12,
                    UserId = 3,      // Mohamed BookLover
                    Rating = 4,
                    Comment = "Great quality table, looks amazing in my living room!",
                    IsApproved = true,
                    ApprovedAt = baseDate.AddDays(10),
                    CreatedAt = baseDate.AddDays(8),
                    UpdatedAt = null,
                    IsActive = true
                },

                // Pending Review (not approved yet)
                new Review
                {
                    Id = 9,
                    ProductId = 1,   // iPhone 15 Pro Max
                    UserId = 2,      // Sara Fashion
                    Rating = 3,
                    Comment = "Good phone but not worth the price difference from previous model.",
                    IsApproved = false,
                    ApprovedAt = null,
                    CreatedAt = baseDate.AddDays(9),
                    UpdatedAt = null,
                    IsActive = true
                }
            );
        }

        private void SeedRoles(ModelBuilder builder)
        {
            
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1", // Admin Role
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "ad5c25b6-d186-4b30-b03a-f77ccf2dee04"
                },
                new IdentityRole
                {
                    Id = "2", // Seller Role
                    Name = "Seller",
                    NormalizedName = "SELLER",
                    ConcurrencyStamp = "a7500aa8-7270-4703-939a-3d7675ed7ee0"
                },
                new IdentityRole
                {
                    Id = "3", // Customer Role
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = "2baf1f52-48cc-462b-a100-916e8adf3757"
                }
            );
        }


    }
}
