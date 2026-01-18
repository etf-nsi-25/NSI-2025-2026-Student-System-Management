using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;
using University.Infrastructure.Entities;
using University.Infrastructure.Mappers;
using System.Linq.Expressions;
using System.Linq;

namespace University.Infrastructure.Repositories
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly UniversityDbContext _context;

        public AcademicYearRepository(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicYear>> GetAsync(
            Expression<Func<AcademicYear, bool>>? filter = null,
            Func<IQueryable<AcademicYear>, IOrderedQueryable<AcademicYear>>? orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            var query = _context.AcademicYears.AsQueryable();
            var schemas = await query.ToListAsync(cancellationToken);
            return schemas.Select(s => AcademicYearMapper.ToDomain(s));
        }

        public async Task<AcademicYear?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == intId, cancellationToken);
            return AcademicYearMapper.ToDomain(schema);
        }

        public async Task<AcademicYear?> FirstOrDefaultAsync(Expression<Func<AcademicYear, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.AcademicYears.ToListAsync(cancellationToken);
            return schemas.Select(s => AcademicYearMapper.ToDomain(s)).AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<AcademicYear, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.AcademicYears.ToListAsync(cancellationToken);
            return schemas.Select(s => AcademicYearMapper.ToDomain(s)).AsQueryable().Any(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<AcademicYear, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.AcademicYears.ToListAsync(cancellationToken);
            var domains = schemas.Select(s => AcademicYearMapper.ToDomain(s));
            return predicate == null ? domains.Count() : domains.AsQueryable().Count(predicate);
        }

        public async Task<AcademicYear> AddAsync(AcademicYear entity, CancellationToken cancellationToken = default)
        {
            var schema = new AcademicYearSchema
            {
                Year = entity.Year,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            await _context.AcademicYears.AddAsync(schema, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            entity.Id = schema.Id;
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<AcademicYear> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities) await AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(AcademicYear entity, CancellationToken cancellationToken = default)
        {
            var schema = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (schema != null)
            {
                schema.Year = entity.Year;
                schema.StartDate = entity.StartDate;
                schema.EndDate = entity.EndDate;
                schema.IsActive = entity.IsActive;
                schema.UpdatedAt = DateTime.UtcNow;
                _context.AcademicYears.Update(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.AcademicYears.FindAsync(new object[] { intId }, cancellationToken);
            if (schema != null)
            {
                _context.AcademicYears.Remove(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(AcademicYear entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken);
        }

        public async Task<AcademicYear?> GetActiveAcademicYearAsync()
        {
            var schema = await _context.AcademicYears
                .FirstOrDefaultAsync(ay => ay.IsActive);

            return AcademicYearMapper.ToDomain(schema);
        }
    }
}
