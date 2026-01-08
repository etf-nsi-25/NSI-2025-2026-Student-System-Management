# UNSA Student Management System (UNSA SMS)

## Overview
UNSA SMS is a modular .NET 8 application for managing university data including identity, courses, students, support, notifications, and analytics.  
It follows a *clean modular architecture* with separate modules for each major domain.

The system is designed so each module is *autonomous, has its own database context, and communicates with other modules via **interfaces and events*.  
The Common module provides shared utilities, DTOs, and interfaces.

---

## Modules

- *Identity* – User registration, JWT generation and verification.  
- *University* – University/faculty management (courses, faculties).  
- *Faculty* – Student management, attendance, exams, grades.  
- *Support* – Faculty support systems (document requests, enrollment).  
- *Notifications* – Send notifications via email, SMS, etc.  
- *Analytics* – Periodic statistics and data queries.  
- *Common* – Shared utilities, DTOs, interfaces, enums.  
- *Application* – Main entry point that wires all modules together.

---

## Project Structure
UNSA_SMS/
│
├─ Application/ # Main entry point (Web API)
├─ Modules/
│ ├─ Identity/ # Identity module (API, Core, Infrastructure)
│ ├─ University/ # University management module
│ ├─ Faculty/ # Student management module
│ ├─ Support/ # Faculty support module
│ ├─ Notifications/ # Notifications module
│ └─ Analytics/ # Analytics and reporting module
├─ Common/ # Shared utilities, DTOs, interfaces, enums
└─ Directory.Packages.props # Centralized NuGet versions

---

## Running the Application

Before running the application it is necessary to provide environment variables in order to 
connect to database. Database can be started via `docker-compose` (check Deployment folder).

To connect to the database locally you should set an environment variable such as:

`export ConnectionStrings__Database="Host=localhost;Port=5432;Database=unsa;Username=unsa;Password=unsa"`

1. *Open the solution*
   - Open UNSA_SMS.sln in *Visual Studio 2022+* or *VS Code*.

2. *Restore packages*
  
    - dotnet restore
3.  *Build the solution* 
    - dotnet build
4. *Run the Application project*   
    - cd Application
    - dotnet run   
5. *Open Swagger UI*
   https://localhost:7229/index.html

## Database Migrations

All migrations are applied automatically on application startup. You can disable this by setting the applyMigrations variable in Program.cs to false.

Note: FacultyDbContext depends on HTTP context, so migrations for this module cannot run automatically on startup. They must be run manually in the 
Package Manager Console and Set Default Project to Faculty.Infrastructure after that run "Update-Database"

### Adding a New Migration

If you add or modify an entity in a module:

Open Package Manager Console.

Set Default Project to the infrastructure project of the modified module.

Run:

Add-Migration <MIGRATION_NAME> -Context <ModuleDbContext> -OutputDir Migrations


This will create a new migration in the Migrations folder of the modified module.

To apply it to the database, run:

Update-Database

### Removing an Incorrect Migration

If a migration was created by mistake run:

Remove-Migration


Make sure the Default Project is set to the module where the migration was created.

### Downgrading / Reverting a Migration

To revert to a previous migration:

Update-Database -Migration <PREVIOUS_MIGRATION_NAME>


This will roll back the database to the specified migration.

Always ensure you are in the correct Default Project corresponding to the DbContext.

## QA Sandbox

To bring up a clean, isolated QA sandbox database (Docker Postgres + migrations + seeded tenants/users), run:

- `powershell -ExecutionPolicy Bypass -File Deployment/local/sandbox.ps1 -ResetDb`

This will reset the local Postgres volume, create/use the `unsa_test_sprint4` database, apply migrations, and seed 3 tenants + users.

## Frontend 
The frontend is a React 19 application. To install it you will need to install Node 20.
To start the frontend:
1. navigate to `Frontend` module
2. Execute `run npm install` to install dependencies
3. Execute `run npm dev` to run the app in dev mode

Before commiting new changes to frontend please run `npm run build` to make sure there are no build
breaking changes.

### SSL
In order to run the app in Https mode (which is required) you will need to generate a 
certificate/key pair which is trusted both by the backend .net and vite.

This key/certificate pair is automatically generated when you run `npm run build` or `dotnet build`.
