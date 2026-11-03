<div dir="rtl">

# فروشگاه آنلاین — REST API

یک API بک‌اند برای مدیریت فروشگاه آنلاین چندنقشی که با **ASP.NET Core Web API** و **C#** ساخته شده است. این پروژه امکانات احراز هویت، مدیریت محصولات و دسته‌بندی‌ها، سبد خرید و گردش سفارش‌ها را برای مشتری، فروشنده و مدیر فراهم می‌کند.

</div>

# Online Shop — REST API

A role-based backend API for an online shop, built with **ASP.NET Core Web API** and **C#**. It provides authentication, product and category management, shopping carts, and order workflows for customers, sellers, and administrators.

---

## فهرست مطالب | Contents

- [ویژگی‌ها | Features](#ویژگیها--features)
- [فناوری‌ها | Tech stack](#فناوریها--tech-stack)
- [پیش‌نیازها | Prerequisites](#پیشنیازها--prerequisites)
- [راه‌اندازی | Getting started](#راهاندازی--getting-started)
- [مستندات API | API documentation](#مستندات-api--api-documentation)
- [مسیرهای اصلی API | Main API routes](#مسیرهای-اصلی-api--main-api-routes)
- [ساختار پروژه | Project structure](#ساختار-پروژه--project-structure)
- [امنیت و تنظیمات | Security and configuration](#امنیت-و-تنظیمات--security-and-configuration)

## ویژگی‌ها | Features

<div dir="rtl">

- ثبت‌نام، ورود، خروج و تغییر رمز عبور با استفاده از ASP.NET Core Identity و JWT.
- ذخیرهٔ توکن دسترسی در کوکی `HttpOnly` و پشتیبانی از نقش‌های **Admin**، **Seller** و **Customer**.
- مدیریت محصولات شامل ایجاد، ویرایش، حذف، بارگذاری تصویر و تأیید محصول توسط مدیر.
- نمایش عمومی محصولات تأییدشده و دسته‌بندی‌های کالا.
- مدیریت سبد خرید مشتری و ثبت و پیگیری سفارش.
- گردش سفارش چندفروشنده‌ای با امکان بررسی و تأیید توسط فروشنده و مدیر.
- مدیریت کاربران و دسته‌بندی‌ها با دسترسی‌های مبتنی بر نقش.
- ذخیره‌سازی داده‌ها با SQL Server و Entity Framework Core؛ مستندات OpenAPI و رابط Scalar در محیط توسعه.

</div>

- Registration, login, logout, and password changes using ASP.NET Core Identity and JWT.
- `HttpOnly` access-token cookie and **Admin**, **Seller**, and **Customer** roles.
- Product creation, editing, deletion, image upload, and administrator approval.
- Public access to approved products and product categories.
- Customer shopping carts, order placement, and order tracking.
- Multi-seller order workflows with seller and administrator review and approval.
- Role-protected user and category management.
- SQL Server persistence via Entity Framework Core; OpenAPI documentation and Scalar UI in development.

## فناوری‌ها | Tech stack

- C# / .NET 10 (`net10.0`)
- ASP.NET Core Web API
- Entity Framework Core 10 و SQL Server
- ASP.NET Core Identity و JWT Bearer Authentication
- OpenAPI و Scalar

## پیش‌نیازها | Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server یا SQL Server LocalDB
- ابزار EF Core CLI برای اجرای migrationها:

```bash
dotnet tool install --global dotnet-ef
```

در صورت نصب بودن ابزار، نسخهٔ آن را بررسی/به‌روزرسانی کنید:

```bash
dotnet tool update --global dotnet-ef
```

## راه‌اندازی | Getting started

### ۱. دریافت پروژه | Clone the repository

```bash
git clone https://github.com/AbdollahiAmir/ASP.net-Core-shop-API.git
cd ASP.net-Core-shop-API
```

### ۲. تنظیم مقادیر محرمانه | Configure secrets

برنامه هنگام اجرا نقش‌ها و حساب مدیر اولیه را ایجاد می‌کند؛ بنابراین نام کاربری و رمز عبور مدیر باید تنظیم شده باشند. همچنین کلید امضای JWT باید در تنظیمات قرار گیرد. مقادیر زیر را با User Secrets (مناسب توسعهٔ محلی) تنظیم کنید:

The application seeds the roles and initial administrator account at startup, so the administrator credentials must be configured. A JWT signing key is also required. Set these values with User Secrets (recommended for local development):

```bash
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret-key" --project BackEnd
dotnet user-secrets set "Seed:AdminUserName" "admin" --project BackEnd
dotnet user-secrets set "Seed:AdminPassword" "replace-with-a-strong-password" --project BackEnd
```

رمز عبور باید الزامات Identity را رعایت کند: حداقل ۸ نویسه و شامل حرف کوچک، حرف بزرگ، عدد و نویسهٔ غیرحرفی. برای محیط غیرتوسعه، از Secret Manager یا راهکار امن مدیریت اسرار محیط استقرار استفاده کنید؛ اسرار را در مخزن یا فایل‌های تنظیمات commit نکنید.

The password must satisfy the configured Identity policy: at least 8 characters, including lowercase and uppercase letters, a digit, and a non-alphanumeric character. In deployed environments, use a secure secret-management mechanism. Never commit secrets to the repository or configuration files.

### ۳. تنظیم اتصال پایگاه‌داده | Configure the database connection

مقدار پیش‌فرض `DefaultConnection` در `BackEnd/appsettings.json` از **SQL Server LocalDB** استفاده می‌کند. اگر از SQL Server دیگری استفاده می‌کنید، مقدار `ConnectionStrings:DefaultConnection` را با User Secrets تنظیم کنید:

The default `DefaultConnection` in `BackEnd/appsettings.json` uses **SQL Server LocalDB**. If you use another SQL Server instance, override `ConnectionStrings:DefaultConnection` with User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=ShopDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True" --project BackEnd
```

رشتهٔ اتصال را متناسب با روش احراز هویت و محیط SQL Server خود تنظیم کنید. اگر از LocalDB پیش‌فرض استفاده می‌کنید، این مرحله لازم نیست.

Adjust the connection string for your SQL Server and authentication setup. This step is unnecessary when using the default LocalDB connection.

### ۴. اعمال migration و اجرای برنامه | Apply migrations and run

```bash
dotnet ef database update --project BackEnd --startup-project BackEnd
dotnet run --project BackEnd --launch-profile https
```

سرور توسعه با تنظیمات فعلی روی `https://localhost:7011` و `http://localhost:5267` اجرا می‌شود. برای ورود و کار با کوکی امن، از HTTPS استفاده کنید.

With the current launch settings, the development server listens on `https://localhost:7011` and `http://localhost:5267`. Use HTTPS for sign-in and secure-cookie behavior.

## مستندات API | API documentation

در محیط توسعه، مستندات OpenAPI و رابط Scalar فعال هستند. پس از اجرای برنامه، رابط Scalar را در مسیر زیر باز کنید:

In the Development environment, OpenAPI and Scalar are enabled. Once the application is running, open the Scalar UI at:

```text
https://localhost:7011/scalar/v1
```

## مسیرهای اصلی API | Main API routes

تمام مسیرها با پیشوند `/api` ارائه می‌شوند. مسیرهای محافظت‌شده به احراز هویت نیاز دارند و بعضی از آن‌ها فقط برای نقش مشخصی در دسترس هستند.

All routes use the `/api` prefix. Protected routes require authentication, and some are restricted to specific roles.

| Controller | مسیرها و کاربرد | Routes and purpose |
|---|---|---|
| `Auth` | `POST /api/Auth/register`, `POST /api/Auth/login`, `POST /api/Auth/logout`, `GET /api/Auth/me`, `POST /api/Auth/change-password` | ثبت‌نام، ورود/خروج، اطلاعات کاربر و تغییر رمز عبور / Registration, sign-in/out, current user, password change |
| `PublicProducts` | `GET /api/PublicProducts/products`, `GET /api/PublicProducts/products/{id}/image` | محصولات تأییدشده و تصویر آن‌ها / Approved products and images |
| `Products` | `GET/POST /api/Products`, `GET/PATCH/DELETE /api/Products/{id}`, `PATCH /api/Products/{id}/approval` | مدیریت محصولات و تأیید مدیر / Product management and administrator approval |
| `ProductCategories` | `GET/POST /api/ProductCategories`, `GET/PATCH/DELETE /api/ProductCategories/{id}` | مشاهده و مدیریت دسته‌بندی‌ها / Browse and manage categories |
| `Cart` | `GET /api/Cart`, `POST /api/Cart/items`, `PATCH/DELETE /api/Cart/items/{productId}` | عملیات سبد خرید مشتری / Customer cart operations |
| `Orders` | `POST /api/Orders`, `GET /api/Orders`, `GET /api/Orders/{id}`, `PATCH /api/Orders/{id}/cancel` | ثبت، مشاهده و لغو سفارش مشتری / Customer order placement, listing, and cancellation |
| `SellerOrders` | `GET /api/SellerOrders`, `GET /api/SellerOrders/{orderId}`, `PATCH /api/SellerOrders/{orderId}/fulfillment` | سفارش‌ها و وضعیت انجام سفارش توسط فروشنده / Seller orders and fulfillment |
| `AdminOrders` | `GET /api/AdminOrders`, `GET /api/AdminOrders/{id}`, مسیرهای تصمیم‌گیری و تأیید سفارش | مدیریت و بررسی سفارش‌ها توسط مدیر / Administrator order management and review |
| `Users` | `/api/Users` | مدیریت کاربران با دسترسی مدیر / Administrator user management |

برای جزئیات پارامترها و مدل‌های درخواست/پاسخ، مرجع OpenAPI را در محیط توسعه بررسی کنید.

For request/response schemas and complete endpoint details, consult the OpenAPI reference in Development.

## ساختار پروژه | Project structure

```text
.
├── BackEnd.slnx
├── BackEnd/
│   ├── Controllers/   # API endpoints
│   ├── DTOs/          # Request and response models
│   ├── Data/          # EF Core DbContext and roles
│   ├── Migrations/    # EF Core database migrations
│   ├── Models/        # Domain and identity models
│   ├── IServices/     # Service abstractions
│   ├── Services/      # Business logic
│   ├── Program.cs     # Dependency injection and middleware setup
│   └── appsettings*.json
└── README.md
```

## امنیت و تنظیمات | Security and configuration

- کلید JWT و اعتبارنامهٔ مدیر را خارج از Git نگه دارید و برای محیط تولید از مقادیر امن و منحصربه‌فرد استفاده کنید.
- API از کوکی `HttpOnly` با `Secure` و `SameSite=None` برای توکن استفاده می‌کند؛ در استقرار، HTTPS و تنظیمات مناسب دامنه/CORS را در نظر بگیرید.
- رمز عبور مدیر اولیه در اولین راه‌اندازی استفاده می‌شود؛ حساب و نقش‌های اولیه توسط seeder برنامه ایجاد می‌شوند.
- **این پروژه در وضعیت فعلی migration اولیه دارد؛ پیش از استفادهٔ عملیاتی، تنظیمات، سیاست‌های دسترسی و سناریوهای امنیتی را متناسب با نیاز استقرار بررسی کنید.**

- Keep the JWT signing key and administrator credentials out of Git; use unique, securely managed values in production.
- The API stores the token in an `HttpOnly`, `Secure`, `SameSite=None` cookie. Use HTTPS and configure deployment domains/CORS appropriately.
- The initial administrator account and roles are created by the application seeder at startup.
- **The repository currently includes an initial database migration. Review deployment settings, authorization policies, and security requirements before production use.**

## مشارکت | Contributing

از پیشنهادها و مشارکت‌ها استقبال می‌شود. برای تغییرات بزرگ، ابتدا یک Issue ایجاد کنید تا دربارهٔ راهکار هماهنگ شویم.

Contributions and suggestions are welcome. For significant changes, please open an Issue first to discuss the proposed approach.

## مجوز | License

در حال حاضر فایل مجوزی در مخزن تعریف نشده است. پیش از استفاده یا توزیع مجدد، مجوز مناسب پروژه را مشخص کنید.

No license file is currently defined in the repository. Add an appropriate license before distributing or reusing the project.
