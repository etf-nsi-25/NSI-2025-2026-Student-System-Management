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

        public async Task<IEnumerable<Department>> GetAllByFacultyIdAsync(int facultyId)
        {
            var entities = await _context.Departments
                .Include(d => d.Faculty)
                .Where(d => d.FacultyId == facultyId)
                .ToListAsync();

            return entities.Select(d => new Department
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                HeadOfDepartment = d.HeadOfDepartmentId,
                Faculty = new Faculty
                {
                    Id = d.Faculty.Id,
                    Name = d.Faculty.Name,
                    Description = d.Faculty.Description,
                    Address = d.Faculty.Address,
                    Code = d.Faculty.Code,
                    EstablishedDate = d.Faculty.EstablishedDate,
                    DeanId = d.Faculty.DeanId
                },
            });
        }
    }
}
