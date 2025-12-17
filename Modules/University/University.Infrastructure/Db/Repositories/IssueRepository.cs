using University.Core.Entities;
using University.Core.Interfaces.Repositories;
using University.Infrastructure.Db;

namespace University.Infrastructure.Db.Repositories
{
    public class IssueRepository : BaseRepository<Issue>, IIssueRepository
    {
        public IssueRepository(UniversityDbContext context) : base(context)
        {
        }
    }
}
