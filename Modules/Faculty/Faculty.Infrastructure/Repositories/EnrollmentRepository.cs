using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly FacultyDbContext _context;
        private readonly ITenantService _tenantService;

        public EnrollmentRepository(FacultyDbContext context, ITenantService tenantService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        }

        public async Task<Student?> GetStudentByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var studentSchema = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

            return studentSchema == null
                ? null
                : StudentMapper.ToDomain(studentSchema, includeRelationships: false);
        }

        public async Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var courseSchema = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            return courseSchema == null
                ? null
                : CourseMapper.ToDomain(courseSchema, includeRelationships: false);
        }

        public Task<bool> IsAlreadyEnrolledAsync(int studentId, Guid courseId, CancellationToken cancellationToken = default)
            => _context.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
        {
            var schema = EnrollmentMapper.ToPersistence(enrollment);

            schema.FacultyId = _tenantService.GetCurrentFacultyId();
            schema.CreatedAt = DateTime.UtcNow;

            _context.Enrollments.Add(schema);
            await _context.SaveChangesAsync(cancellationToken);

            return EnrollmentMapper.ToDomain(schema, includeRelationships: false);
        }

        public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .ToListAsync(cancellationToken);

            return EnrollmentMapper.ToDomainCollection(schemas, includeRelationships: true).ToList();
        }
    }
}
