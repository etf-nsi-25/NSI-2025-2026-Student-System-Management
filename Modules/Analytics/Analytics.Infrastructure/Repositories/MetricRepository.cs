using Analytics.Core.Entities;
using Analytics.Core.Interfaces;
using Analytics.Infrastructure.Db;
using Common.Infrastructure.Repositories;

namespace Analytics.Infrastructure.Repositories
{
    public class MetricRepository : BaseRepository<Metric>, IMetricRepository
    {
        private new readonly AnalyticsDbContext _context;

        public MetricRepository(AnalyticsDbContext context) : base(context)
        {
            _context = context;
        }
    }
}