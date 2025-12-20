using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Faculty.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly ITenantService _tenantService;
        private readonly ILogger<ExamService> _logger;

        public ExamService(
            IExamRepository examRepository,
            ITenantService tenantService,
            ILogger<ExamService> _logger)
        {
            _examRepository = examRepository;
            _tenantService = tenantService;
            this._logger = _logger;
        }

        public async Task<ExamResponse> CreateExamAsync(CreateExamRequest request, int teacherId)
        {
            _logger.LogInformation("Creating exam for course {CourseId} by teacher {TeacherId}", request.CourseId, teacherId);

            // Validate that the teacher is assigned to the course
            var isAssigned = await _examRepository.IsProfessorAssignedToCourseAsync(teacherId, request.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} is not assigned to course {CourseId}", teacherId, request.CourseId);
                throw new UnauthorizedAccessException("You are not authorized to create exams for this course.");
            }

            var exam = new Exam
            {
                FacultyId = _tenantService.GetCurrentFacultyId(),
                CourseId = request.CourseId,
                Name = request.Name,
                ExamDate = request.ExamDate,
                RegDeadline = request.RegDeadline
            };

            var createdExam = await _examRepository.AddAsync(exam);
            _logger.LogInformation("Exam created with ID {ExamId}", createdExam.Id);

            return MapToResponse(createdExam);
        }

        public async Task<ExamResponse?> GetExamByIdAsync(int id, int teacherId)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            // Check if the teacher is assigned to the course
            var isAssigned = await _examRepository.IsProfessorAssignedToCourseAsync(teacherId, exam.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} attempted to access exam {ExamId} for unauthorized course", teacherId, id);
                throw new UnauthorizedAccessException("You are not authorized to view this exam.");
            }

            return MapToResponse(exam);
        }

        public async Task<List<ExamResponse>> GetExamsByProfessorAsync(int teacherId)
        {
            var exams = await _examRepository.GetExamsByProfessorAsync(teacherId);
            return exams.Select(MapToResponse).ToList();
        }

        public async Task<ExamResponse?> UpdateExamAsync(int id, UpdateExamRequest request, int teacherId)
        {
            _logger.LogInformation("Updating exam {ExamId} by teacher {TeacherId}", id, teacherId);

            var existingExam = await _examRepository.GetByIdAsync(id);
            if (existingExam == null)
                return null;

            // Check if the teacher is assigned to the course
            var isAssigned = await _examRepository.IsProfessorAssignedToCourseAsync(teacherId, existingExam.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} attempted to update exam {ExamId} for unauthorized course", teacherId, id);
                throw new UnauthorizedAccessException("You are not authorized to update this exam.");
            }

            existingExam.CourseId = request.CourseId;
            existingExam.Name = request.Name;
            existingExam.ExamDate = request.ExamDate;
            existingExam.RegDeadline = request.RegDeadline;

            var updatedExam = await _examRepository.UpdateAsync(existingExam);
            if (updatedExam == null)
                return null;

            _logger.LogInformation("Exam {ExamId} updated successfully", id);
            return MapToResponse(updatedExam);
        }

        public async Task<bool> DeleteExamAsync(int id, int teacherId)
        {
            _logger.LogInformation("Deleting exam {ExamId} by teacher {TeacherId}", id, teacherId);

            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            // Check if the teacher is assigned to the course
            var isAssigned = await _examRepository.IsProfessorAssignedToCourseAsync(teacherId, exam.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} attempted to delete exam {ExamId} for unauthorized course", teacherId, id);
                throw new UnauthorizedAccessException("You are not authorized to delete this exam.");
            }

            var result = await _examRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Exam {ExamId} deleted successfully", id);
            }

            return result;
        }

        private ExamResponse MapToResponse(Exam exam)
        {
            return new ExamResponse
            {
                Id = exam.Id,
                CourseId = exam.CourseId,
                CourseName = exam.Course?.Name,
                Name = exam.Name,
                ExamDate = exam.ExamDate,
                RegDeadline = exam.RegDeadline,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt
            };
        }
    }
}