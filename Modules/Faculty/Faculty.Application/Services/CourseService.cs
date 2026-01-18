using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;
using AutoMapper;

namespace Faculty.Application.Services
{
    public class CourseService(ICourseRepository repo, ICourseAssignmentRepository _courseAssignmentRepo, IMapper _mapper) : ICourseService
    {
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
            var created = await repo.AddAsync(entity);
            return ToDto(created);
        }

        public async Task<CourseDTO?> GetByIdAsync(Guid id)
        {
            var course = await repo.GetByIdAsync(id);
            return course == null ? null : ToDto(course);
        }

        public async Task<List<CourseDTO>> GetAllAsync()
        {
            var list = await repo.GetAllAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<List<CourseDTO>> GetByTeacherAsync(string userId)
        {
            var list = await repo.GetByTeacherUserIdAsync(userId);
            return list.Select(ToDto).ToList();
        }

        public async Task<CourseDTO?> UpdateAsync(Guid id, CourseDTO dto)
        {
            dto.Id = id;
            var updated = await repo.UpdateAsync(ToEntity(dto));
            return updated == null ? null : ToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
            => await repo.DeleteAsync(id);

        public async Task<bool> IsTeacherAssignedToCourse(int teacherID, Guid courseID)
        {
            return await repo.IsTeacherAssignedToCourse(teacherID, courseID);
        }

        public (List<AssignmentDTO>, int) GetAssignmentsAsync(int teacherID, string? query, int pageSize, int pageNumber)
        {
            var (assignments, count) = repo.GetAssignmentsAsync(teacherID, query, pageSize, pageNumber);
            return (_mapper.Map<List<AssignmentDTO>>(assignments), count);
        }

        public async Task<TeacherDto?> GetTeacherForCourseAsync(Guid courseId)
        {
            var teacher = await _courseAssignmentRepo.GetTeacherForCourseAsync(courseId);
            if (teacher == null) return null;

            return new TeacherDto
            {
                Id = teacher.Id,
                FullName = $"{teacher.FirstName} {teacher.LastName}"
            };
        }
    }
}
