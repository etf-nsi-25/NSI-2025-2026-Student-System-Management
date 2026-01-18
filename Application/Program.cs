using Analytics.API.Controllers;
using Analytics.Infrastructure;
using Application.Seed;
using Common.Core.Tenant;
using Common.Infrastructure.DependencyInjection;
using EventBus.Infrastructure;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.DependencyInjection;
using FluentValidation.AspNetCore;
using Identity.API.Controllers;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.DependencyInjection;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Support.API.Controllers;
using Support.Infrastructure;
using Support.Infrastructure.Db;
using University.API.Controllers;
using University.Infrastructure;
using University.Infrastructure.Db;
using FacultyController = Faculty.API.Controllers.FacultyController;
using Notifications.Infrastructure.DependencyInjection;
using Analytics.Infrastructure.Db;
using Analytics.Infrastructure.Db.Seed;

// Npgsql/Postgres timestamp compatibility for local dev.
// Prevents failures when DateTime.Kind is Unspecified but the DB column is timestamptz.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


var builder = WebApplication.CreateBuilder(args);

// Add services from modules
builder.Services.AddCommonModule();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddSupportModule(builder.Configuration);
builder.Services.AddUniversityModule(builder.Configuration);
builder.Services.AddFacultyModule(builder.Configuration);
builder.Services.AddNotificationsModule(builder.Configuration);
builder.Services.AddAnalyticsModule(builder.Configuration);
builder.Services.AddEventBus();

// Add controllers and module API assemblies
var mvcBuilder = builder.Services.AddControllers();

var moduleControllers = new[]
{
    typeof(IdentityController).Assembly,
    typeof(SupportController).Assembly,
    typeof(UniversityController).Assembly,
    typeof(FacultyController).Assembly,
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
    // xml docs
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var xmlPath in xmlFiles)
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // bearer jwt - adds the lock + authorize button
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();
app.UseMiddleware<Application.GlobalExceptionHandlingMiddleware>();

// Put false if you dont want to apply migrations on start
var applyMigrations = true;

if (applyMigrations)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var tenantScope = services.GetRequiredService<IScopedTenantContext>();
        using (tenantScope.Use(SeedConstants.FacultyId))
        {
            // Identity module
            try
            {
                var identityDb = services.GetRequiredService<AuthDbContext>();
                identityDb.Database.Migrate();
                if (app.Environment.IsDevelopment())
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var identitySeeder = services.GetRequiredService<IdentityDbContextSeed>();
                    await identitySeeder.SeedAsync(
                        identityDb,
                        userManager,
                        SeedConstants.SuperAdminUserId,
                        SeedConstants.AdminUserId,
                        SeedConstants.TeacherUserId,
                        SeedConstants.StudentUserId
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error migrating IdentityDbContext: {ex.Message}");
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

            // University module
            try
            {
                var universityDb = services.GetRequiredService<UniversityDbContext>();
                universityDb.Database.Migrate();
                var universitySeeder = services.GetRequiredService<UniversityDbInitializier>();
                await universitySeeder.SeedAsync(
                    universityDb);
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
                if (app.Environment.IsDevelopment())
                {
                    var facultySeeder = services.GetRequiredService<FacultyDbContextSeed>();
                    await facultySeeder.SeedAsync(
                        facultyDb,
                        SeedConstants.TeacherUserId,
                        SeedConstants.StudentUserId
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error migrating FacultyDbContext: {ex.Message}");
            }

            // Analytics module
            try
            {
                var analyticsDb = services.GetRequiredService<AnalyticsDbContext>();
                analyticsDb.Database.Migrate();
                var analyticSeeder = services.GetRequiredService<AnalyticsDbInitializer>();
                await analyticSeeder.SeedAsync(
                    analyticsDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error migrating AnalyticsDbContext: {ex.Message}");
            }

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

if (app.Environment.IsDevelopment())
{
    app.MapGet(
        "/__routes",
        (Microsoft.AspNetCore.Routing.EndpointDataSource ds) =>
        {
            var routes = ds
                .Endpoints.OfType<RouteEndpoint>()
                .Select(e =>
                {
                    var methods = e
                        .Metadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                        .FirstOrDefault()
                        ?.HttpMethods;

                    return new
                    {
                        pattern = e.RoutePattern.RawText,
                        methods = methods?.ToArray() ?? Array.Empty<string>(),
                        displayName = e.DisplayName,
                    };
                })
                .OrderBy(r => r.pattern)
                .ToList();

            return Results.Ok(routes);
        }
    );
}

app.Run();
