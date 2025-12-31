using Analytics.API.Controllers;
using Analytics.Infrastructure;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.DependencyInjection;
using Identity.API.Controllers;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Notifications.API.Controllers;
using Notifications.Infrastructure;
using Support.API.Controllers;
using Support.Infrastructure;
using Support.Infrastructure.Db;
using University.API.Controllers;
using University.Infrastructure;
using University.Infrastructure.Db;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using FacultyController = Faculty.API.Controllers.FacultyController;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);




var jwt = builder.Configuration.GetSection("JwtSettings");
var signingKey = jwt["SigningKey"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(signingKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JWT AUTH FAILED: " + ctx.Exception);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(); // dodaj ovo (dobro je imati)









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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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



app.Use(async (ctx, next) =>
{
    await next();

    var ep = ctx.GetEndpoint();
    Console.WriteLine($"REQ {ctx.Request.Method} {ctx.Request.Path} -> {ctx.Response.StatusCode} | endpoint={(ep?.DisplayName ?? "NULL")}");
});



app.MapGet("/__routes", (Microsoft.AspNetCore.Routing.EndpointDataSource ds) =>
{
    var routes = ds.Endpoints
        .OfType<RouteEndpoint>()
        .Select(e =>
        {
            var methods = e.Metadata
                .OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                .FirstOrDefault()?.HttpMethods;

            return new
            {
                pattern = e.RoutePattern.RawText,
                methods = methods == null ? Array.Empty<string>() : methods.ToArray(),
                displayName = e.DisplayName
            };
        })
        .OrderBy(r => r.pattern)
        .ToList();

    return Results.Ok(routes);
});



app.Run();
