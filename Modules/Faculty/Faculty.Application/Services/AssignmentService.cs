using AutoMapper;
using Common.Core.Interfaces.Repsitories;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Shared;
using Faculty.Infrastructure.Http;
using Microsoft.AspNetCore.Http;

namespace Faculty.Application.Services
{
    public class AssignmentService(IBaseRepository<Assignment> _assignmentRepository, ICourseService _courseService, ITeacherService _teacherService, ITenantService _tenantService, IMapper _mapper) : IAssignmentService
    {
        public async Task<Response> CreateAssignment(CreateAssignmentDTO assignmentDTO, Guid userID)
        {
            var assignment = _mapper.Map<Assignment>(assignmentDTO);

            assignment.FacultyId = _tenantService.GetCurrentFacultyId();
            assignment.CreatedAt = DateTime.UtcNow;

            var validationResult = (await ValidateAssignment(assignment, userID));
            if(validationResult != null)
            {
                return new Response(StatusCodes.Status400BadRequest, validationResult, "BadRequest");
            }

            await _assignmentRepository.AddAsync(assignment);

            return new Response(StatusCodes.Status200OK, "Assignment created!");
        }

        public async Task<Response> DeleteAssignment(int assignmentID)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentID);

            if (assignment == null)
                return new Response(StatusCodes.Status400BadRequest, "Bad Request", $"Assignment with id {assignmentID} does not exist.");

            assignment.IsDeleted = true;
            await _assignmentRepository.UpdateAsync(assignment);
            return new Response(StatusCodes.Status200OK, "Assignment deleted!");
        }

        public async Task<PaginatedDTO<AssignmentDTO>> GetAssignmentsByUserId(Guid userID, string? query, int pageSize, int pageNumber)
        {
            var teacherID = await _teacherService.GetTeacherIDByUserID(userID);
            if (teacherID == null || teacherID == 0)
                throw new ArgumentException($"Teacher with UserId {userID} does not exist.");

            var (assignments, total) = _courseService.GetAssignmentsAsync(teacherID ?? 0, query, pageSize, pageNumber);
            var list = _mapper.Map<List<AssignmentDTO>>(assignments);

            var result = new PaginatedDTO<AssignmentDTO>
            {
                Page = pageNumber,
                Total = total,
                PageSize = pageSize,
                Data = list,
                HasMore = pageSize * pageNumber < total,
            };

            result.Page = pageNumber;
            result.PageSize = pageSize;
            result.Data = list;
            return result;
        }

        public async Task<Response> UpdateAssignment(int assignmentID, CreateAssignmentDTO assignmentDTO, Guid userID)
        {
            var assignment = _mapper.Map<Assignment>(assignmentDTO);

            assignment.Id = assignmentID;
            assignment.FacultyId = _tenantService.GetCurrentFacultyId();
            assignment.UpdatedAt = DateTime.UtcNow;

            var validationResult = await ValidateAssignment(assignment, userID);
            if (validationResult != null)
            {
                return new Response(StatusCodes.Status400BadRequest, validationResult, "BadRequest");
            }

            await _assignmentRepository.UpdateAsync(assignment);

            return new Response(StatusCodes.Status200OK, "Assignment updated!");
        }

        private async Task<string?> ValidateAssignment(Assignment assignment, Guid userID)
        {
            if (assignment == null) return "No object provided!";

            if (assignment.DueDate < DateTime.UtcNow) return "Due date can not be in past.";

            if (await _courseService.GetByIdAsync(assignment.CourseId) == null)
                return $"Course with Id {assignment.CourseId} does not exist.";

            var teacherID = await _teacherService.GetTeacherIDByUserID(userID);
            if(teacherID == null || teacherID == 0)
                return $"Teacher with UserId {userID} does not exist.";

            if (!await _courseService.IsTeacherAssignedToCourse(teacherID ?? 0, assignment.CourseId))
                return $"Teacher with Id {teacherID} does not have permissions for course with Id {assignment.CourseId}.";
            
            return null;
        }
    }
}
