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
    public class ProgramRepository : IProgramRepository
    {
        private readonly UniversityDbContext _context;

        public ProgramRepository(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Program>> GetAsync(
            Expression<Func<Program, bool>>? filter = null,
            Func<IQueryable<Program>, IOrderedQueryable<Program>>? orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            var query = _context.Programs.AsQueryable();
            var schemas = await query.ToListAsync(cancellationToken);
            return schemas.Select(s => ProgramMapper.ToDomain(s));
        }

        public async Task<Program?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.Programs.FirstOrDefaultAsync(x => x.Id == intId, cancellationToken);
            return ProgramMapper.ToDomain(schema);
        }

        public async Task<Program?> FirstOrDefaultAsync(Expression<Func<Program, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Programs.ToListAsync(cancellationToken);
            return schemas.Select(s => ProgramMapper.ToDomain(s)).AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<Program, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Programs.ToListAsync(cancellationToken);
            return schemas.Select(s => ProgramMapper.ToDomain(s)).AsQueryable().Any(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<Program, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Programs.ToListAsync(cancellationToken);
            var domains = schemas.Select(s => ProgramMapper.ToDomain(s));
            return predicate == null ? domains.Count() : domains.AsQueryable().Count(predicate);
        }

        public async Task<Program> AddAsync(Program entity, CancellationToken cancellationToken = default)
        {
            var schema = new ProgramSchema
            {
                DepartmentId = entity.DepartmentId,
                Name = entity.Name,
                Code = entity.Code,
                DegreeType = entity.DegreeType,
                DurationYears = entity.DurationYears,
                Credits = entity.Credits,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Programs.AddAsync(schema, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            entity.Id = schema.Id;
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Program> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities) await AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(Program entity, CancellationToken cancellationToken = default)
        {
            var schema = await _context.Programs.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (schema != null)
            {
                schema.Name = entity.Name;
                schema.Code = entity.Code;
                schema.DepartmentId = entity.DepartmentId;
                schema.DegreeType = entity.DegreeType;
                schema.DurationYears = entity.DurationYears;
                schema.Credits = entity.Credits;
                schema.UpdatedAt = DateTime.UtcNow;
                _context.Programs.Update(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.Programs.FindAsync(new object[] { intId }, cancellationToken);
            if (schema != null)
            {
                _context.Programs.Remove(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(Program entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken);
        }

        public async Task<IEnumerable<Program>> GetAllByDepartmentIdAsync(int departmentId)
        {
            var schemas = await _context.Programs
                .Where(p => p.DepartmentId == departmentId)
                .ToListAsync();

            return schemas.Select(s => ProgramMapper.ToDomain(s)).ToList();
        }
    }
}
