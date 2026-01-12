using University.Application.DTOs;
using University.Application.Interfaces;
using University.Core.Entities;

namespace University.Application.Services
{
    public class FacultyService : IFacultyService
    {
        // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
        private static List<Faculty> _dummyFaculties = new List<Faculty>
        {
            new Faculty { Id = 1, Name = "Faculty of Electrical Engineering", Address = "Zmaja od Bosne bb", Code = "ETF" },
            new Faculty { Id = 2, Name = "Faculty of Philosophy", Address = "Franje Račkog 1", Code = "FF" },
            new Faculty { Id = 3, Name = "Faculty of Economics", Address = "Trg oslobođenja - Alija Izetbegović 1", Code = "EFSA" }
        };
        private static int _nextId = 4;

        public Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync(string? nameFilter)
        {
            // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
            var faculties = _dummyFaculties.AsEnumerable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                faculties = faculties.Where(f => f.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            }


            var dtos = faculties.Select(f => new FacultyDto
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Code = f.Code
            });

            return Task.FromResult(dtos);
        }

        public Task<FacultyDto?> GetFacultyByIdAsync(int id)
        {
            // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
            var faculty = _dummyFaculties.FirstOrDefault(f => f.Id == id);
            if (faculty == null)
            {
                return Task.FromResult<FacultyDto?>(null);
            }

            var dto = new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name,
                Address = faculty.Address,
                Code = faculty.Code
            };
            return Task.FromResult<FacultyDto?>(dto);
        }

        public Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto dto)
        {
            // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
            if (_dummyFaculties.Any(f => f.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Faculty with the same name already exists.");
            }

            if (_dummyFaculties.Any(f => f.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Faculty with the same code already exists.");
            }

            var newFaculty = new Faculty
            {
                Id = _nextId++,
                Name = dto.Name,
                Address = dto.Address,
                Code = dto.Code
            };

            _dummyFaculties.Add(newFaculty);

            var facultyDto = new FacultyDto
            {
                Id = newFaculty.Id,
                Name = newFaculty.Name,
                Address = newFaculty.Address,
                Code = newFaculty.Code
            };

            return Task.FromResult(facultyDto);
        }

        public Task<FacultyDto?> UpdateFacultyAsync(int id, UpdateFacultyDto dto)
        {
            // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
            var faculty = _dummyFaculties.FirstOrDefault(f => f.Id == id);
            if (faculty == null)
            {
                return Task.FromResult<FacultyDto?>(null);
            }

            if (dto.Name != null && _dummyFaculties.Any(f => f.Id != id && f.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Faculty with the same name already exists.");
            }

            if (dto.Code != null && _dummyFaculties.Any(f => f.Id != id && f.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Faculty with the same code already exists.");
            }

            if (dto.Name != null)
            {
                faculty.Name = dto.Name;
            }
            if (dto.Address != null)
            {
                faculty.Address = dto.Address;
            }
            if (dto.Code != null)
            {
                faculty.Code = dto.Code;
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

        public Task<bool> DeleteFacultyAsync(int id)
        {
            // TODO: REMOVE DUMMY DATA AND REPLACE WITH REPOSITORY CALL ONCE DB CODE IS MERGED.
            var faculty = _dummyFaculties.FirstOrDefault(f => f.Id == id);
            if (faculty == null)
            {
                return Task.FromResult(false);
            }

            // TODO: Check if Faculty is used in Users or Courses tables. If yes, throw Conflict Exception.

            _dummyFaculties.Remove(faculty);
            return Task.FromResult(true);
        }
    }
}