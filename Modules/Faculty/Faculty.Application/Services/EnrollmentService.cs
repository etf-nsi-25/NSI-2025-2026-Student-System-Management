using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;



namespace Faculty.Application.Services
{
        public class EnrollmentService : IEnrollmentService
        {
            private readonly FacultyDbContext _db;
            private readonly IHttpContextAccessor _http;

            public EnrollmentService(FacultyDbContext db, IHttpContextAccessor http)
            {
                _db = db;
                _http = http;
            }

        /* private int GetStudentIdFromJwt()
         {
             var user = _http.HttpContext?.User;
             if (user is null) throw new UnauthorizedAccessException("No user context.");

             var idStr =
                 user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? user.FindFirst("sub")?.Value
                 ?? user.FindFirst("studentId")?.Value;

             if (!int.TryParse(idStr, out var studentId))
                 throw new UnauthorizedAccessException("StudentId claim missing/invalid in JWT.");

             return studentId;
         }  */
        private int GetStudentIdFromJwt()
        {
            return 1; //FOR TESTING PIRPOSES SINCE LOGIN IS NOT WORKING
        }



        public async Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync()
            {
                var studentId = GetStudentIdFromJwt();

                return await _db.Enrollments
                    .AsNoTracking()
                    .Where(e => e.StudentId == studentId)
                    .Select(e => new EnrollmentListItemDto
                    {
                        EnrollmentId = e.Id,
                        CourseId = e.CourseId,
                        CourseName = e.Course.Name,   // koristi navigation property
                        Status = e.Status,
                        Grade = e.Grade
                    })
                    .ToListAsync();
            }

            public async Task<CreateEnrollmentResponseDto> CreateEnrollmentAsync(CreateEnrollmentRequestDto dto)
            {
                var studentId = GetStudentIdFromJwt();

                // 1) student postoji
                var studentExists = await _db.Students.AnyAsync(s => s.Id == studentId);
                if (!studentExists)
                    throw new KeyNotFoundException("Student not found.");

                // 2) course postoji
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
                if (course == null)
                    throw new KeyNotFoundException("Course not found.");

                // 3) nije već upisan
                var already = await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == dto.CourseId);
                if (already)
                    throw new InvalidOperationException("Already enrolled in this course");

            // 4) prerequisites (ovo ti je dio koji moraš spojiti na vaše tabele)
            // TODO: implementiraj real check (npr. CoursePrerequisites / Enrollment passed)
            // Ako nemaš još model, za sada preskoči ili stavi minimalno:
            // bool prereqOk = true;

            /* var enrollment = new Faculty.Core.Entities.Enrollment
             {
                 StudentId = studentId,
                 CourseId = dto.CourseId,
                 Status = "Enrolled",
                 Grade = null,
                 CreatedAt = DateTime.UtcNow
             };*/

            var enrollment = new Faculty.Core.Entities.Enrollment
            {
                FacultyId = Guid.Parse("641a4bfe-d83b-403d-be28-db9fa41330e5e"),
                StudentId = 1,
                CourseId = dto.CourseId,
                Status = "Enrolled",
                Grade = null,
                CreatedAt = DateTime.UtcNow
            };  //DODANO ZA TESTIRANJE JER LOGIN NE RADI


            _db.Enrollments.Add(enrollment);
                await _db.SaveChangesAsync();

                return new CreateEnrollmentResponseDto
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    CourseName = course.Name,
                    EnrollmentDate = enrollment.CreatedAt
                };
            }
        }
}




