using Common.Infrastructure.DependencyInjection;
using EventBus.Infrastructure;
using Faculty.Infrastructure.DependencyInjection;
using Identity.Application.Interfaces;
using Identity.Core.Enums;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Identity.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Support.Infrastructure;
using University.Infrastructure;

static IConfiguration BuildConfiguration()
{
    // Match the DesignTimeDbContextFactory pattern: locate Application/appsettings.json.
    var currentDir = Directory.GetCurrentDirectory();
    var directory = new DirectoryInfo(currentDir);

    while (directory != null)
    {
        var appDir = Path.Combine(directory.FullName, "Application");
        if (Directory.Exists(appDir) && File.Exists(Path.Combine(appDir, "appsettings.json")))
        {
            return new ConfigurationBuilder()
                .SetBasePath(appDir)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Could not find Application/appsettings.json. Run this from within the repo.");
}

static Guid Tenant(string guid) => Guid.Parse(guid);

var configuration = BuildConfiguration();

var connectionString = configuration.GetConnectionString("Database");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Database is missing. Set ConnectionStrings__Database.");
}

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddCommonModule();

        // Module registrations (match Application/Program.cs)
        services.AddIdentityModule(configuration);
        services.AddUniversityModule(configuration);
        services.AddFacultyModule(configuration);
        services.AddSupportModule(configuration);
        services.AddEventBus();
    })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

// Apply migrations (standard modules)
await services.GetRequiredService<Identity.Infrastructure.Db.AuthDbContext>().Database.MigrateAsync();
await services.GetRequiredService<University.Infrastructure.Db.UniversityDbContext>().Database.MigrateAsync();
await services.GetRequiredService<Support.Infrastructure.Db.SupportDbContext>().Database.MigrateAsync();

// Faculty migrations are not safe to auto-apply in the Web host (tenant resolution),
// but in this seeder we can apply them using a design-time tenant.
await using (var facultyDb = new Faculty.Infrastructure.Db.FacultyDbContextFactory().CreateDbContext(Array.Empty<string>()))
{
    await facultyDb.Database.MigrateAsync();
}

// Seed tenants (FacultyIds are GUID tenants in Identity + Faculty module)
var tenants = new[]
{
    Tenant("11111111-1111-1111-1111-111111111111"),
    Tenant("22222222-2222-2222-2222-222222222222"),
    Tenant("33333333-3333-3333-3333-333333333333"),
};

var userService = services.GetRequiredService<IUserService>();
var userRepository = services.GetRequiredService<IUserRepository>();
var hasher = services.GetRequiredService<IIdentityHasherService>();

const string defaultPassword = "Pwd123!";

static string Email(string prefix, Guid tenant) => $"{prefix}.{tenant.ToString("N").Substring(0, 8)}@qa.local";
static string Username(string prefix, Guid tenant) => $"{prefix}_{tenant.ToString("N").Substring(0, 8)}";
static string Index(Guid tenant) => $"{tenant.ToString("N").Substring(0, 6)}";

foreach (var tenantId in tenants)
{
    // Admin (direct insert because IUserService forbids Admin/Superadmin)
    {
        var adminEmail = Email("admin", tenantId);
        var existingAdmin = await userRepository.GetByEmailAsync(adminEmail, CancellationToken.None);
        if (existingAdmin == null)
        {
            var passwordHash = hasher.HashPassword(defaultPassword);
            var admin = Identity.Core.Entities.User.Create(
                Username("admin", tenantId),
                passwordHash,
                "QA",
                "Admin",
                adminEmail,
                tenantId,
                UserRole.Admin);

            await userRepository.AddAsync(admin);
            await userRepository.SaveAsync();
        }
    }

    // Teacher (through service to exercise event bus)
    {
        var teacherEmail = Email("teacher", tenantId);
        var existingTeacher = await userRepository.GetByEmailAsync(teacherEmail, CancellationToken.None);
        if (existingTeacher == null)
        {
            await userService.CreateUserAsync(
                Username("teacher", tenantId),
                defaultPassword,
                "QA",
                "Teacher",
                teacherEmail,
                tenantId,
                null,
                UserRole.Teacher);
        }
    }

    // Student (through service to trigger Faculty student creation via event)
    {
        var studentEmail = Email("student", tenantId);
        var existingStudent = await userRepository.GetByEmailAsync(studentEmail, CancellationToken.None);
        if (existingStudent == null)
        {
            await userService.CreateUserAsync(
                Username("student", tenantId),
                defaultPassword,
                "QA",
                "Student",
                studentEmail,
                tenantId,
                Index(tenantId),
                UserRole.Student);
        }
    }
}

Console.WriteLine("Sandbox Sprint 4 seed complete.");
Console.WriteLine($"Connection string: {connectionString}");
Console.WriteLine($"Default password for seeded users: {defaultPassword}");
Console.WriteLine("Tenants (FacultyId GUIDs):");
foreach (var t in tenants)
{
    Console.WriteLine($"- {t}");
}
