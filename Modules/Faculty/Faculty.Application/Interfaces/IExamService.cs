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
        Task<ExamResponseDTO> CreateExamAsync(CreateExamRequestDTO request, int teacherId, Guid facultyId);
        Task<ExamResponseDTO?> GetExamByIdAsync(int id, int teacherId);
        Task<List<ExamResponseDTO>> GetExamsByTeacherAsync(int teacherId);
        Task<ExamResponseDTO?> UpdateExamAsync(int id, UpdateExamRequestDTO request, int teacherId);
        Task<bool> DeleteExamAsync(int id, int teacherId);
    }
}