using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Faculty.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        private readonly IHttpContextAccessor _http;

        public EnrollmentService(
            IEnrollmentRepository repo,
            IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        // =========================
        // JWT helper (OSTAJE)
        // =========================
        private int GetStudentIdFromJwt()
        {
            var user = _http.HttpContext?.User;
            if (user is null)
                throw new UnauthorizedAccessException("No user context.");

            var idStr =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? user.FindFirst("studentId")?.Value;

            if (!int.TryParse(idStr, out var studentId))
                throw new UnauthorizedAccessException("StudentId claim missing/invalid in JWT.");

            return studentId;
        }

        // =========================
        // GET MY ENROLLMENTS
        // =========================
        public async Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync()
        {
            var studentId = GetStudentIdFromJwt();

            var enrollments = await _repo.GetByStudentIdAsync(studentId);

            return enrollments.Select(e => new EnrollmentListItemDto
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                CourseName = e.Course.Name,
                Status = e.Status,
                Grade = e.Grade
            }).ToList();
        }

        // =========================
        // CREATE ENROLLMENT
        // =========================
        public async Task<CreateEnrollmentResponseDto> CreateEnrollmentAsync(
            CreateEnrollmentRequestDto dto)
        {
            var studentId = GetStudentIdFromJwt();

            // student exists
            if (!await _repo.StudentExistsAsync(studentId))
                throw new KeyNotFoundException("Student not found.");

            // course exists
            var course = await _repo.GetCourseAsync(dto.CourseId);
            if (course is null)
                throw new KeyNotFoundException("Course not found.");

            // not already enrolled
            if (await _repo.ExistsAsync(studentId, dto.CourseId))
                throw new InvalidOperationException("Already enrolled in this course.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                Status = "Enrolled",
                Grade = null,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(enrollment);

            return new CreateEnrollmentResponseDto
            {
                EnrollmentId = created.Id,
                StudentId = created.StudentId,
                CourseId = created.CourseId,
                CourseName = course.Name,
                EnrollmentDate = created.CreatedAt
            };
        }
    }
}
