using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Marketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedDateForeachEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Bio", "City", "Country", "CreatedAt", "DateOfBirth", "Email", "FirstName", "IsActive", "IsEmailConfirmed", "LastLoginAt", "LastName", "PhoneNumber", "ProfilePictureUrl", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Main Street", "Professional seller with 5 years experience", "New York", "USA", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, "john.seller@marketplace.com", "John", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seller", "+1234567890", null, null },
                    { 2, "456 Oak Avenue", "Loves shopping for electronics and books", "Los Angeles", "USA", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "jane.customer@marketplace.com", "Jane", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer", "+0987654321", null, null },
                    { 3, "789 Tech Park", "Tech enthusiast selling gadgets", "San Francisco", "USA", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "mike.tech@marketplace.com", "Mike", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TechSeller", "+1122334455", null, null },
                    { 4, "321 Fashion Street", "Fashion designer and vintage collector", "Miami", "USA", new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "sarah.fashion@marketplace.com", "Sarah", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "FashionSeller", "+5544332211", null, null },
                    { 5, "555 Library Lane", "Book collector and seller", "Chicago", "USA", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "david.books@marketplace.com", "David", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BookLover", "+6677889900", null, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ApprovedAt", "CategoryId", "Condition", "CreatedAt", "Description", "DiscountedPrice", "ExpiresAt", "IsActive", "IsApproved", "IsFeatured", "Location", "Name", "Price", "SellerId", "SoldQuantity", "StockQuantity", "UpdatedAt", "Views" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), 5, 0, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Latest iPhone with titanium body, A17 Pro chip, 48MP camera", 1099.99m, null, true, true, true, "Cairo, Egypt", "iPhone 15 Pro Max", 1199.99m, 1, 10, 25, null, 350 },
                    { 2, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), 5, 0, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Samsung's flagship with AI features, 200MP camera, S Pen", 1099.99m, null, true, true, true, "Cairo, Egypt", "Samsung Galaxy S24 Ultra", 1199.99m, 1, 8, 30, null, 280 },
                    { 3, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), 5, 0, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Pure Android experience with advanced AI camera features", null, null, true, true, false, "Alexandria, Egypt", "Google Pixel 8 Pro", 999.99m, 5, 5, 20, null, 150 },
                    { 4, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), 6, 0, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Ultimate performance with M3 Max chip, 36GB RAM, 1TB SSD", 3299.99m, null, true, true, true, "Cairo, Egypt", "MacBook Pro 16-inch M3 Max", 3499.99m, 1, 4, 15, null, 420 },
                    { 5, new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), 6, 0, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Premium Windows laptop with 4K OLED display, i9 processor", 2299.99m, null, true, true, false, "Alexandria, Egypt", "Dell XPS 15", 2499.99m, 5, 3, 10, null, 200 },
                    { 6, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), 7, 0, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Premium genuine leather jacket, perfect for any occasion", 249.99m, null, true, true, true, "Alexandria, Egypt", "Classic Leather Jacket", 299.99m, 2, 15, 40, null, 180 },
                    { 7, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Classic vintage-style denim jeans, comfortable and stylish", null, null, true, true, false, "Alexandria, Egypt", "Vintage Denim Jeans", 89.99m, 2, 25, 60, null, 130 },
                    { 8, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 8, 0, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Beautiful floral print dress, perfect for summer", 59.99m, null, true, true, false, "Alexandria, Egypt", "Floral Summer Dress", 79.99m, 2, 20, 50, null, 160 },
                    { 9, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 8, 0, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Stunning evening gown for special occasions", 179.99m, null, true, true, true, "Cairo, Egypt", "Elegant Evening Gown", 199.99m, 2, 8, 25, null, 200 },
                    { 10, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), 3, 0, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "The New York Times bestselling novel about choices and regrets", 19.99m, null, true, true, true, "Giza, Egypt", "The Midnight Library", 24.99m, 3, 45, 100, null, 95 },
                    { 11, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 3, 0, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "The best-selling guide to building good habits and breaking bad ones", 22.99m, null, true, true, false, "Giza, Egypt", "Atomic Habits", 27.99m, 3, 38, 90, null, 125 },
                    { 12, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), 4, 0, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Elegant modern coffee table with glass top and wooden frame", 299.99m, null, true, true, true, "Cairo, Egypt", "Modern Coffee Table", 349.99m, 4, 5, 15, null, 150 },
                    { 13, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), 4, 0, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Set of 5 indoor plants with decorative pots", null, null, true, true, false, "Alexandria, Egypt", "Indoor Plant Collection", 89.99m, 4, 12, 30, null, 110 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "AltText", "CreatedAt", "DisplayOrder", "FileName", "FileSize", "ImageUrl", "IsActive", "IsPrimary", "ProductId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "iPhone 15 Pro Max - Front view", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, "iphone15-main.jpg", 2450000L, "/images/products/iphone15-main.jpg", true, true, 1, null },
                    { 2, "iPhone 15 Pro Max - Back view", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2, "iphone15-back.jpg", 2300000L, "/images/products/iphone15-back.jpg", true, false, 1, null },
                    { 3, "Samsung Galaxy S24 Ultra - Main view", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, "samsung-main.jpg", 2600000L, "/images/products/samsung-main.jpg", true, true, 2, null },
                    { 4, "MacBook Pro 16-inch - Front view", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, "macbook-main.jpg", 3200000L, "/images/products/macbook-main.jpg", true, true, 4, null },
                    { 5, "MacBook Pro 16-inch - Keyboard", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2, "macbook-keyboard.jpg", 2800000L, "/images/products/macbook-keyboard.jpg", true, false, 4, null },
                    { 6, "Classic Leather Jacket - Front", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 1, "jacket-main.jpg", 1900000L, "/images/products/jacket-main.jpg", true, true, 6, null },
                    { 7, "The Midnight Library book cover", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, "midnight-library.jpg", 1500000L, "/images/products/midnight-library.jpg", true, true, 10, null },
                    { 8, "Modern Coffee Table", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, "coffee-table.jpg", 2200000L, "/images/products/coffee-table.jpg", true, true, 12, null }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "ApprovedAt", "Comment", "CreatedAt", "IsActive", "IsApproved", "ProductId", "Rating", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Amazing phone! The camera is incredible and the battery life is outstanding.", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 1, 5, null, 4 },
                    { 2, new DateTime(2026, 8, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Great phone but very expensive. The performance is top notch though.", new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 1, 4, null, 3 },
                    { 3, new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Excellent phone with amazing camera features!", new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 2, 5, null, 4 },
                    { 4, new DateTime(2026, 8, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Best laptop I've ever owned. The M3 Max chip is a beast!", new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 4, 5, null, 4 },
                    { 5, new DateTime(2026, 8, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Beautiful jacket, great quality leather. Fits perfectly!", new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 6, 5, null, 4 },
                    { 6, new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Nice jacket but a bit expensive. Good quality though.", new DateTime(2026, 8, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 6, 4, null, 3 },
                    { 7, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Life-changing book! Couldn't put it down.", new DateTime(2026, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 10, 5, null, 4 },
                    { 8, new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Great quality table, looks amazing in my living room!", new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 12, 4, null, 3 },
                    { 9, null, "Good phone but not worth the price difference from previous model.", new DateTime(2026, 8, 19, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 1, 3, null, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
