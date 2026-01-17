using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly FacultyDbContext _context;

        public TeacherRepository(FacultyDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetByUserIdAsync(string userId)
        {
            var persistence = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            return persistence == null ? null : TeacherMapper.ToDomain(persistence, includeRelationships: false);
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            var persistence = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == id);

            return persistence == null ? null : TeacherMapper.ToDomain(persistence, includeRelationships: false);
        }
    }
}