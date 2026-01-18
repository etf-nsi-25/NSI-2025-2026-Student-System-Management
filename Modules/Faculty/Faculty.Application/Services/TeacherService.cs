using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services
{
    public class TeacherService(ITeacherRepository _teacherRepository) : ITeacherService
    {
        public async Task<int?> GetTeacherIDByUserID(Guid userID)
        {
            return (await _teacherRepository.FirstOrDefaultAsync(t => t.UserId.Equals(userID)))?.Id;
        }
    }
}
