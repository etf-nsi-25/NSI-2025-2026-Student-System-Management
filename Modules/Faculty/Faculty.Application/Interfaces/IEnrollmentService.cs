using Faculty.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

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
