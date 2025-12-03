using System.Threading.Tasks;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Repositories
{
    public class AcademicYearRepository : BaseRepository<AcademicYear>, IAcademicYearRepository
    {
        private new readonly UniversityDbContext _context;

        public AcademicYearRepository(UniversityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AcademicYear?> GetActiveAcademicYearAsync()
        {
            return await _context.AcademicYears
                .AsNoTracking()
                .FirstOrDefaultAsync(ay => ay.IsActive);
        }
    }
}
