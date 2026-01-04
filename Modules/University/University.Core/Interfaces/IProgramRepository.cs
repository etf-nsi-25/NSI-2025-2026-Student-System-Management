using Common.Core.Interfaces.Repsitories;
using University.Core.Entities;

namespace University.Core.Interfaces
{
    public interface IProgramRepository : IBaseRepository<Program>
    {
        Task<IEnumerable<Program>> GetAllByDepartmentIdAsync(int departmentId);
    }
}
