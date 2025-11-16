
// Import module DI namespaces
using Identity.Infrastructure;
using University.Infrastructure;
using Faculty.Infrastructure;
using Support.Infrastructure;
using Notifications.Infrastructure;
using Analytics.Infrastructure;
using Identity.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services from modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddUniversityModule();
builder.Services.AddFacultyModule();
builder.Services.AddSupportModule();
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

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

// Map controllers
app.MapControllers();

app.Run();
