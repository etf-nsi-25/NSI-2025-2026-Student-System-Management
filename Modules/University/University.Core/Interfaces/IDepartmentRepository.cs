using Common.Core.Interfaces.Repsitories;
using University.Core.Entities;

namespace University.Core.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<IEnumerable<Department>> GetAllByFacultyIdAsync(Guid facultyId);
    }
}
