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

        public async Task<ExamResponseDTO> CreateExamAsync(CreateExamRequestDTO request, int teacherId)
        {
            _logger.LogInformation("Creating exam for course {CourseId} by teacher {TeacherId}", request.CourseId, teacherId);

            // Validate that the teacher is assigned to the course
            var isAssigned = await _examRepository.IsTeacherAssignedToCourseAsync(teacherId, request.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} is not assigned to course {CourseId}", teacherId, request.CourseId);
                throw new UnauthorizedAccessException("You are not authorized to create exams for this course.");
            }

            // Check for date conflicts
            var hasConflict = await _examRepository.HasDateConflictAsync(request.CourseId, null, request.ExamDate, request.Location);
            if (hasConflict)
            {
                _logger.LogWarning("Date conflict detected for course {CourseId} at {Location} on {ExamDate}", request.CourseId, request.Location, request.ExamDate);
                throw new InvalidOperationException("There is already an exam scheduled for this course at the same location and date.");
            }

            var exam = new Exam
            {
                FacultyId = _tenantService.GetCurrentFacultyId(),
                CourseId = request.CourseId,
                Name = request.Name,
                Location = request.Location,
                ExamType = request.ExamType,
                ExamDate = request.ExamDate,
                RegDeadline = request.RegDeadline
            };

            var createdExam = await _examRepository.AddAsync(exam);
            _logger.LogInformation("Exam created with ID {ExamId}", createdExam.Id);

            return MapToResponse(createdExam);
        }

        public async Task<ExamResponseDTO?> GetExamByIdAsync(int id, int teacherId)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            // Check if the teacher is assigned to the course
            var isAssigned = await _examRepository.IsTeacherAssignedToCourseAsync(teacherId, exam.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} attempted to access exam {ExamId} for unauthorized course", teacherId, id);
                throw new UnauthorizedAccessException("You are not authorized to view this exam.");
            }

            return MapToResponse(exam);
        }

        public async Task<List<ExamResponseDTO>> GetExamsByTeacherAsync(int teacherId)
        {
            var exams = await _examRepository.GetExamsByTeacherAsync(teacherId);
            return exams.Select(MapToResponse).ToList();
        }

        public async Task<ExamResponseDTO?> UpdateExamAsync(int id, UpdateExamRequestDTO request, int teacherId)
        {
            _logger.LogInformation("Updating exam {ExamId} by teacher {TeacherId}", id, teacherId);

            var existingExam = await _examRepository.GetByIdAsync(id);
            if (existingExam == null)
                return null;

            // Check if the teacher is assigned to the course
            var isAssigned = await _examRepository.IsTeacherAssignedToCourseAsync(teacherId, existingExam.CourseId);
            if (!isAssigned)
            {
                _logger.LogWarning("Teacher {TeacherId} attempted to update exam {ExamId} for unauthorized course", teacherId, id);
                throw new UnauthorizedAccessException("You are not authorized to update this exam.");
            }

            // Check for date conflicts (exclude current exam)
            var hasConflict = await _examRepository.HasDateConflictAsync(request.CourseId, id, request.ExamDate, request.Location);
            if (hasConflict)
            {
                _logger.LogWarning("Date conflict detected for course {CourseId} at {Location} on {ExamDate} (excluding exam {ExamId})", request.CourseId, request.Location, request.ExamDate, id);
                throw new InvalidOperationException("There is already an exam scheduled for this course at the same location and date.");
            }

            existingExam.CourseId = request.CourseId;
            existingExam.Name = request.Name;
            existingExam.Location = request.Location;
            existingExam.ExamType = request.ExamType;
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
            var isAssigned = await _examRepository.IsTeacherAssignedToCourseAsync(teacherId, exam.CourseId);
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

        private ExamResponseDTO MapToResponse(Exam exam)
        {
            return new ExamResponseDTO
            {
                Id = exam.Id,
                CourseId = exam.CourseId,
                CourseName = exam.Course?.Name,
                Name = exam.Name,
                Location = exam.Location,
                ExamType = exam.ExamType,
                ExamDate = exam.ExamDate,
                RegDeadline = exam.RegDeadline,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt
            };
        }
    }
}