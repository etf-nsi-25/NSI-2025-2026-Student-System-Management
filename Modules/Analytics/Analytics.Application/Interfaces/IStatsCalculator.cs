using System.Text.Json.Nodes;
using Analytics.Core.Entities;

namespace Analytics.Application.Interfaces;

public interface IStatsCalculator
{
    public string MetricCode { get; }
    public Scope Scope { get; }

    Task<JsonObject> CalculateAsync(Guid scopeIdentifier);
}
