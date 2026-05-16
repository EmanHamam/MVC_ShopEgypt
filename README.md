


# ShopEgypt

ShopEgypt is a full-stack e-commerce web application built with **ASP.NET Core MVC** as a team project (ITI — IDA track). The storefront lets customers browse products, manage a cart and wishlist, place orders, and pay through Stripe. A separate admin panel covers catalog management, orders, customers, and store settings.

The app runs on **.NET 10** and uses **SQL Server** with Entity Framework Core. Authentication is handled by **ASP.NET Core Identity** (email/password plus optional Google and Facebook login). Product images are stored on **Cloudinary**; transactional email goes through **SendGrid**.

---

## Screenshots (for review)



https://github.com/user-attachments/assets/7c75abaf-1355-4a93-99d2-9c9a299bd790



Add your screenshots under `docs/images/` as needed.

### Overview

| | |
|---|---|
| **Home / shop entry** | Replace with your screenshot |
|  | Landing or redirect to catalog (`/products`) |

### Storefront

- Product catalog (filters, paging)
- Product details & reviews
- Shopping cart
- Checkout — address
- Checkout — summary
- Stripe payment
- Order confirmation
- My Orders
- Wishlist

### Authentication

- Login
- Register
- Email confirmation (optional)

### Admin panel (`/Adminn`)

- Dashboard
- Products list
- Product create / edit
- Orders
- Customers
- Categories
- Reviews
- Settings

### Architecture (optional)

- Solution / layer diagram
- Database ER (optional)

> **Tip:** Run the app at `http://localhost:5224`, capture full-width browser shots, and keep a checklist in `docs/images/README.md`.

---

## What’s in the repo

The solution is split into four projects so the web layer stays thin and business logic lives in services:

| Project | Role |
|---------|------|
| **ShopEgypt** | MVC web app — controllers, Razor views, Identity UI, admin area |
| **ShopEgypt.Application** | Interfaces, DTOs, session helpers, Mapster mapping config |
| **ShopEgypt.Infrastructure** | EF Core `ApplicationDbContext`, repositories, service implementations, Stripe/SendGrid/Cloudinary integrations |
| **ShopEgypt.Domain** | Entities, enums (`OrderStatus`, `UserRole`, product sort options, etc.) |

**ShopEgypt** references Application and Infrastructure. Application references Domain only. Infrastructure references Application and Domain.

---

## Main features

### Storefront (customer)

- Product catalog at `/products` with paging, category filter, keyword search, price range, and sorting
- Product detail pages with reviews
- Shopping cart (session-based for guests; merged into the user account on login)
- Wishlist (authenticated users)
- Checkout flow: address → order summary → Stripe payment → confirmation
- **My Orders** at `/Orders/MyOrders` (order history and detail, cancel/pay pending where allowed)

### Admin panel (`/Adminn/...`)

Restricted to users in the **Admin** role:

| Section | URL (example) |
|---------|----------------|
| Dashboard | `/Adminn/Dashboard/Index` |
| Products | `/Adminn/Products/Index` |
| Orders | `/Adminn/AdminOrders/Index` |
| Customers | `/Adminn/Customers/Index` |
| Categories | `/Adminn/Categories/Index` |
| Reviews | `/Adminn/Reviews/Index` |
| Settings | `/Adminn/Settings/Index` |

The admin area folder is named `Adminn` (not a typo in this README — that’s the actual area name in code).

### Authentication & roles

- **Customer** — default role on registration (`UserRole.Customer`)
- **Admin** — full access to the `Adminn` area
- Email confirmation is required before sign-in
- Login redirects: admins → dashboard; customers → shop (`/products`) or a safe `returnUrl` (admin URLs are never used for customers after login)
- Cookie paths: login `/Identity/Account/Login`, access denied `/Identity/Account/AccessDenied`

External providers (Google, Facebook) are wired in `ServiceRegistration` when credentials exist in configuration.

---

## Architecture notes

- **Unit of Work** (`IUnitOfWork`) exposes generic repositories and domain services used across the app.
- **Services** (cart, product, order, review, wishlist, address, customer, Stripe, etc.) implement interfaces from Application and are registered in `Infrastructure.ServiceRegistration`.
- **Mapster** maps entities to DTOs (`MapsterConfig.RegisterMappings()` in `Program.cs`).
- **Session** backs the guest cart (30-minute idle timeout); `ICartService.MergeSessionCartToUserCartAsync()` runs after a successful customer login.

### Routing: shop vs admin

Shop and admin both deal with “orders” and “products”, so controller names were split to avoid MVC generating wrong URLs (e.g. `/Adminn/Orders/MyOrders` for a customer):

