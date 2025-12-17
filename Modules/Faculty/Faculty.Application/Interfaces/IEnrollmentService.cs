using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces
{
    public interface IEnrollmentService
    {
        /// <summary>
        /// Returns enrollments for the currently authenticated student (from JWT).
        /// </summary>
        Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync();

        /// <summary>
        /// Validates and creates an enrollment for the currently authenticated student (from JWT).
        /// </summary>
        Task<CreateEnrollmentResponseDto> CreateEnrollmentAsync(CreateEnrollmentRequestDto dto);
    }
}
