using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faculty.Application.DTOs;
using static System.Net.Mime.MediaTypeNames;


namespace Faculty.Application.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDto> CreateEnrollmentAsync(Guid courseId, string userId, CancellationToken cancellationToken = default);
    Task<List<StudentEnrollmentItemDto>> GetMyEnrollmentsAsync(string userId, CancellationToken cancellationToken = default);

}


