using Support.Core.Entities;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.Db.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(SupportDbContext context) : base(context)
        {
        }
    }
}