| Concern | Controller | Typical URL |
|---------|------------|-------------|
| Customer orders | `ShopOrdersController` | `/Orders/MyOrders`, `/Orders/Detail/{id}`, … |
| Admin order list | `AdminOrdersController` | `/Adminn/AdminOrders/Index` |
| Customer catalog | `ProductsController` | `/products`, `/products/details/{id}` |
| Admin catalog | `ProductsController` (in area `Adminn`) | `/Adminn/Products/Index` |

Area routes are registered **before** the default route in `Program.cs` so admin URLs resolve correctly; login uses explicit paths (`AuthRedirectHelper`) instead of fragile `RedirectToAction` with mixed areas.

---

## Checkout flow (customer)

1. **Cart** — add/update/remove items (`CartController`)
2. **Address** — `ShopOrdersController.Address` (GET/POST); pending order data held in `TempData` until payment
3. **View summary** — review line items and totals
4. **Confirm** — order persisted, Stripe PaymentIntent created
5. **Payment** — Stripe Elements on the client; success posts to `PaymentSuccess`
6. **Order confirmation** — cart cleared, order status updated

Order states follow `OrderStatus` (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled).

---

## Domain model (overview)

Core entities include `Product`, `ProductImage`, `Category`, `Brand`, `Cart`, `CartItem`, `Order`, `OrderItem`, `Address`, `Payment`, `Review`, `Wishlist`, `WishlistItem`, and `ApplicationUser` (extends Identity user with profile fields).

Migrations live under `ShopEgypt.Infrastructure/Migrations/`.

---

## Configuration

`appsettings.json` and environment-specific files are **gitignored**. Use **User Secrets** (local) or environment variables / `appsettings.Secrets.json` (CI) for:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<SQL Server connection string>"
  },
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  },
  "SendGrid": {
    "ApiKey": "",
    "FromEmail": "",
    "FromName": "ShopEgypt"
  },
  "Authentication": {
    "Google": { "ClientId": "", "ClientSecret": "" },
    "Facebook": { "AppId": "", "AppSecret": "" }
  },
  "Stripe": {
    "SecretKey": "",
    "PublishableKey": ""
  }
}
```

Google/Facebook blocks are optional if you only use email login. Stripe keys are required for the payment step.

**User Secrets (Visual Studio / CLI):**

```bash
cd ShopEgypt
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=ShopEgypt;..."
```

Repeat for other keys under the same section names as above.

---

## Run locally

**Prerequisites:** .NET 10 SDK, SQL Server (LocalDB or full instance), Node not required (static assets under `wwwroot`).

```bash
git clone <repo-url>
cd MVC_ShopEgypt
dotnet restore
```

Apply the database (from repo root):

```bash
dotnet ef database update --project ShopEgypt.Infrastructure --startup-project ShopEgypt
```

Run the web project:

```bash
dotnet run --project ShopEgypt
```

Default HTTP URL: **http://localhost:5224** (see `ShopEgypt/Properties/launchSettings.json`).

Open `/products` for the shop. Sign in at `/Identity/Account/Register` or `/Identity/Account/Login`. Seed an admin user in the database (or uncomment/adjust `ApplicationDbContextSeeder`) and assign the **Admin** role to access `/Adminn/Dashboard/Index`.

---

## Project layout (web app)

```
ShopEgypt/
├── Areas/
│   ├── Adminn/          # Admin MVC area (controllers + views)
│   └── Identity/        # Razor Pages (login, register, etc.)
├── Controllers/         # Storefront controllers (Products, Cart, ShopOrders, …)
├── Views/               # Storefront Razor views
├── ViewComponents/      # Header (cart/wishlist counts, nav)
├── wwwroot/             # CSS, JS, images
└── Program.cs           # Pipeline, Identity, routing

ShopEgypt.Application/   # Interfaces, DTOs
ShopEgypt.Infrastructure/ # DbContext, services, migrations, external APIs
ShopEgypt.Domain/        # Entities and enums
```

---

## CI

GitHub Actions workflow (`.github/workflows/dotnet.yml`) restores, builds, and publishes on pushes to `main` / `develop`. It expects repository secrets for the database, Cloudinary, SendGrid, and OAuth providers. Note: the workflow file currently targets **.NET 8** in `DOTNET_VERSION` while the projects target **net10.0** — align these if the pipeline fails on SDK version.

---

## Publishing

Publish profiles exist under `ShopEgypt/Properties/PublishProfiles/` (FTP / Web Deploy). Configure connection strings and secrets on the host the same way as local development.

---

## Team / context

Built as a coursework team project demonstrating layered MVC, Identity, areas, EF Core, and third-party integrations (payments, email, image CDN). If you extend the repo, keep shop controllers out of the `Adminn` area name collisions (prefer explicit routes or distinct controller names like `ShopOrders` / `AdminOrders`).

For questions about deployment or credentials, check with whoever manages the team’s secret store — nothing sensitive belongs in git.
