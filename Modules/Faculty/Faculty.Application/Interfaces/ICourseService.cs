using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDTO> AddAsync(CourseDTO course);
        Task<CourseDTO?> GetByIdAsync(Guid id);
        Task<List<CourseDTO>> GetAllAsync();
        Task<List<CourseDTO>> GetByTeacherAsync(string userId);
        Task<CourseDTO?> UpdateAsync(Guid id, CourseDTO course);
        Task<bool> DeleteAsync(Guid id);
        Task<TeacherDto?> GetTeacherForCourseAsync(Guid courseId);

        Task<bool> IsTeacherAssignedToCourse(int teacherID, Guid courseID);
        (List<AssignmentDTO>, int) GetAssignmentsAsync(int teacherID, string? query, int pageSize, int pageNumber);
    }
}
