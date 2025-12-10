using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support.Application.DTOs;

namespace Support.Application.Services
{
    public interface IRequestService
    {
        Task<CreateRequestResponseDto> CreateRequestAsync(CreateRequestDto dto);
    }
}