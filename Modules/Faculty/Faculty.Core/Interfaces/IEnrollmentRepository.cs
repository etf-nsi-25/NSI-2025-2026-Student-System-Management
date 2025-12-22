

using Faculty.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Faculty.Core.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetByStudentIdAsync(int studentId);

        Task<bool> StudentExistsAsync(int studentId);

        Task<Course?> GetCourseAsync(Guid courseId);

        Task<bool> ExistsAsync(int studentId, Guid courseId);

        Task<Enrollment> AddAsync(Enrollment enrollment);
    }
}
