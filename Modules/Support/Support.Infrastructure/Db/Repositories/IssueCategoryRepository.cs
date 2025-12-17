using Support.Core.Entities;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.Db.Repositories
{
    public class IssueCategoryRepository : BaseRepository<IssueCategory>, IIssueCategoryRepository
    {
        public IssueCategoryRepository(SupportDbContext context) : base(context)
        {
        }
    }
}
