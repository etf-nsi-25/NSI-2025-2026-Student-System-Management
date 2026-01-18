using Common.Core.Interfaces.Repsitories;
using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces
{
    public interface ITeacherRepository: IBaseRepository<Teacher>
    {
        Task<Teacher?> GetByUserIdAsync(string userId);
        Task<Teacher?> GetByIdAsync(int id);
    }
}