using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;

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

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.Code == code);

            if (faculty == null)
                return null;

            return new Faculty
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Address = faculty.Address,
                Code = faculty.Code,
                Description = faculty.Description,
                EstablishedDate = faculty.EstablishedDate,
                DeanId = faculty.DeanId,
            };
        }
    }
}
