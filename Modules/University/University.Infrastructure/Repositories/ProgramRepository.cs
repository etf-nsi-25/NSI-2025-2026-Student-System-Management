using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;

namespace University.Infrastructure.Repositories
{
    public class ProgramRepository : BaseRepository<Program>, IProgramRepository
    {
        private new readonly UniversityDbContext _context;

        public ProgramRepository(UniversityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Program>> GetAllByDepartmentIdAsync(int departmentId)
        {
            var programs = await _context.Programs
                .Where(p => p.DepartmentId == departmentId)
                .ToListAsync();

            return programs.Select(p => new Program
            {
                Id = p.Id,
                DepartmentId = p.DepartmentId,
                Name = p.Name,
                Code = p.Code,
                DegreeType = p.DegreeType,
                DurationYears = p.DurationYears,
                Credits = p.Credits,
            });
        }
    }
}
