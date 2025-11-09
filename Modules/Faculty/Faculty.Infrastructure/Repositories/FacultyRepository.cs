using Faculty.Core.Entities;
using Faculty.Core.Enums;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Faculty.Core.Models; // For PagedResult

namespace Faculty.Infrastructure.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly FacultyDbContext _context;
        
        public FacultyRepository(FacultyDbContext context)
        {
            _context = context;
        }
        
        public async Task<Faculty.Core.Entities.Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }
        
        public async Task<PagedResult<Faculty.Core.Entities.Faculty>> GetAllAsync(
            string? nameFilter, 
            FacultyStatus? statusFilter,
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Faculties.AsNoTracking();
            
            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(f => EF.Functions.Like(f.Name, $"%{nameFilter}%"));
                
            if (statusFilter.HasValue)
                query = query.Where(f => f.Status == statusFilter.Value);
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderBy(f => f.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            
            return new PagedResult<Faculty.Core.Entities.Faculty>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        
        public async Task<Faculty.Core.Entities.Faculty> AddAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default)
        {
            await _context.Faculties.AddAsync(faculty, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return faculty;
        }
        
        public async Task UpdateAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default)
        {
            faculty.UpdatedAt = DateTime.UtcNow;
            _context.Faculties.Update(faculty);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var faculty = await _context.Faculties.FindAsync(new object[] { id }, cancellationToken);
            if (faculty != null)
            {
                _context.Faculties.Remove(faculty);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        
        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return !await _context.Faculties
                .AnyAsync(f => f.Name == name && f.Id != excludeId, cancellationToken);
        }
        
        public async Task<bool> IsAbbreviationUniqueAsync(string? abbreviation, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
                return true;
                
            return !await _context.Faculties
                .AnyAsync(f => f.Abbreviation == abbreviation && f.Id != excludeId, cancellationToken);
        }
        
        public async Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Check if referenced by Users or Courses (placeholder for now)
            // For this ticket, return false to allow hard delete
            // TODO: Implement actual relationship checks when User/Course modules are available
            return await Task.FromResult(false);
        }
    }
}