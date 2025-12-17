using Support.Core.Entities;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.Db.Repositories
{
    public class IssueRepository : BaseRepository<Issue>, IIssueRepository
    {
        public IssueRepository(SupportDbContext context) : base(context)
        {
        }
    }
}
