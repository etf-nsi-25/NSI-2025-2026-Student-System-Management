using System.Threading.Tasks;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Repositories
{
    public class FacultyRepository : BaseRepository<Faculty>, IFacultyRepository
    {
        private new readonly UniversityDbContext _context;

        public FacultyRepository(UniversityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Faculty?> GetFacultyByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Code == code);
        }
    }
}
