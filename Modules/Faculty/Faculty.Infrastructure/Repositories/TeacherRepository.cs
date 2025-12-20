using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
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
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}