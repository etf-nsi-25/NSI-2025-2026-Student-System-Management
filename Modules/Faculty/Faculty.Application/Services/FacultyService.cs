using Faculty.Application.DTOs;
using Faculty.Core.Entities;
using Faculty.Core.Exceptions;
using Faculty.Core.Interfaces;
using Faculty.Core.Models;
using Faculty.Core.Enums;

namespace Faculty.Application.Services
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _repository;
        
        public FacultyService(IFacultyRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<Faculty.Core.Entities.Faculty> CreateFacultyAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default)
        {
            // Validation for uniqueness will be handled by the caller (e.g., controller)
            // or can be moved here if the service is responsible for all validation.
            // For now, assuming basic validation is done before calling this.
            
            var created = await _repository.AddAsync(faculty, cancellationToken);
            return created;
        }
        
        public async Task<PagedResult<Faculty.Core.Entities.Faculty>> GetFacultiesAsync(
            string? nameFilter, 
            FacultyStatus? statusFilter,
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAllAsync(
                nameFilter, 
                statusFilter, 
                pageNumber, 
                pageSize,
                cancellationToken);
            
            return result;
        }
        
        public async Task<Faculty.Core.Entities.Faculty?> GetFacultyByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var faculty = await _repository.GetByIdAsync(id, cancellationToken);
            return faculty;
        }
        
        public async Task<Faculty.Core.Entities.Faculty> UpdateFacultyAsync(Faculty.Core.Entities.Faculty faculty, CancellationToken cancellationToken = default)
        {
            // Validation for uniqueness will be handled by the caller (e.g., controller)
            // or can be moved here if the service is responsible for all validation.
            // For now, assuming basic validation is done before calling this.
            
            await _repository.UpdateAsync(faculty, cancellationToken);
            return faculty;
        }
        
        public async Task DeleteFacultyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new NotFoundException($"Faculty with ID {id} not found");
            
            // Check for conflicts
            if (await _repository.IsInUseAsync(id, cancellationToken))
                throw new ConflictException("Faculty is in use and cannot be deleted");
            
            await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}