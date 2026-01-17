
using System.Text.Json;
using Analytics.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Analytics.Infrastructure.Db.Seed;

public class AnalyticsDbInitializer
{

    public async Task SeedAsync(AnalyticsDbContext _context)
    {

        var path = Path.Combine(AppContext.BaseDirectory, "metrics.json");

        if (!File.Exists(path))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var seedData = JsonSerializer.Deserialize<List<MetricDto>>(json);

        if (seedData == null) return;

        var existingCodes = await _context.Metrics
            .AsNoTracking()
            .Select(m => m.Code)
            .ToListAsync();

        var existingSet = new HashSet<string>(existingCodes);

        var newMetrics = seedData
            .Where(m => !existingSet.Contains(m.Code))
            .Select(m => new Metric
            {
                Code = m.Code,
                Description = m.Description
            })
            .ToList();

        if (newMetrics.Any())
        {
            await _context.Metrics.AddRangeAsync(newMetrics);
            await _context.SaveChangesAsync();
        }
    }
    private class MetricDto
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
    }
}


