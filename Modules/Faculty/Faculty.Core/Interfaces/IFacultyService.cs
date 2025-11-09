using Faculty.Core.Entities;
using Faculty.Core.Enums;
using Faculty.Core.Models;

namespace Faculty.Core.Interfaces
{
    public interface IFacultyService
    {
        Task<Faculty.Core.Entities.Faculty> CreateFacultyAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default);
        
        Task<PagedResult<Faculty.Core.Entities.Faculty>> GetFacultiesAsync(
            string? nameFilter, 
            FacultyStatus? statusFilter,
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken = default);
        
        Task<Faculty.Core.Entities.Faculty?> GetFacultyByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        Task<Faculty.Core.Entities.Faculty> UpdateFacultyAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default);
        
        Task DeleteFacultyAsync(Guid id, CancellationToken cancellationToken = default);
    }
}