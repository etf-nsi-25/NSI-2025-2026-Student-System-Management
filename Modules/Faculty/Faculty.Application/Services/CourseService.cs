using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;


namespace Faculty.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;

        public CourseService(ICourseRepository repo)
        {
            _repo = repo;
        }

        private CourseDTO ToDto(Course c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            Type = c.Type.ToString(),
            ProgramId = c.ProgramId,
            ECTS = c.ECTS
        };

        private Course ToEntity(CourseDTO dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Code = dto.Code,
            Type = Enum.Parse<CourseType>(dto.Type, true),
            ProgramId = dto.ProgramId,
            ECTS = dto.ECTS
        };

        public async Task<CourseDTO> AddAsync(CourseDTO dto)
        {
            var entity = ToEntity(dto);
            var created = await _repo.AddAsync(entity);
            return ToDto(created);
        }

        public async Task<CourseDTO?> GetByIdAsync(Guid id)
        {
            var course = await _repo.GetByIdAsync(id);
            return course == null ? null : ToDto(course);
        }

        public async Task<List<CourseDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<List<CourseDTO>> GetByTeacherAsync(string userId)
        {
            var list = await _repo.GetByTeacherUserIdAsync(userId);
            return list.Select(ToDto).ToList();
        }

        public async Task<CourseDTO?> UpdateAsync(Guid id, CourseDTO dto)
        {
            dto.Id = id;
            var updated = await _repo.UpdateAsync(ToEntity(dto));
            return updated == null ? null : ToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
            => await _repo.DeleteAsync(id);
    }
}
