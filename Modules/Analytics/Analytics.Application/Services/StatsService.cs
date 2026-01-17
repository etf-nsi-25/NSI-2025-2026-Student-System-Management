using System.Text.Json.Nodes;
using Analytics.Application.Interfaces;
using Analytics.Core.Entities;
using Analytics.Core.Interfaces;

namespace Analytics.Application.Services;

public class StatsService : IStatsService
{
    private readonly IStatRepository _statRepository;
    private readonly IEnumerable<IStatsCalculator> _calculators;

    public StatsService(IStatRepository statRepository, IEnumerable<IStatsCalculator> calculators)
    {
        _statRepository = statRepository;
        _calculators = calculators;
    }

    public async Task<JsonObject> GetOrUpdateStatAsync(string metricCode, Scope scope, Guid scopeIdentifier, bool forceRefresh = false)
    {
        if (!_calculators.Any())
        {
            throw new Exception("No calculators were registered");
        }

        var calculator = _calculators.FirstOrDefault(c => c.MetricCode == metricCode && c.Scope == scope);

        if (calculator == null)
        {
            throw new Exception("No calculator defined");
        }

        var stats = await _statRepository.GetStatsByMetricAndScopeAsync(metricCode, scope, scopeIdentifier);
        var stat = stats.FirstOrDefault();

        
        if (stat != null && !forceRefresh)
        {
            var node = JsonNode.Parse(stat.Value);
            if (node is JsonObject jsonObject)
            {
                return jsonObject;
            }
        }

        var newValue = await calculator.CalculateAsync(scopeIdentifier);
        
        if (stat == null)
        {
            stat = new Stat
            {
                Id = Guid.NewGuid(),
                MetricCode = metricCode,
                Scope = scope,
                ScopeIdentifier = scopeIdentifier,
                Value = newValue.ToJsonString(),
                RecordedAt = DateTime.UtcNow
            };
            await _statRepository.AddAsync(stat);
        }
        else
        {
            stat.Value = newValue.ToJsonString();
            stat.RecordedAt = DateTime.UtcNow;
            await _statRepository.UpdateAsync(stat);
        }

        return newValue;
    }
}
