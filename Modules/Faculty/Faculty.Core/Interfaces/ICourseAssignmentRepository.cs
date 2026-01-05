using System;
using System.Threading.Tasks;
using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces
{
    public interface ICourseAssignmentRepository
    {
        Task<Teacher?> GetTeacherForCourseAsync(Guid courseId);
    }
}
