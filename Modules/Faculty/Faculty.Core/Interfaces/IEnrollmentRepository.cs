using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IEnrollmentRepository
{
    Task<Student?> GetStudentByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> IsAlreadyEnrolledAsync(int studentId, Guid courseId, CancellationToken cancellationToken = default);

    Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

}

