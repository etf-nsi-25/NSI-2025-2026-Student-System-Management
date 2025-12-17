using Faculty.Application.Interfaces;
using Faculty.Application.Services;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Faculty.Core.Interfaces;
using Faculty.Core.Services;
using Faculty.Infrastructure.Repositories;
using Faculty.Infrastructure.DependencyInjection;
using Faculty.Infrastructure.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<FacultyDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("Database");
    options.UseNpgsql(conn);
});

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();


//builder.Services.AddScoped<ITenantService, HttpTenantService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddFacultyModule(builder.Configuration);

///DODANO ZBOG JWT JER LOGIN NE RADI SAMO ZA TESTIRANJE
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ITenantService>(_ =>
        new DevTenantServiceFAKE(Guid.Parse("641a4bfe-d83b-403d-be28-db9fa4130e5e")));
}
else
{
    builder.Services.AddScoped<ITenantService, HttpTenantService>();
}
///


var app = builder.Build();

// Global error handling middleware
app.UseMiddleware<Faculty.API.Middleware.ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
