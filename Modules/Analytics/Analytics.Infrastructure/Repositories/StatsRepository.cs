using Analytics.Core.Entities;
using Analytics.Core.Interfaces;
using Analytics.Infrastructure.Db;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories
{
    public class StatsRepository : BaseRepository<Stat>, IStatRepository
    {
        private new readonly AnalyticsDbContext _context;

        public StatsRepository(AnalyticsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stat>> GetStatsByMetricAndScopeAsync(string metricCode, Scope scope, Guid scopeIdentifier )
        {
            return await _context.Set<Stat>()
            .AsNoTracking()
            .Where(s => s.MetricCode == metricCode &&
                        s.Scope == scope &&
                        s.ScopeIdentifier == scopeIdentifier)
            .ToListAsync();
        }
    }
}