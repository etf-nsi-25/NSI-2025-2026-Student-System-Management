using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;

        private readonly ICourseAssignmentRepository _courseAssignmentRepo;

        public CourseService(ICourseRepository repo, ICourseAssignmentRepository courseAssignmentRepo)
        {
            _repo = repo;
            _courseAssignmentRepo = courseAssignmentRepo;
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


        public async Task<List<ProfessorCourseDTO>> GetProfessorCoursesAsync(string userId)
        {
            var coursesWithCount = await _repo.GetProfessorCoursesWithStudentCountAsync(userId);
            return coursesWithCount.Select(x => new ProfessorCourseDTO
            {
                Id = x.Course.Id,
                Name = x.Course.Name,
                Code = x.Course.Code,
                Status = x.StudentCount > 0 ? "Active" : "Inactive",
                StudentCount = x.StudentCount
            }).ToList();
        }

        public async Task<CourseDTO?> UpdateAsync(Guid id, CourseDTO dto)
        {
            dto.Id = id;
            var updated = await _repo.UpdateAsync(ToEntity(dto));
            return updated == null ? null : ToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
            => await _repo.DeleteAsync(id);

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
