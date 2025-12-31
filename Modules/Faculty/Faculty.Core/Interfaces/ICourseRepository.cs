using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> AddAsync(Course course);
        Task<Course?> GetByIdAsync(Guid id);
        Task<List<Course>> GetAllAsync();
        Task<List<Course>> GetByTeacherUserIdAsync(string userId);
        Task<Course?> UpdateAsync(Course course);
        Task<bool> DeleteAsync(Guid id);
    }
}
