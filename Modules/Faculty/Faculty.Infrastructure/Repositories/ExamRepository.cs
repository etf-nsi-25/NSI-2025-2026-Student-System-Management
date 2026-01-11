using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Faculty.Infrastructure.Schemas;
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
            var persistence = ExamMapper.ToPersistence(exam);
            _context.Exams.Add(persistence);
            await _context.SaveChangesAsync();

            return ExamMapper.ToDomain(persistence, includeRelationships: false);
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            var persistence = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            return persistence == null ? null : ExamMapper.ToDomain(persistence, includeRelationships: true);
        }

        public async Task<List<Exam>> GetExamsByTeacherAsync(int teacherId)
        {
            var persistence = await _context.Exams
                .Include(e => e.Course)
                .Where(e => _context.CourseAssignments
                    .Any(ca => ca.TeacherId == teacherId && ca.CourseId == e.CourseId))
                .ToListAsync();

            return ExamMapper.ToDomainCollection(persistence, includeRelationships: true).ToList();
        }

        public async Task<List<Exam>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds)
        {
            if (courseIds == null || courseIds.Count == 0)
            {
                return new List<Exam>();
            }

            var now = DateTime.UtcNow;

            var examSchemas = await _context.Exams
                .Include(e => e.Course)
                .Where(e => courseIds.Contains(e.CourseId) &&
                           e.ExamDate.HasValue &&
                           e.ExamDate.Value > now)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            return ExamMapper.ToDomainCollection(examSchemas, includeRelationships: true).ToList();
        }

        public async Task<Exam?> UpdateAsync(Exam exam)
        {
            var existing = await _context.Exams.FindAsync(exam.Id);
            if (existing == null)
                return null;

            exam.UpdatedAt = DateTime.UtcNow;
            ExamMapper.UpdatePersistence(existing, exam);

            await _context.SaveChangesAsync();
            return ExamMapper.ToDomain(existing, includeRelationships: false);
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