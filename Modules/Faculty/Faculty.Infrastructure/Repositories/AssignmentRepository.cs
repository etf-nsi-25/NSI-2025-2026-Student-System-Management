using Common.Infrastructure.Repositories;
using Faculty.Core.Entities;
using Faculty.Infrastructure.Db;

namespace Faculty.Infrastructure.Repositories
{
    public class AssignmentRepository(FacultyDbContext context) : BaseRepository<Assignment>(context)
    {
    }
}
