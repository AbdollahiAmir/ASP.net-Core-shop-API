<div align="center">

# 🛍️ Online Shop API

### RESTful backend for a role-based, multi-seller online store

**یک API بک‌اند برای فروشگاه آنلاین چندفروشنده‌ای، با مدیریت نقش‌محور کاربران، محصولات، سبد خرید و سفارش‌ها**

<br />

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?logo= dotnet&logoColor=white)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![License](https://img.shields.io/badge/License-Not_specified-lightgrey)

[✨ ویژگی‌ها](#-ویژگیها--features) · [🚀 شروع سریع](#-شروع-سریع--quick-start) · [📚 API](#-مرور-api--api-overview) · [🧱 ساختار](#-ساختار-پروژه--project-structure)

</div>

---

## 🌟 درباره پروژه | About

<div dir="rtl">

**Online Shop API** یک سرویس RESTful برای هستهٔ بک‌اند فروشگاه آنلاین است که با ASP.NET Core 10 ساخته شده. ساختار نقش‌محور آن سه گروه مدیر، فروشنده و مشتری را پشتیبانی می‌کند و فرایندهای اصلی فروشگاه را از مدیریت کالا تا ثبت و بررسی سفارش پوشش می‌دهد.

</div>

**Online Shop API** is a RESTful backend for an online store, built with ASP.NET Core 10. Its role-based design supports administrators, sellers, and customers, covering core commerce workflows from catalog management to order placement and review.

## ✨ ویژگی‌ها | Features

| فارسی | English |
|---|---|
| 🔐 ثبت‌نام، ورود، خروج و تغییر رمز عبور با ASP.NET Core Identity و JWT | 🔐 Registration, login, logout, and password changes with ASP.NET Core Identity and JWT |
| 👥 دسترسی مبتنی بر نقش‌های **Admin**، **Seller** و **Customer** | 👥 Role-based access for **Admin**, **Seller**, and **Customer** |
| 📦 مدیریت محصولات، بارگذاری تصویر و تأیید محصول توسط مدیر | 📦 Product management, image uploads, and admin approval |
| 🗂️ مشاهده و مدیریت دسته‌بندی‌های محصولات | 🗂️ Browse and manage product categories |
| 🛒 سبد خرید و ثبت و پیگیری سفارش مشتری | 🛒 Customer carts, order placement, and tracking |
| 🤝 گردش سفارش چندفروشنده‌ای با بررسی فروشنده و مدیر | 🤝 Multi-seller order workflow with seller and admin review |
| 🗄️ ذخیره‌سازی با SQL Server و Entity Framework Core | 🗄️ SQL Server persistence with Entity Framework Core |
| 📖 مستندات OpenAPI و رابط Scalar در محیط توسعه | 📖 OpenAPI documentation and Scalar UI in Development |

## 🧰 فناوری‌ها | Tech stack

- **C# / .NET 10** — `net10.0`
- **ASP.NET Core Web API**
- **Entity Framework Core 10** و **SQL Server**
- **ASP.NET Core Identity** و **JWT Bearer**
- **OpenAPI** و **Scalar.AspNetCore**

## 🚀 شروع سریع | Quick start

### پیش‌نیازها | Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server یا SQL Server LocalDB
- ابزار EF Core CLI (`dotnet-ef`)

نصب ابزار EF Core در صورت نیاز:

Install the EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

### ۱. کلون و ورود به پوشه | Clone the repository

```bash
git clone https://github.com/AbdollahiAmir/ASP.net-Core-shop-API.git
cd ASP.net-Core-shop-API
```

### ۲. تنظیم اسرار محلی | Configure local secrets

<div dir="rtl">

برنامه در زمان اجرا نقش‌ها و حساب مدیر اولیه را Seed می‌کند. برای شروع، کلید JWT و مشخصات مدیر را با User Secrets تنظیم کنید. این اطلاعات محرمانه را در Git یا فایل‌های تنظیمات commit نکنید.

</div>

At startup, the app seeds the roles and initial administrator account. Configure the JWT signing key and admin credentials with User Secrets. Do not commit secrets to Git or configuration files.

```bash
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret-key" --project BackEnd
dotnet user-secrets set "Seed:AdminUserName" "admin" --project BackEnd
dotnet user-secrets set "Seed:AdminPassword" "replace-with-a-strong-password" --project BackEnd
```

رمز مدیر باید حداقل ۸ نویسه و شامل حرف کوچک، حرف بزرگ، عدد و نویسهٔ غیرحرفی باشد.

The admin password must be at least 8 characters and contain lowercase and uppercase letters, a digit, and a non-alphanumeric character.

### ۳. اتصال پایگاه‌داده | Configure the database

اتصال پیش‌فرض در `BackEnd/appsettings.json` از SQL Server LocalDB استفاده می‌کند. برای اتصال به SQL Server دیگر، مقدار اتصال را با User Secrets جایگزین کنید:

The default connection in `BackEnd/appsettings.json` targets SQL Server LocalDB. To use a different SQL Server, override it with User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=ShopDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True" --project BackEnd
```

رشتهٔ اتصال را با روش احراز هویت و تنظیمات محیط خود هماهنگ کنید. برای LocalDB پیش‌فرض این مرحله لازم نیست.

Adjust the connection string to match your SQL Server authentication and environment. Skip this step when using the default LocalDB setup.

### ۴. اعمال Migration و اجرا | Apply migrations and run

```bash
dotnet ef database update --project BackEnd --startup-project BackEnd
dotnet run --project BackEnd --launch-profile https
```

در تنظیمات فعلی، برنامه روی `https://localhost:7011` و `http://localhost:5267` اجرا می‌شود. برای ورود و استفاده از کوکی امن، HTTPS را به‌کار ببرید.

With the current launch settings, the app runs at `https://localhost:7011` and `http://localhost:5267`. Use HTTPS for sign-in and secure-cookie behavior.

## 📚 مستندات API | API documentation

در محیط **Development**، مستندات OpenAPI و رابط Scalar فعال هستند:

OpenAPI and Scalar are enabled in the **Development** environment:

**Scalar UI:** [`https://localhost:7011/scalar/v1`](https://localhost:7011/scalar/v1)

## 🧭 مرور API | API overview

تمام endpointها زیر پیشوند `/api` قرار دارند. مسیرهای محافظت‌شده به احراز هویت نیاز دارند و بر اساس نقش محدود شده‌اند.

All endpoints are under the `/api` prefix. Protected routes require authentication and may be restricted by role.

| Controller | Endpointهای منتخب | دسترسی معمول |
|---|---|---|
| `Auth` | `POST /api/Auth/register`, `POST /api/Auth/login`, `POST /api/Auth/logout`, `GET /api/Auth/me`, `POST /api/Auth/change-password` | عمومی / کاربر واردشده · Public / authenticated |
| `PublicProducts` | `GET /api/PublicProducts/products`, `GET /api/PublicProducts/products/{id}/image` | عمومی · Public |
| `Products` | `GET/POST /api/Products`, `GET/PATCH/DELETE /api/Products/{id}`, `PATCH /api/Products/{id}/approval` | مدیر و فروشنده؛ تأیید فقط مدیر · Admin and Seller; approval is Admin-only |
| `ProductCategories` | `GET /api/ProductCategories`, `POST /api/ProductCategories`, `PATCH/DELETE /api/ProductCategories/{id}` | خواندن عمومی؛ مدیریت مدیر · Public read; Admin management |
| `Cart` | `GET /api/Cart`, `POST /api/Cart/items`, `PATCH/DELETE /api/Cart/items/{productId}` | مشتری · Customer |
| `Orders` | `POST /api/Orders`, `GET /api/Orders`, `GET /api/Orders/{id}`, `PATCH /api/Orders/{id}/cancel` | مشتری · Customer |
| `SellerOrders` | `GET /api/SellerOrders`, `GET /api/SellerOrders/{orderId}`, `PATCH /api/SellerOrders/{orderId}/fulfillment` | فروشنده و مدیر · Seller and Admin |
| `AdminOrders` | `GET /api/AdminOrders`, `GET /api/AdminOrders/{id}`, مسیرهای تصمیم‌گیری و تأیید سفارش | مدیر · Admin |
| `Users` | `GET /api/Users`, `GET/PATCH/DELETE /api/Users/{id}` | مدیر · Admin |

برای فهرست کامل endpointها، پارامترها و مدل‌های درخواست/پاسخ، Scalar یا مستندات OpenAPI را در محیط توسعه ببینید.

For the complete endpoint list, parameters, and request/response schemas, use Scalar or the OpenAPI document in Development.

## 🧱 ساختار پروژه | Project structure

```text
.
├── BackEnd.slnx
├── BackEnd/
│   ├── Controllers/   # API endpoints
│   ├── DTOs/          # Request and response models
│   ├── Data/          # DbContext and roles
│   ├── Migrations/    # EF Core migrations
│   ├── Models/        # Domain and identity models
│   ├── IServices/     # Service abstractions
│   ├── Services/      # Business logic
│   ├── Program.cs     # App configuration and middleware
│   └── appsettings*.json
└── README.md
```

## 🔒 امنیت و نکات استقرار | Security & deployment

- کلید JWT و اطلاعات ورود مدیر را خارج از مخزن و در یک Secret Manager امن نگه دارید.
- توکن در کوکی `HttpOnly`, `Secure`, `SameSite=None` نگهداری می‌شود؛ استقرار را با HTTPS انجام دهید و CORS/دامنه‌ها را متناسب با کلاینت تنظیم کنید.
- نقش‌ها و حساب مدیر از طریق seeder برنا��ه ساخته می‌شوند؛ مقادیر Seed باید پیش از اجرا تنظیم شده باشند.
- پروژه در حال حاضر Migration اولیه دارد؛ پیش از استقرار عملیاتی، پیکربندی، دسترسی‌ها و نیازمندی‌های امنیتی را بررسی کنید.

- Keep the JWT signing key and administrator credentials in a secure secret manager, outside the repository.
- The token is stored in an `HttpOnly`, `Secure`, `SameSite=None` cookie. Deploy over HTTPS and configure CORS and allowed domains for your client.
- Roles and the initial admin are created by the application seeder; configure the Seed values before startup.
- The repository currently contains an initial migration. Review configuration, authorization, and security requirements before production deployment.

## 🤝 مشارکت | Contributing

از مشارکت استقبال می‌شود! برای تغییرات بزرگ، لطفاً ابتدا یک Issue ایجاد کنید تا دربارهٔ راهکار پیشنهادی هماهنگ شویم.

Contributions are welcome! For substantial changes, please open an Issue first to discuss the proposed approach.

## 📄 مجوز | License

در حال حاضر فایل License در مخزن وجود ندارد. پیش از استفاده یا بازتوزیع، مجوز پروژه را مشخص کنید.

There is currently no license file in the repository. Add a license before using or redistributing the project.

---

<div align="center">

**Built with ASP.NET Core · ساخته‌شده با ASP.NET Core**

</div>
