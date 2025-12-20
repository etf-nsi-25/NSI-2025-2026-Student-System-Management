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
        Task<List<Exam>> GetAllAsync();
        Task<List<Exam>> GetExamsByProfessorAsync(int teacherId);
        Task<Exam?> UpdateAsync(Exam exam);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsProfessorAssignedToCourseAsync(int teacherId, Guid courseId);
    }
}