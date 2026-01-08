using Analytics.API.Controllers;
using Analytics.Infrastructure;
using Common.Infrastructure.DependencyInjection;
using EventBus.Infrastructure;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.DependencyInjection;
using Identity.API.Controllers;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Controllers;
using Notifications.Infrastructure;
using Support.API.Controllers;
using Support.Infrastructure;
using Support.Infrastructure.Db;
using University.API.Controllers;
using University.Infrastructure;
using University.Infrastructure.Db;
using FluentValidation.AspNetCore;
using FacultyController = Faculty.API.Controllers.FacultyController;

// Npgsql/Postgres timestamp compatibility for local dev.
// Prevents failures when DateTime.Kind is Unspecified but the DB column is timestamptz.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services from modules
builder.Services.AddCommonModule();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddUniversityModule(builder.Configuration);
builder.Services.AddFacultyModule(builder.Configuration);
builder.Services.AddSupportModule(builder.Configuration);
builder.Services.AddNotificationsModule();
builder.Services.AddAnalyticsModule();
builder.Services.AddEventBus();

// Add controllers and module API assemblies
var mvcBuilder = builder.Services.AddControllers();

var moduleControllers = new[]
{
    typeof(IdentityController).Assembly,
    typeof(UniversityController).Assembly,
    typeof(FacultyController).Assembly,
    typeof(SupportController).Assembly,
    typeof(NotificationsController).Assembly,
    typeof(AnalyticsController).Assembly
};

foreach (var asm in moduleControllers)
{
    mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(asm));
}

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Load all XML documentation files (e.g. Application.xml, Identity.API.xml)
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");

    foreach (var xmlPath in xmlFiles)
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

var app = builder.Build();

// Put false if you dont want to apply migrations on start
var applyMigrations = true;

if (applyMigrations)
{

    using (var scope = app.Services.CreateScope())
    {
    var services = scope.ServiceProvider;

    // Identity module
    try
    {
        var identityDb = services.GetRequiredService<AuthDbContext>();
        identityDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating IdentityDbContext: {ex.Message}");
    }

    // University module
    try
    {
        var universityDb = services.GetRequiredService<UniversityDbContext>();
        universityDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating UniversityDbContext: {ex.Message}");
    }

    // Faculty module
    try
    {
        var facultyDb = services.GetRequiredService<FacultyDbContext>();
        facultyDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating FacultyDbContext: {ex.Message}");
    }

    // Support module
        try
        {
            var supportDb = services.GetRequiredService<SupportDbContext>();
            supportDb.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error migrating SupportDbContext: {ex.Message}");
        }
    }

}

// Middleware
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

// Map controllers
app.MapControllers();

app.Run();