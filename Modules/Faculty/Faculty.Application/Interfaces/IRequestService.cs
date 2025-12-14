using Faculty.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Faculty.Application.Interfaces
{
    public interface IRequestService
    {

        Task<IEnumerable<StudentRequestDto>> GetAllRequestsAsync();

        Task<StudentRequestDto> ProcessRequestAsync(Guid requestId, CreateConfirmationRequest request);
    }
}