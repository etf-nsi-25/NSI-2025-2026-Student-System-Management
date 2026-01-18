namespace Analytics.Application.Interfaces;
using Analytics.Application.DTO;

public interface ITeacherAnalyticsService
{
    
    Task<TeacherFilterDataDto> GetTeacherFilterDataAsync(int teacherId);
    
    Task<TeacherFilterDataDto> GetTeacherFilterDataByUserIdAsync(Guid userId);
    Task<TeacherFilterDataDto> GetTeacherFilterDataByEmailAsync(string email);
    Task<List<StudentPerformanceDto>> GetStudentPerformanceAsync(string courseName);
}