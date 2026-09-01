# 🛒 Marketplace API

A backend RESTful API for an online marketplace, built with **ASP.NET Core 9** and designed using a layered architecture approach.

The application provides the core functionality required for a modern marketplace, including product management, authentication, shopping cart, orders, reviews, wishlist, user profiles, image uploads, email notifications, and administrative features.

## 🚀 Features

### 🔐 Authentication & Authorization

* User registration and login
* JWT-based authentication
* Role-based authorization
* ASP.NET Core Identity
* Admin functionality
* User profile management

### 📦 Product Management

* Create, update, and delete products
* Product categories
* Product images
* Product search
* Filtering by category, price, and condition
* Featured products
* Stock management
* Product views
* Soft delete
* Product approval
* Sold quantity tracking

### 🛒 Shopping

* Shopping cart
* Add/remove cart items
* Update item quantities
* Wishlist / favorites
* Product availability checking

### 📋 Orders

* Create orders
* Retrieve user orders
* Retrieve order details
* Order status management
* Stock updates
* Order confirmation emails

### ⭐ Reviews

* Add product reviews
* Product ratings
* Average rating calculation
* Review management

### 👤 User Management

* User profiles
* Seller information
* Admin management
* User statistics

### 📊 Administration

* Product management
* Category management
* User management
* Marketplace statistics
* Product approval

### 📧 Email

* Order confirmation emails
* SMTP-based email service

### 📝 Logging

* Structured application logging using Serilog
* Console logging
* File logging
* Error logging

---

## 🏗️ Architecture

The project follows a layered architecture that separates business logic, domain entities, infrastructure concerns, and API responsibilities.

```text
Marketplace
│
├── Marketplace.Api
│   ├── Controllers
│   ├── Services
│   ├── Interfaces
│   ├── Program.cs
│   └── Configuration
│
├── Marketplace.Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Mappings
│
├── Marketplace.Domain
│   ├── Entities
│   ├── Enums
│   └── Interfaces
│
└── Marketplace.Infrastructure
    ├── Data
    ├── Repositories
    ├── Models
    └── Migrations
```

### Layer Responsibilities

**Marketplace.Api**

Responsible for HTTP requests, controllers, authentication configuration, Swagger/OpenAPI, dependency injection, and API-specific services.

**Marketplace.Application**

Contains application business logic, DTOs, service interfaces, mappings, and application-level operations.

**Marketplace.Domain**

Contains core domain entities, enums, and repository abstractions.

**Marketplace.Infrastructure**

Contains Entity Framework Core, SQL Server database configuration, repositories, migrations, and persistence-related implementations.

---

## 🛠️ Technologies

| Technology              | Purpose                 |
| ----------------------- | ----------------------- |
| C#                      | Programming language    |
| ASP.NET Core 9          | Web API framework       |
| Entity Framework Core 9 | ORM                     |
| SQL Server              | Database                |
| ASP.NET Core Identity   | User management         |
| JWT                     | Authentication          |
| AutoMapper              | Object mapping          |
| Serilog                 | Logging                 |
| Swagger / OpenAPI       | API documentation       |
| Repository Pattern      | Data access abstraction |
| Dependency Injection    | Dependency management   |
| REST API                | API architecture        |

---

## 📂 Main Entities

The application includes the following domain entities:

* User
* Product
* Category
* ProductImage
* ShoppingCart
* CartItem
* Order
* OrderItem
* Review
* Favorite

---

## 🔑 API Controllers

The API currently includes controllers for:

* Account
* Products
* Categories
* Orders
* Cart
* Reviews
* Wishlist
* Profile
* Admin

---

## 🗄️ Database

The project uses:

* **SQL Server**
* **Entity Framework Core**
* **EF Core Migrations**
* **ASP.NET Core Identity**

Database migrations are automatically applied when the application starts in the current configuration.

---

## ⚙️ Getting Started

### Prerequisites

Make sure you have installed:

* [.NET 9 SDK](https://dotnet.microsoft.com/)
* SQL Server
* Visual Studio 2022 or another compatible IDE

### 1. Clone the repository

```bash
git clone https://github.com/Ma7mudramdan/Marketplace.git
cd Marketplace
```

### 2. Configure the database

Update the connection string in:

```text
Marketplace.Api/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MarketplaceDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Configure JWT

Add your JWT configuration:

```json
{
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "Marketplace",
    "Audience": "MarketplaceUsers"
  }
}
```

> For production environments, sensitive configuration values should be stored using environment variables, user secrets, or a secure secret-management solution.

### 4. Configure Email

Configure the SMTP settings required by the email service in your application configuration.

### 5. Run the application

```bash
dotnet restore
dotnet build
dotnet run --project Marketplace.Api
```

### 6. Open Swagger

After running the application, open the Swagger UI URL shown by ASP.NET Core.

Swagger provides interactive documentation and allows you to test the API endpoints.

---

## 🔒 Authentication

The API uses **JWT Bearer Authentication**.

After logging in:

```text
Authorization: Bearer <your-token>
```

Use the generated JWT token to access protected endpoints.

Swagger is configured to support Bearer token authentication.

---

## 🔄 Example Request Flow

A typical product request follows this flow:

```text
HTTP Request
     │
     ▼
ProductsController
     │
     ▼
IProductService
     │
     ▼
ProductService
     │
     ▼
IProductRepository
     │
     ▼
ProductRepository
     │
     ▼
Entity Framework Core
     │
     ▼
SQL Server
```

This separation keeps the API layer independent from the database implementation and makes the application easier to maintain and extend.

---

## 🧪 Testing

Testing is planned as a future improvement and can include:

* Unit tests for application services
* Repository integration tests
* API integration tests
* Authentication and authorization tests
* Product and order workflow tests

---

## 🚧 Future Improvements

Possible future improvements include:

* Add comprehensive unit and integration tests
* Add FluentValidation
* Add global exception handling middleware
* Improve API response consistency
* Add API versioning
* Add refresh tokens
* Improve pagination and filtering
* Add caching
* Add Docker support
* Add CI/CD with GitHub Actions
* Add production deployment
* Add automated API tests

---

## 🎯 Project Goals

This project was built to practice and demonstrate practical backend development concepts using the .NET ecosystem, including:

* Object-Oriented Programming
* RESTful API development
* Layered architecture
* Dependency Injection
* Repository Pattern
* Entity Framework Core
* LINQ
* Authentication and Authorization
* Database design
* Transactions
* Logging
* DTOs and mapping
* Asynchronous programming
* API documentation

---

## 👨‍💻 Author

**Mahmoud Ramadan**

Backend .NET Developer / Student

GitHub: [Ma7mudramdan](https://github.com/Ma7mudramdan)

---

## 📄 License

This project is for educational and portfolio purposes.
