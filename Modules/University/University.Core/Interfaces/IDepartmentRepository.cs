using Common.Core.Interfaces;
using University.Infrastructure.Entities;

namespace University.Core.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<IEnumerable<Department>> GetAllByFacultyIdAsync(Guid facultyId);
    }
}
