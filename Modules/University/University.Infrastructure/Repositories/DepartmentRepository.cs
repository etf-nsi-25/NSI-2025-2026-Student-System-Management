using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Infrastructure.Entities;

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
            return await _context.Departments
                .AsNoTracking()
                .Where(d => d.FacultyId == facultyId)
                .ToListAsync();
        }
    }
}

