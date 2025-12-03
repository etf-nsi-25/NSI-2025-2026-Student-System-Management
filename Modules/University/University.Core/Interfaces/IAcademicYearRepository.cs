using System.Threading.Tasks;
using Common.Core.Interfaces;
using University.Core.Interfaces;
using University.Infrastructure.Entities;


namespace University.Core.Interfaces
{
    public interface IAcademicYearRepository : IBaseRepository<AcademicYear>
    {
        Task<AcademicYear?> GetActiveAcademicYearAsync();
    }
}
