using Support.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Support.Infrastructure.Db;
using Support.API.Controllers;
using Support.API.PdfPoc;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<PdfPocService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add our Support module
//builder.Services.AddSupportModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();