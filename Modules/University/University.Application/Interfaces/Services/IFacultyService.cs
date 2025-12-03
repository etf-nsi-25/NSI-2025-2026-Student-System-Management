using University.Application.DTOs;
using University.Core.Entities;

namespace University.Application.Interfaces.Services
{
    public interface IFacultyService
    {
        Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync();
        Task<FacultyDto?> GetFacultyByIdAsync(int id);
        Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto facultyDto);
        Task<FacultyDto?> UpdateFacultyAsync(int id, UpdateFacultyDto facultyDto);
        Task<bool> DeleteFacultyAsync(int id);
    }
}
