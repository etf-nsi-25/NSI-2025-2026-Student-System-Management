using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly FacultyDbContext _context;
        private readonly ITenantService _tenantService;

        public CourseRepository(FacultyDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            var schema = CourseMapper.ToPersistence(course);

            schema.Id = Guid.NewGuid();
            schema.FacultyId = _tenantService.GetCurrentFacultyId();
            schema.CreatedAt = DateTime.UtcNow;

            _context.Courses.Add(schema);
            await _context.SaveChangesAsync(cancellationToken);

            return CourseMapper.ToDomain(schema, includeRelationships: false);
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var schema = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return schema == null
                ? null
                : CourseMapper.ToDomain(schema, includeRelationships: false);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Courses
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return CourseMapper.ToDomainCollection(schemas, includeRelationships: false).ToList();
        }

        public async Task<List<Course>> GetByTeacherUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

            if (teacher == null)
                return new List<Course>();

            var courseSchemas = await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Where(ca => ca.TeacherId == teacher.Id)
                .Select(ca => ca.Course)
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return CourseMapper.ToDomainCollection(courseSchemas, includeRelationships: false).ToList();
        }

        public async Task<Course?> UpdateAsync(Course course, CancellationToken cancellationToken = default)
        {
            var existingSchema = await _context.Courses
                .FirstOrDefaultAsync(x => x.Id == course.Id, cancellationToken);

            if (existingSchema == null)
                return null;

            CourseMapper.UpdatePersistence(existingSchema, course);
            existingSchema.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return CourseMapper.ToDomain(existingSchema, includeRelationships: false);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingSchema = await _context.Courses
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (existingSchema == null)
                return false;

            _context.Courses.Remove(existingSchema);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> IsTeacherAssignedToCourse(int teacherID, Guid courseID)
        {
            return (await _context.CourseAssignments.CountAsync(ca => ca.TeacherId == teacherID && ca.CourseId == courseID)) != 0;
        }

        public (List<Assignment>, int) GetAssignmentsAsync(int teacherID, string? query, int pageSize, int pageNumber)
        {
            var listOfCourses = _context.CourseAssignments.Where(ca => ca.TeacherId == teacherID).ToList();
            List<Assignment> assignmentList = new();

            for (int i = 0; i < listOfCourses.Count; i++)
            {
                assignmentList.AddRange(_context.Assignments.Where(a => listOfCourses[i].CourseId == a.CourseId).Include(a => a.Course).ToList() ?? []);
            }

            if(String.IsNullOrEmpty(query))
                assignmentList = assignmentList.Where(a => a.Name.Contains(query!)).ToList();
            var total = assignmentList.Count;

            assignmentList = assignmentList.OrderByDescending(a => a.DueDate).ThenBy(a => a.Name).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return (assignmentList, total);
        }
    }
}
