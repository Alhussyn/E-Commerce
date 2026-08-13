# E-Commerce Store

A professional, full-featured e-commerce web application built with **ASP.NET Core 9 MVC**, **Entity Framework Core**, and **SQL Server**.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap)
![JavaScript](https://img.shields.io/badge/JavaScript-ES6-F7DF1E?style=for-the-badge&logo=javascript)

---

## Features

### Customer Experience
- **Product Catalog** — 25 products across 5 categories with professional imagery
- **Product Details** — Image gallery with 3 views per product, ratings, reviews
- **Search & Filter** — Search by name, brand, or category with debounced input
- **Shopping Cart** — AJAX-powered cart with quantity updates and real-time feedback
- **Checkout** — Secure checkout with server-side price and stock validation
- **Order History** — Track past orders with status updates
- **Wishlist** — Save products for later with heart toggle

### Admin Dashboard
- **Product Management** — Full CRUD for products with image URL support
- **Category Management** — Create, edit, and organize product categories
- **Order Management** — View all orders, update status (Pending → Processing → Shipped → Delivered)
- **Analytics Dashboard** — Total products, orders, revenue, customers, pending orders, low stock alerts

### Security & Reliability
- Session-based authentication with role authorization (User / Manager)
- Anti-forgery token protection on all POST actions
- Server-side price calculation (no client-side trust)
- Race-condition-safe checkout with serializable transactions
- Image fallback system for broken external URLs

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 9.0 MVC |
| **Language** | C# 12 |
| **ORM** | Entity Framework Core 9.0 |
| **Database** | SQL Server |
| **Frontend** | Razor Views, Bootstrap 5, Vanilla JavaScript |
| **Email** | MailKit (SMTP) |
| **Version Control** | Git / GitHub |

---

## Project Structure

```
E-Commerce/
├── Controllers/          # MVC Controllers
│   ├── AuthController        # Authentication & OTP
│   ├── ProductController     # Storefront, cart, checkout
│   ├── ManageController      # Admin dashboard
│   ├── OrderController       # User order history
│   ├── ProfileController     # User profile
│   ├── CategoryController    # Category management
│   └── CRUDController        # Legacy product CRUD (secured)
├── Models/               # Entity models
├── ViewModels/           # Checkout & product form VMs
├── Data/                 # DbContext & seed data
├── Services/             # Email service (SMTP)
├── Views/                # Razor views per controller
├── Migrations/           # EF Core migrations
├── wwwroot/
│   ├── css/              # Custom stylesheets
│   ├── js/               # JavaScript (AJAX, fallbacks)
│   └── images/           # Product images (local + fallback)
└── Program.cs            # App configuration & seed
```

---

## Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB, Express, or full)

### Setup

```bash
# Clone the repository
git clone https://github.com/Alhussyn/E-Commerce.git
cd E-Commerce

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The application will:
1. Apply database migrations automatically
2. Seed 25 products with 75 gallery images on first run
3. Start at `http://localhost:5232`

### Default Accounts

| Role | Email | Password |
|---|---|---|
| **Admin** | admin@store.com | Admin@123456 |
| **Customer** | testuser@example.com | Test123456 |

---

## Screenshots

> **Storefront** — Clean product grid with category filtering
> **Product Details** — Image gallery, reviews, add to cart
> **Admin Dashboard** — Product management, order tracking
> **Cart & Checkout** — AJAX-powered cart, secure checkout

---

## Database Schema

- **Users** — Authentication, roles, OTP support
- **Products** — Name, price, discount, stock, brand, SKU
- **Categories** — Product organization
- **ProductImages** — Multi-image gallery per product
- **Carts / CartItems** — Per-user shopping cart
- **Orders / OrderItems** — Order history with shipping info
- **Wishlists** — Saved products per user
- **Reviews** — Star ratings and comments per product

---

## License

This project is for educational purposes.

---

## Author

**Alhussyn** — [GitHub](https://github.com/Alhussyn)
