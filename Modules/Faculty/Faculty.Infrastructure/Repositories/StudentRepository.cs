using Common.Infrastructure.Repositories;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Faculty.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly FacultyDbContext _context;

    public StudentRepository(FacultyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Student>> GetAsync(
        Expression<Func<Student, bool>>? filter = null,
        Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default)
    {
        IQueryable<StudentSchema> query = _context.Students;

        if (filter != null)
        {
            var allSchemas = await query.ToListAsync(cancellationToken);
            var allDomains = StudentMapper.ToDomainCollection(allSchemas, includeRelationships: !string.IsNullOrEmpty(includeProperties)).ToList();
            var filtered = filter != null ? allDomains.AsQueryable().Where(filter) : allDomains.AsQueryable();
            
            if (orderBy != null)
            {
                return orderBy(filtered).ToList();
            }
            return filtered.ToList();
        }

        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        var schemas = await query.ToListAsync(cancellationToken);
        var domains = StudentMapper.ToDomainCollection(schemas, includeRelationships: !string.IsNullOrEmpty(includeProperties)).ToList();

        if (orderBy != null)
        {
            return orderBy(domains.AsQueryable()).ToList();
        }

        return domains;
    }

    public async Task<Student?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var schema = await _context.Students.FindAsync(new object[] { id }, cancellationToken);
        return schema != null ? StudentMapper.ToDomain(schema, includeRelationships: false) : null;
    }

    public async Task<Student?> FirstOrDefaultAsync(
        Expression<Func<Student, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var schemas = await _context.Students.ToListAsync(cancellationToken);
        var domains = StudentMapper.ToDomainCollection(schemas, includeRelationships: false);
        return domains.FirstOrDefault(predicate.Compile());
    }

    public async Task<bool> AnyAsync(
        Expression<Func<Student, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var schemas = await _context.Students.ToListAsync(cancellationToken);
        var domains = StudentMapper.ToDomainCollection(schemas, includeRelationships: false);
        return domains.Any(predicate.Compile());
    }

    public async Task<int> CountAsync(
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _context.Students.CountAsync(cancellationToken);

        var schemas = await _context.Students.ToListAsync(cancellationToken);
        var domains = StudentMapper.ToDomainCollection(schemas, includeRelationships: false);
        return domains.Count(predicate.Compile());
    }

    public async Task<Student> AddAsync(Student entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var schema = StudentMapper.ToPersistence(entity);
        await _context.Students.AddAsync(schema, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return StudentMapper.ToDomain(schema, includeRelationships: false);
    }

    public async Task AddRangeAsync(IEnumerable<Student> entities, CancellationToken cancellationToken = default)
    {
        var schemas = StudentMapper.ToPersistenceCollection(entities);
        await _context.Students.AddRangeAsync(schemas, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Student entity, CancellationToken cancellationToken = default)
    {
        var existingSchema = await _context.Students.FindAsync(new object[] { entity.Id });
        if (existingSchema != null)
        {
            StudentMapper.UpdatePersistence(existingSchema, entity);
            existingSchema.UpdatedAt = DateTime.UtcNow;
            _context.Students.Update(existingSchema);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        var schema = await _context.Students.FindAsync(new object[] { id }, cancellationToken);
        if (schema != null)
        {
            await DeleteAsync(StudentMapper.ToDomain(schema, includeRelationships: false), cancellationToken);
        }
    }

    public async Task DeleteAsync(Student entity, CancellationToken cancellationToken = default)
    {
        var schema = await _context.Students.FindAsync(new object[] { entity.Id }, cancellationToken);
        if (schema != null)
        {
            _context.Students.Remove(schema);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
