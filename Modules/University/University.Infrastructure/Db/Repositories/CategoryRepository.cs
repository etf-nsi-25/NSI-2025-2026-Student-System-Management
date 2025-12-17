using University.Core.Entities;
using University.Core.Interfaces.Repositories;
using University.Infrastructure.Db;

namespace University.Infrastructure.Db.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(UniversityDbContext context) : base(context)
        {
        }
    }
}
