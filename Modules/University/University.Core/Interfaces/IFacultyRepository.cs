using Common.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Core.Interfaces
{
    public interface IFacultyRepository : IBaseRepository<Faculty>
    {
        Task<Faculty?> GetFacultyByCodeAsync(string code);
    }
}

