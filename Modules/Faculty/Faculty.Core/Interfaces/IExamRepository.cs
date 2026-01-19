using Faculty.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Interfaces
{
    public interface IExamRepository
    {
        Task<Exam> AddAsync(Exam exam);
        Task<Exam?> GetByIdAsync(int id);
        Task<List<Exam>> GetExamsByTeacherAsync(int teacherId);
        Task<List<Exam>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds);
        Task<Exam?> UpdateAsync(Exam exam);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsTeacherAssignedToCourseAsync(int teacherId, Guid courseId);
        Task<bool> HasDateConflictAsync(Guid courseId, int? excludeExamId, DateTime examDate, string location);
    }
}