using Analytics.Application.Calculators;
using Analytics.Application.Interfaces;
using Analytics.Application.Services;
using Analytics.Core.Interfaces;
using Analytics.Infrastructure.Db;
using Analytics.Infrastructure.Db.Seed;
using Analytics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AnalyticsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddScoped<IStatRepository, StatsRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<AnalyticsDbInitializer>();

        services.AddScoped<IStatsService, StatsService>();

        services.Scan(scan => scan
            .FromAssemblyOf<StudentCountCalculator>()
            .AddClasses(classes => classes.AssignableTo<IStatsCalculator>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}

