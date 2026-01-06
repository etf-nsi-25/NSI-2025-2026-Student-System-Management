using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Core.Interfaces;

namespace Support.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _repo;

        public RequestService(IRequestRepository repo)
        {
            _repo = repo;
        }

        public async Task<CreateRequestResponseDto> CreateRequestAsync(CreateRequestDto dto)
        {
            var allowed = new[] { "Transcript", "Certificate", "Enrollment" };

            if (!allowed.Contains(dto.RequestType))
                throw new ArgumentException("Invalid request type");

            if (dto.StudentId <= 0)
                throw new ArgumentException("Invalid student id");

            var request = new DocumentRequest
            {
                UserId = dto.StudentId.ToString(),   
                FacultyId = dto.FacultyId,           
                DocumentType = dto.RequestType,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };


            await _repo.CreateAsync(request);

            return new CreateRequestResponseDto
            {
                Id = request.Id,
                Success = true,
                Message = "Request created successfully"
            };
        }
    }
}
