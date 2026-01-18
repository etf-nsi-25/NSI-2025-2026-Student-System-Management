using System.Linq.Expressions;
using System.Linq;
using Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Db;
using University.Core.Interfaces;
using University.Core.Entities;
using University.Infrastructure.Entities;
using University.Infrastructure.Mappers;

namespace University.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly UniversityDbContext _context;

        public DepartmentRepository(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAsync(
            Expression<Func<Department, bool>>? filter = null,
            Func<IQueryable<Department>, IOrderedQueryable<Department>>? orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            var query = _context.Departments.AsQueryable();
            var schemas = await query.ToListAsync(cancellationToken);
            return schemas.Select(s => DepartmentMapper.ToDomain(s));
        }

        public async Task<Department?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.Departments.FirstOrDefaultAsync(x => x.Id == intId, cancellationToken);
            return DepartmentMapper.ToDomain(schema);
        }

        public async Task<Department?> FirstOrDefaultAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Departments.ToListAsync(cancellationToken);
            return schemas.Select(s => DepartmentMapper.ToDomain(s)).AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Departments.ToListAsync(cancellationToken);
            return schemas.Select(s => DepartmentMapper.ToDomain(s)).AsQueryable().Any(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<Department, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var schemas = await _context.Departments.ToListAsync(cancellationToken);
            var domains = schemas.Select(s => DepartmentMapper.ToDomain(s));
            return predicate == null ? domains.Count() : domains.AsQueryable().Count(predicate);
        }

        public async Task<Department> AddAsync(Department entity, CancellationToken cancellationToken = default)
        {
            // Note: ToSchema for Department is not implemented yet in the Mapper, 
            // but we can do it manually or add it to Mapper.
            // For now, let's just do it manually here if needed.
            var schema = new DepartmentSchema
            {
                FacultyId = entity.FacultyId,
                Name = entity.Name,
                Code = entity.Code,
                HeadOfDepartmentId = entity.HeadOfDepartmentId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Departments.AddAsync(schema, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            entity.Id = schema.Id;
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Department> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities) await AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(Department entity, CancellationToken cancellationToken = default)
        {
            var schema = await _context.Departments.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (schema != null)
            {
                schema.Name = entity.Name;
                schema.Code = entity.Code;
                schema.FacultyId = entity.FacultyId;
                schema.HeadOfDepartmentId = entity.HeadOfDepartmentId;
                schema.UpdatedAt = DateTime.UtcNow;
                _context.Departments.Update(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var intId = (int)id;
            var schema = await _context.Departments.FindAsync(new object[] { intId }, cancellationToken);
            if (schema != null)
            {
                _context.Departments.Remove(schema);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(Department entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken);
        }

        public async Task<IEnumerable<Department>> GetAllByFacultyIdAsync(Guid facultyId)
        {
            var schemas = await _context.Departments
                .Include(d => d.Faculty)
                .Where(d => d.FacultyId == facultyId)
                .ToListAsync();

            return schemas.Select(s => DepartmentMapper.ToDomain(s)).ToList();
        }
    }
}
