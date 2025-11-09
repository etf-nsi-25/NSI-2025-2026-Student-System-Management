using Faculty.Core.Entities;
using Faculty.Core.Enums;
using Faculty.Core.Models;

namespace Faculty.Core.Interfaces
{
    public interface IFacultyRepository
    {
        Task<Faculty.Core.Entities.Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        Task<PagedResult<Faculty.Core.Entities.Faculty>> GetAllAsync(
            string? nameFilter, 
            FacultyStatus? statusFilter,
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken = default);
        
        Task<Faculty.Core.Entities.Faculty> AddAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default);
        
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
        
        Task<bool> IsAbbreviationUniqueAsync(string? abbreviation, Guid? excludeId = null, CancellationToken cancellationToken = default);
        
        Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default); // For conflict detection
    }
}