using Analytics.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.AccessControl;
using Common.Core.Interfaces.Repsitories;


namespace Analytics.Core.Interfaces;

public interface IStatRepository : IBaseRepository<Stat>
{
    Task<IEnumerable<Stat>> GetStatsByMetricAndScopeAsync(string metricCode, Scope scope, Guid scopeIdentifier);
}
