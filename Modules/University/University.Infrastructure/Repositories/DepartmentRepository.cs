using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;

namespace University.Infrastructure.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private new readonly UniversityDbContext _context;

        public DepartmentRepository(UniversityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllByFacultyIdAsync(Guid facultyId)
        {
            var entities = await _context.Departments
                .AsNoTracking()
                .Where(d => d.FacultyId == facultyId)
                .ToListAsync();

            return entities.Select(d => new Department
            {
                Id = d.Id,
                FacultyId = d.FacultyId,
                Name = d.Name,
                Code = d.Code,
                HeadOfDepartment = d.HeadOfDepartmentId,
            });
        }
    }
}

