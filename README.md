# Motorak

Motorak is an ASP.NET Core MVC web application for automotive sales, rentals, servicing, and customer management. Users can browse vehicles, buy or rent cars, add vehicles to a shopping cart, request mechanic services, and review completed services. The application also includes role-based administration and mechanic/customer workflows.

> **Project status:** This README documents the current source tree. The solution does not currently include an automated test project.

## Features

- Car inventory management for vehicles offered for sale or rent
- Car details, images, pricing, and category information
- Shopping cart for authenticated users
- Purchase and rental transactions
- Customer and mechanic profiles
- Service requests assigned to mechanics
- Service reviews and ratings
- ASP.NET Core Identity registration, login, account management, and role support
- Google and Facebook external authentication hooks
- Email delivery through SendGrid for Identity email workflows
- English (`en-US`) and Arabic (`ar-EG`) localization
- Background processing with Hangfire, including recurring reminders
- Razor Pages for Identity and MVC views for the main application

## Technology stack

- .NET 8 / ASP.NET Core MVC
- Entity Framework Core with SQL Server
- ASP.NET Core Identity
- Hangfire with SQL Server storage
- AutoMapper
- Razor Views and Razor Pages
- Bootstrap, jQuery, and client-side validation
- SendGrid

## Solution structure

```text
motorakNewSol/
├── motoraktest.sln             # Visual Studio solution
├── motorak.pll/                # ASP.NET Core presentation/web layer
│   ├── Controllers/            # MVC controllers
│   ├── Views/                  # MVC views
│   ├── Areas/Identity/         # Identity UI and account management
│   ├── wwwroot/                # CSS, JavaScript, images, and videos
│   ├── Program.cs              # Application startup and dependency injection
│   └── appsettings*.json       # Application configuration
├── motorak.bll/                # Business logic, services, view models, and mappings
├── motorak.dal/                # EF Core DbContext, entities, repositories, enums, migrations
└── Motorak.Utility/            # Email and shared utility components
```

## Prerequisites

- Windows, macOS, or Linux
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Optional: Visual Studio 2022 or VS Code with the C# Dev Kit
- Optional: SendGrid account and verified sender for email features
- Optional: Google and Facebook OAuth applications for external login

Verify the SDK installation:

```bash
dotnet --version
```

The solution targets `net8.0` in all four projects.

## Configuration

The web project currently reads the `defaultConnection` connection string from [`motorak.pll/appsettings.json`](motorak.pll/appsettings.json). Update it to point to a SQL Server instance available on your machine. Do not commit production connection strings, passwords, API keys, or OAuth secrets.

For local development, prefer user secrets or environment variables:

```bash
dotnet user-secrets --project motorak.pll init
dotnet user-secrets --project motorak.pll set "ConnectionStrings:defaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=Motorak;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets --project motorak.pll set "SendGrid:ApiKey" "your-sendgrid-api-key"
dotnet user-secrets --project motorak.pll set "SendGrid:FromEmail" "verified-sender@example.com"
dotnet user-secrets --project motorak.pll set "SendGrid:FromName" "Motorak Support"
```

If external authentication is enabled, configure provider credentials outside source control as well. The current startup code contains provider configuration that should be moved to configuration or user secrets and rotated before sharing or deploying the application.

## Database setup

The application registers `MotorakDbContext` with SQL Server and calls `EnsureCreatedAsync()` during startup. It also contains EF Core migrations under [`motorak.dal/Migrations`](motorak.dal/Migrations).

For a development database, the normal startup flow is:

```bash
dotnet restore motoraktest.sln
dotnet build motoraktest.sln
dotnet run --project motorak.pll
```

If you use migrations instead of the current `EnsureCreated` startup behavior, apply them with:

```bash
dotnet ef database update --project motorak.dal --startup-project motorak.pll
```

The application creates the Identity roles `Admin`, `Customer`, and `Mechanic` at startup. It also attempts to create the development administrator defined in [`motorak.dal/DataTemp/AdminLogins.cs`](motorak.dal/DataTemp/AdminLogins.cs). Change that seed behavior and credentials before using the application outside a local development environment.

## Running the application

From the repository root:

```bash
dotnet run --project motorak.pll
```

The launch profiles expose these development URLs:

- HTTP: `http://localhost:5093`
- HTTPS: `https://localhost:7052`
- IIS Express: `http://localhost:5941` and HTTPS port `44313`

The exact URL may vary when using Visual Studio or another launch profile.

## Main routes and areas

- `/` — home page
- `/Car` — vehicle inventory
- `/Cart` — authenticated user cart
- `/Purchase` — purchase workflow
- `/Rent` — rental workflow
- `/Service` — service requests
- `/ServiceReview` — service reviews
- `/Customer` — customer management
- `/Mechanic` — mechanic management
- `/Payment` — payment page
- `/Identity/Account/Login` — Identity login
- `/Identity/Account/Register` — Identity registration
- `/Mustafa` — Hangfire dashboard

The Hangfire dashboard is currently exposed at `/Mustafa`. Protect it with an authorization filter before deploying to a shared or production environment.

## Background jobs

Hangfire uses the configured SQL Server connection and starts a server when the web application starts. A recurring `ReminderService.ShowReminder` job is scheduled to run every minute.

## Development notes

- Keep secrets in user secrets, environment variables, or a deployment secret store.
- Use a dedicated development database; do not point local development at production data.
- Review and rotate any credentials present in existing configuration or startup code.
- The application uses `EnsureCreatedAsync()` at startup while migrations are also present. Choose one database lifecycle strategy for production and apply it consistently.
- There are no automated tests in the current solution. Add a test project before relying on CI-based regression coverage.

## License

No license file is currently included in the repository. Add an appropriate license before distributing the project publicly.
