
using Common.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Core.Interfaces
{
    public interface IProgramRepository : IBaseRepository<Program>
    {
        Task<IEnumerable<Program>> GetAllByDepartmentIdAsync(Guid departmentId);
    }
}