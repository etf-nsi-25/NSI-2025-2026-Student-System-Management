using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamResponse> CreateExamAsync(CreateExamRequest request, int teacherId);
        Task<ExamResponse?> GetExamByIdAsync(int id, int teacherId);
        Task<List<ExamResponse>> GetExamsByProfessorAsync(int teacherId);
        Task<ExamResponse?> UpdateExamAsync(int id, UpdateExamRequest request, int teacherId);
        Task<bool> DeleteExamAsync(int id, int teacherId);
    }
}