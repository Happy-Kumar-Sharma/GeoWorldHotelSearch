# GeoWorldHotelSearch Developer Guide

Welcome to the **GeoWorldHotelSearch** project! This guide will help you understand the project structure, development methodology, and best practices for contributing new features (such as add/edit/remove maintenance charges) or maintaining the codebase. Whether you are a beginner or an experienced developer, this document will walk you through every detail you need to work efficiently and confidently.

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Project Structure](#project-structure)
3. [Visual Project Architecture Diagram](#visual-project-architecture-diagram)
4. [How the Project Works (File Connections)](#how-the-project-works-file-connections)
5. [Project Flow & Methodology](#project-flow--methodology)
6. [How to Add a New Feature (Example: Maintenance Charges)](#how-to-add-a-new-feature-example-maintenance-charges)
7. [Best Practices](#best-practices)
8. [Development Environment Setup](#development-environment-setup)
9. [Common Commands](#common-commands)
10. [Troubleshooting](#troubleshooting)
11. [Further Reading](#further-reading)

---

## Project Overview
GeoWorldHotelSearch is an ASP.NET Core MVC web application for searching, listing, and managing hotels. It supports features like hotel search, filtering, booking, and admin management, with a clean separation of concerns and extensible architecture.

---

## Project Structure

```
GeoWorldHotelSearch/
│
├── Controllers/                # MVC Controllers (handle HTTP requests)
│   ├── ApiController.cs
│   ├── BookingController.cs
│   ├── DashboardController.cs
│   ├── HomeController.cs
│   └── HotelsController.cs
│
├── Data/                      # Database context and configuration
│   └── AppDbContext.cs
│
├── Migrations/                # Entity Framework Core migrations
│   ├── <timestamp>_*.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Models/                    # Domain models (entities, view models)
│   ├── Booking.cs
│   ├── DashboardViewModel.cs
│   ├── ErrorViewModel.cs
│   ├── Hotel.cs
│   ├── SearchQuery.cs
│   └── SearchViewModel.cs
│
├── Repositories/              # Data access layer (interfaces & implementations)
│   ├── HotelRepository.cs
│   └── IHotelRepository.cs
│
├── Services/                  # Business logic layer (interfaces & implementations)
│   ├── DashboardService.cs
│   ├── ElasticsearchService.cs
│   ├── HotelService.cs
│   ├── IDashboardService.cs
│   ├── IElasticsearchService.cs
│   └── IHotelService.cs
│
├── Views/                     # Razor views (UI)
│   ├── Home/
│   ├── Hotels/
│   ├── Booking/
│   ├── Shared/
│   └── _ViewImports.cshtml, _ViewStart.cshtml
│
├── wwwroot/                   # Static files (CSS, JS, images, libraries)
│   ├── css/
│   ├── js/
│   └── lib/
│
├── Properties/                # Project properties (launch settings)
│   └── launchSettings.json
│
├── elastic-start-local/       # Local Elasticsearch setup scripts
│
├── Program.cs                 # Application entry point
├── GeoWorldHotelSearch.csproj # Project file
├── GeoWorldHotelSearch.sln    # Solution file
├── appsettings.json           # App configuration
├── appsettings.Development.json
├── README.md                  # Project overview
└── ...
```

---

## Visual Project Architecture Diagram

Below is a simple diagram showing how the main parts of the project connect and interact. Each arrow (→) shows the direction of data or control flow.

```
[User/Browser]
     │
     ▼
[Views (Razor)]
     │
     ▼
[Controllers]
     │
     ▼
[Services]
     │
     ▼
[Repositories]
     │
     ▼
[Data/AppDbContext]
     │
     ▼
[Database]
```

**Legend:**
- **Views**: UI pages (.cshtml) shown to the user.
- **Controllers**: Receive user actions, call services, return views.
- **Services**: Business logic, validation, and orchestration.
- **Repositories**: Data access (CRUD operations).
- **AppDbContext**: Entity Framework Core context for DB.
- **Database**: Actual data storage (e.g., SQL Server, PostgreSQL).

---

## How the Project Works (File Connections)

Let's walk through a typical request, such as searching for hotels:

1. **User interacts with the UI** (e.g., submits a search form in `Views/Hotels/Index.cshtml`).
2. **Controller receives the request** (e.g., `HotelsController.cs`).
   - Validates input.
   - Calls the appropriate method in a service (e.g., `HotelService`).
3. **Service processes the request** (e.g., `HotelService.cs`).
   - Applies business logic.
   - Calls repository methods for data access.
4. **Repository fetches data** (e.g., `HotelRepository.cs`).
   - Uses `AppDbContext` to query the database.
5. **Data is returned up the stack**:
   - Repository → Service → Controller → View.
6. **View renders the result** and sends HTML back to the user's browser.

**Example: Adding a New Feature (Maintenance Charges)**
- Add a model in `Models/` (e.g., `MaintenanceCharge.cs`).
- Update `AppDbContext.cs` in `Data/`.
- Add repository interface and implementation in `Repositories/`.
- Add service interface and implementation in `Services/`.
- Add controller in `Controllers/`.
- Add views in `Views/MaintenanceCharge/`.

**File Connection Map:**

```
[Views/Hotels/Index.cshtml] ←→ [HotelsController.cs] ←→ [HotelService.cs] ←→ [HotelRepository.cs] ←→ [AppDbContext.cs] ←→ [Migrations/*.cs] ←→ [Database]
```

- **Controllers** depend on **Services** (via constructor injection).
- **Services** depend on **Repositories** (via constructor injection).
- **Repositories** use **AppDbContext**.
- **AppDbContext** is configured in `Program.cs` and uses settings from `appsettings.json`.
- **Migrations** are auto-generated files that update the database schema.

---

## Project Flow & Methodology

The project follows the **MVC (Model-View-Controller)** pattern and a layered architecture:

- **Controllers**: Handle HTTP requests, validate input, and coordinate between services and views.
- **Services**: Contain business logic, orchestrate data access, and enforce rules.
- **Repositories**: Abstract data access (usually via Entity Framework Core).
- **Models**: Represent data structures (entities, DTOs, view models).
- **Views**: Render HTML UI using Razor syntax.

**Development Methodology:**
- Feature-driven development: Each feature is developed end-to-end (model, data, logic, UI).
- Separation of concerns: Each layer has a clear responsibility.
- Testability: Logic is placed in services for easy unit testing.
- Migration-based DB changes: All schema changes are tracked via EF Core migrations.

---

## How to Add a New Feature (Example: Maintenance Charges)

Suppose you want to add a feature to manage additional maintenance charges for hotels (add/edit/remove):

### 1. **Design the Data Model**
- Decide if you need a new entity (e.g., `MaintenanceCharge`) or a new property on `Hotel`.
- Edit or create the model in `Models/` (e.g., `Models/MaintenanceCharge.cs`).

### 2. **Update the Database**
- If you changed the model, update `AppDbContext.cs` in `Data/` to add a `DbSet<MaintenanceCharge>` or update the `Hotel` entity.
- Create a migration:
  ```bash
  dotnet ef migrations add AddMaintenanceCharge
  dotnet ef database update
  ```

### 3. **Repository Layer**
- Add methods to the repository interface (e.g., `IMaintenanceChargeRepository.cs` in `Repositories/`).
- Implement them in the repository class (e.g., `MaintenanceChargeRepository.cs`).

### 4. **Service Layer**
- Add methods to the service interface (e.g., `IMaintenanceChargeService.cs` in `Services/`).
- Implement them in the service class (e.g., `MaintenanceChargeService.cs`).

### 5. **Controller Layer**
- Add a new controller (e.g., `MaintenanceChargeController.cs` in `Controllers/`) or update an existing one.
- Implement actions for add/edit/remove.

### 6. **Views (UI)**
- Add Razor views in `Views/MaintenanceCharge/` (e.g., `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`).
- Update navigation in shared layout if needed.

### 7. **Validation & Testing**
- Add validation attributes to models.
- Test the feature end-to-end.

### 8. **Documentation**
- Update `README.md` and this `developer_guide.md` with details about the new feature.

---

## Best Practices

- **Follow MVC and Layered Architecture:** Keep logic in services, not controllers or views.
- **Use Dependency Injection:** Register services and repositories in `Program.cs`.
- **Write Clean, Readable Code:** Use meaningful names, comments, and consistent formatting.
- **Use ViewModels for UI:** Avoid exposing entities directly to views.
- **Handle Errors Gracefully:** Use try-catch, return user-friendly messages.
- **Keep Migrations Up-to-Date:** Always run migrations after model changes.
- **Test Thoroughly:** Unit test services, integration test controllers.
- **Document Your Code:** Use XML comments and keep documentation updated.
- **Respect Separation of Concerns:** Don’t mix data, logic, and UI.
- **Use Git for Version Control:** Commit often with clear messages.
- **Keep Views Simple:** Views should only display data, not contain logic.
- **Use Partial Views and Layouts:** For reusable UI components.
- **Comment Complex Logic:** Make your code easy for others to understand.
- **Keep Configuration Out of Code:** Use `appsettings.json` for settings.

---

## Development Environment Setup

1. **Clone the Repository:**
   ```bash
   git clone <repo-url>
   cd GeoWorldHotelSearch
   ```
2. **Install .NET SDK (8.0 or later):**
   [Download .NET](https://dotnet.microsoft.com/download)
3. **Restore Dependencies:**
   ```bash
   dotnet restore
   ```
4. **Apply Migrations:**
   ```bash
   dotnet ef database update
   ```
5. **Run the Application:**
   ```bash
   dotnet run
   ```
6. **Access in Browser:**
   Visit [http://localhost:5176](http://localhost:5176)

---

## Common Commands

- **Build:**
  ```bash
  dotnet build
  ```
- **Run:**
  ```bash
  dotnet run
  ```
- **Add Migration:**
  ```bash
  dotnet ef migrations add <MigrationName>
  ```
- **Update Database:**
  ```bash
  dotnet ef database update
  ```
- **List Migrations:**
  ```bash
  dotnet ef migrations list
  ```
- **Test:**
  ```bash
  dotnet test
  ```

---

## Troubleshooting

- **Port Already in Use:**
  ```bash
  lsof -i :5176
  kill -9 <PID>
  ```
- **Migration Issues:**
  - Check for pending migrations and apply them.
- **Database Connection Errors:**
  - Verify connection string in `appsettings.json`.
- **Static Files Not Loading:**
  - Check `wwwroot/` and ensure files are referenced correctly in views.

---

## Further Reading
- [ASP.NET Core MVC Documentation](https://docs.microsoft.com/aspnet/core/mvc/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Razor Pages](https://docs.microsoft.com/aspnet/core/razor-pages/)
- [C# Coding Conventions](https://docs.microsoft.com/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)

---

> **Happy Coding!**

For any questions, please refer to the project maintainers or open an issue in the repository.
