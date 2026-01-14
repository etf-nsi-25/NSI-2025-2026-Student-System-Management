using System.Text.Json.Nodes;
using Analytics.Core.Entities;

namespace Analytics.Application.Interfaces;

public interface IStatsService
{
    Task<JsonObject> GetOrUpdateStatAsync(string metricCode, Scope scope, Guid scopeIdentifier, bool forceRefresh = false);
}
