using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Core.Interfaces;

namespace Support.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _repo;

    public RequestService(IRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<CreateRequestResponseDto> CreateRequestAsync(
        Guid userId,
        CreateRequestDto dto)
    {
        var request = new DocumentRequest
{
    Id = Guid.NewGuid(),
     UserId = userId,
    FacultyId = dto.FacultyId,
    DocumentType = dto.DocumentType,
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
