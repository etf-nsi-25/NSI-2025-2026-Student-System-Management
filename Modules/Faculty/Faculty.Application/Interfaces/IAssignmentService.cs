using AutoMapper.Configuration.Annotations;
using Faculty.Application.DTOs;
using Faculty.Core.Shared;

namespace Faculty.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<Response> CreateAssignment(CreateAssignmentDTO assignmentDTO, Guid userID);
        Task<Response> DeleteAssignment(int assignmentID);
        Task<PaginatedDTO<AssignmentDTO>> GetAssignmentsByUserId(Guid userID, string? query, int pageSize, int pageNumber);
        Task<Response> UpdateAssignment(int assignmentID, CreateAssignmentDTO assignmentDTO, Guid userID);
    }
}
