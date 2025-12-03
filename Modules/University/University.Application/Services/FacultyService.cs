using University.Application.DTOs;
using University.Application.Interfaces.Services;
using University.Core.Entities;

namespace University.Application.Services
{
    public class FacultyService : IFacultyService
    {
        private static List<Faculty> _faculties = new List<Faculty>
        {
            new Faculty { Id = 1, Name = "Faculty of Electrical Engineering", Address = "Zmaja od Bosne 33-35", Code = "ETF" },
            new Faculty { Id = 2, Name = "Faculty of Mechanical Engineering", Address = "Zmaja od Bosne 33-35", Code = "MAF" }
        };
        private static int _nextId = 3;

        public Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync()
        {
            var facultyDtos = _faculties.Select(f => new FacultyDto
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Code = f.Code
            });
            return Task.FromResult(facultyDtos.AsEnumerable());
        }

        public Task<FacultyDto?> GetFacultyByIdAsync(int id)
        {
            var faculty = _faculties.FirstOrDefault(f => f.Id == id);
            if (faculty == null)
            {
                return Task.FromResult<FacultyDto?>(null);
            }
            var facultyDto = new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Address = faculty.Address,
                Code = faculty.Code
            };
            return Task.FromResult<FacultyDto?>(facultyDto);
        }

        public Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto facultyDto)
        {
            var faculty = new Faculty
            {
                Id = _nextId++,
                Name = facultyDto.Name,
                Address = facultyDto.Address,
                Code = facultyDto.Code
            };
            _faculties.Add(faculty);
            var newFacultyDto = new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Address = faculty.Address,
                Code = faculty.Code
            };
            return Task.FromResult(newFacultyDto);
        }

        public Task<FacultyDto?> UpdateFacultyAsync(int id, UpdateFacultyDto facultyDto)
        {
            var existingFaculty = _faculties.FirstOrDefault(f => f.Id == id);
            if (existingFaculty == null)
            {
                return Task.FromResult<FacultyDto?>(null);
            }

            existingFaculty.Name = facultyDto.Name;
            existingFaculty.Address = facultyDto.Address;
            existingFaculty.Code = facultyDto.Code;

            var updatedFacultyDto = new FacultyDto
            {
                Id = existingFaculty.Id,
                Name = existingFaculty.Name,
                Address = existingFaculty.Address,
                Code = existingFaculty.Code
            };
            return Task.FromResult<FacultyDto?>(updatedFacultyDto);
        }

        public Task<bool> DeleteFacultyAsync(int id)
        {
            var facultyToRemove = _faculties.FirstOrDefault(f => f.Id == id);
            if (facultyToRemove == null)
            {
                return Task.FromResult(false);
            }
            _faculties.Remove(facultyToRemove);
            return Task.FromResult(true);
        }
    }
}
