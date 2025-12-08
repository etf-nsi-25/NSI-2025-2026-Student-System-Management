using Common.Core.Interfaces.Repsitories;
using University.Core.Entities;


namespace University.Core.Interfaces
{
    public interface IAcademicYearRepository : IBaseRepository<AcademicYear>
    {
        Task<AcademicYear?> GetActiveAcademicYearAsync();
    }
}
