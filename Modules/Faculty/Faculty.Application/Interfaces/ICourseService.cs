using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDTO> AddAsync(CourseDTO course);
        Task<CourseDTO?> GetByIdAsync(Guid id);
        Task<List<CourseDTO>> GetAllAsync();
        Task<CourseDTO?> UpdateAsync(Guid id, CourseDTO course);
        Task<bool> DeleteAsync(Guid id);
    }
}
