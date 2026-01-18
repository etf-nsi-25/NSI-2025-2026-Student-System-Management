using System.Linq.Expressions;
using System.Linq;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;
using University.Infrastructure.Mappers;

namespace University.Infrastructure.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly UniversityDbContext _context;

        public FacultyRepository(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Faculty>> GetAsync(
            Expression<Func<Faculty, bool>>? filter = null,
            Func<IQueryable<Faculty>, IOrderedQueryable<Faculty>>? orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            // Note: Complex filtering on Core entities that translates to Schema queries is difficult.
            // For now, we fetch all and filter in memory or implement specific methods.
            // Since most services use specific methods, we'll implement a basic version.
            var query = _context.Faculties.AsQueryable();
            var schemas = await query.ToListAsync(cancellationToken);
            return schemas.Select(s => FacultyMapper.ToDomain(s));
        }

        public async Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var schema = await _context.Faculties.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return FacultyMapper.ToDomain(schema);
        }

        public async Task<Faculty?> FirstOrDefaultAsync(Expression<Func<Faculty, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Simple implementation for common cases
            var schemas = await _context.Faculties.ToListAsync(cancellationToken);
            return schemas.Select(s => FacultyMapper.ToDomain(s)).AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<Faculty, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Faculties.ToListAsync(cancellationToken);
            return schemas.Select(s => FacultyMapper.ToDomain(s)).AsQueryable().Any(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<Faculty, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Faculties.ToListAsync(cancellationToken);
            var domains = schemas.Select(s => FacultyMapper.ToDomain(s));
            return predicate == null ? domains.Count() : domains.AsQueryable().Count(predicate);
        }

        public async Task<Faculty> AddAsync(Faculty entity, CancellationToken cancellationToken = default)
        {
            var schema = FacultyMapper.ToSchema(entity);
            await _context.Faculties.AddAsync(schema, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            entity.Id = schema.Id;
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Faculty> entities, CancellationToken cancellationToken = default)
        {
            var schemas = entities.Select(e => FacultyMapper.ToSchema(e));
            await _context.Faculties.AddRangeAsync(schemas, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Faculty entity, CancellationToken cancellationToken = default)
        {
            var schema = await _context.Faculties.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (schema != null)
            {
                schema.Name = entity.Name;
                schema.Address = entity.Address;
                schema.Code = entity.Code;
                schema.Description = entity.Description;
                schema.DeanId = entity.DeanId;
                schema.EstablishedDate = entity.EstablishedDate;
                schema.UpdatedAt = DateTime.UtcNow;

                _context.Faculties.Update(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.Faculties.FindAsync(new object[] { intId }, cancellationToken);
            if (schema != null)
            {
                _context.Faculties.Remove(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(Faculty entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken);
        }

        public async Task<Faculty?> GetFacultyByCodeAsync(string code)
        {
            var schema = await _context.Faculties.FirstOrDefaultAsync(f => f.Code == code);
            return FacultyMapper.ToDomain(schema);
        }

        public Task<Faculty?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
