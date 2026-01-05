using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Course?> UpdateAsync(Course course, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Course>> GetByTeacherUserIdAsync(string userId, CancellationToken cancellationToken = default);

    }

}
