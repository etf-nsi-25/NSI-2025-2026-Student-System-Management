using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Repositories
{
    public class ProgramRepository : BaseRepository<Program>, IProgramRepository
    {
        private new readonly UniversityDbContext _context;

        public ProgramRepository(UniversityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Program>> GetAllByDepartmentIdAsync(Guid departmentId)
        {
            return await _context.Programs
                .AsNoTracking()
                .Where(p => p.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}
