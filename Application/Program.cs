// Import module DI namespaces
using University.Infrastructure;
using Support.Infrastructure;
using Notifications.Infrastructure;
using Analytics.Infrastructure;
using Identity.Infrastructure.DependencyInjection;
using Faculty.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Support.Infrastructure.Db;
using University.Infrastructure.Db;
using Identity.Infrastructure.Db;
using Faculty.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "ReactDevClient";

// Add services from modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddUniversityModule(builder.Configuration);
builder.Services.AddFacultyModule(builder.Configuration);
builder.Services.AddSupportModule(builder.Configuration);
builder.Services.AddNotificationsModule();
builder.Services.AddAnalyticsModule();

// Add controllers and module API assemblies
var mvcBuilder = builder.Services.AddControllers();

var moduleControllers = new[]
{
    typeof(Identity.API.Controllers.IdentityController).Assembly,
    typeof(University.API.Controllers.UniversityController).Assembly,
    typeof(Faculty.API.Controllers.FacultyController).Assembly,
    typeof(Support.API.Controllers.SupportController).Assembly,
    typeof(Notifications.API.Controllers.NotificationsController).Assembly,
    typeof(Analytics.API.Controllers.AnalyticsController).Assembly
};

foreach (var asm in moduleControllers)
{
    mvcBuilder.PartManager.ApplicationParts.Add(new Microsoft.AspNetCore.Mvc.ApplicationParts.AssemblyPart(asm));
}

builder.Services.AddCors(options =>
{
	options.AddPolicy(CorsPolicyName, policy =>
	{
		policy
			.WithOrigins("http://localhost:5173")  
			.AllowAnyHeader()
			.AllowAnyMethod()
		    .AllowCredentials();   
	});
});

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

// CORS Configuration for aggregated host - allow frontend dev server
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
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

    // Notifications module - still no migrations present so commenting this code for now
    //try
    //{
    //var notificationsDb = services.GetRequiredService<NotificationsDbContext>();
    //notificationsDb.Database.Migrate();
    //}
    //catch (Exception ex)
    //{
    // Console.WriteLine($"Error migrating NotificationsDbContext: {ex.Message}");
    //}

    // Analytics module - still no migrations present so commenting this code for now
    //try
    //{
    // var analyticsDb = services.GetRequiredService<AnalyticsDbContext>();
    // analyticsDb.Database.Migrate();
    //}
    //catch (Exception ex)
    //{
    // Console.WriteLine($"Error migrating AnalyticsDbContext: {ex.Message}");
    //}
    }

}


// Middleware
app.UseHttpsRedirection();

// Ensure routing is enabled before applying CORS so the middleware can handle preflight requests correctly
app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

// Map controllers
app.MapControllers();

app.Run();
