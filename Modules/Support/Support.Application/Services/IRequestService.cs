using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support.Application.DTOs;

namespace Support.Core.Interfaces;

public interface IRequestService
{
    Task<CreateRequestResponseDto> CreateRequestAsync(
        Guid userId,
        CreateRequestDto dto);
}
