using Common.Core.Interfaces.Repsitories;
using University.Core.Entities;

namespace University.Core.Interfaces
{
    public interface IFacultyRepository : IBaseRepository<Faculty>
    {
        Task<Faculty?> GetFacultyByCodeAsync(string code);
    }
}
