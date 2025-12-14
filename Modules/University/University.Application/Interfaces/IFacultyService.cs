using System.Collections.Generic;
using System.Threading.Tasks;
using University.Application.DTOs;

namespace University.Application.Interfaces
{
    public interface IFacultyService
    {
        Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync(string? nameFilter);
        Task<FacultyDto?> GetFacultyByIdAsync(int id);
        Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto dto);
        Task<FacultyDto?> UpdateFacultyAsync(int id, UpdateFacultyDto dto);
        Task<bool> DeleteFacultyAsync(int id);
    }
}