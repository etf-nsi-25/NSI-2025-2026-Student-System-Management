using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly FacultyDbContext _context;

        public ExamRepository(FacultyDbContext context)
        {
            _context = context;
        }

        public async Task<Exam> AddAsync(Exam exam)
        {
            exam.CreatedAt = DateTime.UtcNow;
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<List<Exam>> GetExamsByTeacherAsync(int teacherId)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Where(e => _context.CourseAssignments
                    .Any(ca => ca.TeacherId == teacherId && ca.CourseId == e.CourseId))
                .ToListAsync();
        }

        public async Task<Exam?> UpdateAsync(Exam exam)
        {
            var existing = await _context.Exams.FindAsync(exam.Id);
            if (existing == null)
                return null;

            existing.Name = exam.Name;
            existing.Location = exam.Location;
            existing.ExamType = exam.ExamType;
            existing.ExamDate = exam.ExamDate;
            existing.RegDeadline = exam.RegDeadline;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Exams.FindAsync(id);
            if (existing == null)
                return false;

            _context.Exams.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTeacherAssignedToCourseAsync(int teacherId, Guid courseId)
        {
            return await _context.CourseAssignments
                .AnyAsync(ca => ca.TeacherId == teacherId && ca.CourseId == courseId);
        }

        public async Task<bool> HasDateConflictAsync(Guid courseId, int? excludeExamId, DateTime examDate, string location)
        {
            var query = _context.Exams
                .Where(e => e.CourseId == courseId &&
                           e.ExamDate.HasValue &&
                           e.ExamDate.Value.Date == examDate.Date &&
                           e.Location == location);

            if (excludeExamId.HasValue)
            {
                query = query.Where(e => e.Id != excludeExamId.Value);
            }

            return await query.AnyAsync();
        }
    }
}