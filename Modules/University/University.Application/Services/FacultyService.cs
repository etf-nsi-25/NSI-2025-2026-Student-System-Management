using University.Application.DTOs;
using University.Application.Interfaces;
using University.Core.Entities;
using University.Core.Interfaces;

namespace University.Application.Services
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;

        public FacultyService(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }
        

        public async Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync(string? nameFilter)
        {
            var faculties = await _facultyRepository.GetAsync(f =>
                string.IsNullOrEmpty(nameFilter) || f.Name.ToLower().Contains(nameFilter.ToLower()));
     
            return faculties.Select(f => new FacultyDto
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Code = f.Code
            });
        }   

        public async Task<FacultyDto?> GetFacultyByIdAsync(Guid id)
        {
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null) return null;

            return new FacultyDto 
            { 
                Id = faculty.Id, 
                Name = faculty.Name, 
                Address = faculty.Address, 
                Code = faculty.Code 
            };
        }

        public async Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto dto)
        {
            if (await _facultyRepository.AnyAsync(f => f.Name == dto.Name))
                throw new ArgumentException("Faculty with the same name already exists.");

            if (await _facultyRepository.AnyAsync(f => f.Code == dto.Code))
                throw new ArgumentException("Faculty with the same code already exists.");

            var newFaculty = new Faculty
            {
                Name = dto.Name,
                Address = dto.Address,
                Code = dto.Code
            };

            await _facultyRepository.AddAsync(newFaculty);

            return new FacultyDto
            {
                Id = newFaculty.Id,
                Name = newFaculty.Name,
                Address = newFaculty.Address,
                Code = newFaculty.Code
            };
        }

        public async Task<FacultyDto?> UpdateFacultyAsync(Guid id, UpdateFacultyDto dto)
        {
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null) return null;

            if (dto.Name != null)
            {
                bool nameExists = await _facultyRepository.AnyAsync(f => f.Name == dto.Name && f.Id != id);
                if (nameExists) throw new ArgumentException("Faculty name already taken.");
                faculty.Name = dto.Name;
            }

            if (dto.Code != null)
            {
                bool codeExists = await _facultyRepository.AnyAsync(f => f.Code == dto.Code && f.Id != id);
                if (codeExists) throw new ArgumentException("Faculty code already taken.");
                faculty.Code = dto.Code;
            }

            if (dto.Address != null) faculty.Address = dto.Address;

            await _facultyRepository.UpdateAsync(faculty);

            return new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Address = faculty.Address,
                Code = faculty.Code
            };
            
        }

        public async Task<bool> DeleteFacultyAsync(Guid id)
        {
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null) return false;

            await _facultyRepository.DeleteAsync(faculty);
            return true;
            
        }
    }
}