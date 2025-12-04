using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Faculty.Test;

public class FacultyDbContextMultiTenancyTests : IDisposable
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly Guid _facultyAGuid;
    private readonly Guid _facultyBGuid;

    public FacultyDbContextMultiTenancyTests()
    {
        _facultyAGuid = CreateFacultyGuid(1);
        _facultyBGuid = CreateFacultyGuid(2);

        using var seedContext = CreateContext(new FixedTenantService(1));
        seedContext.Database.EnsureCreated();
        SeedData(seedContext);
    }

    [Fact]
    public async Task TeachersQuery_ShouldReturnOnlyCurrentFacultyRecords()
    {
        await using var context = CreateContext(new FixedTenantService(1));

        var teachers = await context.Teachers.AsNoTracking().ToListAsync();

        Assert.Single(teachers);
        Assert.Equal(_facultyAGuid, teachers[0].FacultyId);
    }

    [Fact]
    public async Task SwitchingTenant_ShouldReturnDifferentDataset()
    {
        await using var contextA = CreateContext(new FixedTenantService(1));
        var tenantATeacherIds = (await contextA.Teachers.AsNoTracking().ToListAsync())
            .Select(t => t.Id)
            .ToList();

        await using var contextB = CreateContext(new FixedTenantService(2), ensureCreated: false);
        var tenantBTeachers = await contextB.Teachers.AsNoTracking().ToListAsync();

        Assert.Single(tenantBTeachers);
        Assert.DoesNotContain(tenantBTeachers[0].Id, tenantATeacherIds);
        Assert.Equal(_facultyBGuid, tenantBTeachers[0].FacultyId);
    }

    [Fact]
    public async Task StudentsQuery_ShouldRespectTenantFilter()
    {
        await using var context = CreateContext(new FixedTenantService(2));
        var students = await context.Students.AsNoTracking().ToListAsync();

        Assert.Single(students);
        Assert.Equal(_facultyBGuid, students[0].FacultyId);
        Assert.DoesNotContain(students, s => s.FacultyId == _facultyAGuid);
    }

    [Fact]
    public async Task IgnoreQueryFilters_ShouldReturnAllFacultyData_WhenExplicitlyBypassed()
    {
        await using var context = CreateContext(new FixedTenantService(1));

        var teachers = await context.Teachers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(2, teachers.Count);
        Assert.Contains(teachers, t => t.FacultyId == _facultyAGuid);
        Assert.Contains(teachers, t => t.FacultyId == _facultyBGuid);
    }

    private FacultyDbContext CreateContext(ITenantService tenantService, bool ensureCreated = true)
    {
        var options = new DbContextOptionsBuilder<FacultyDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        var context = new FacultyDbContext(options, tenantService);
        if (ensureCreated)
        {
            context.Database.EnsureCreated();
        }
        return context;
    }

    private void SeedData(FacultyDbContext context)
    {
        var teachers = new List<Teacher>
        {
            new Teacher
            {
                FacultyId = _facultyAGuid,
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Alice",
                LastName = "Anderson",
                Title = "Prof.",
                CreatedAt = DateTime.UtcNow
            },
            new Teacher
            {
                FacultyId = _facultyBGuid,
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Bob",
                LastName = "Brown",
                Title = "Dr.",
                CreatedAt = DateTime.UtcNow
            }
        };

        var students = new List<Student>
        {
            new Student
            {
                FacultyId = _facultyAGuid,
                UserId = Guid.NewGuid().ToString(),
                IndexNumber = "A-001",
                FirstName = "Charlie",
                LastName = "Clark",
                CreatedAt = DateTime.UtcNow
            },
            new Student
            {
                FacultyId = _facultyBGuid,
                UserId = Guid.NewGuid().ToString(),
                IndexNumber = "B-001",
                FirstName = "Dana",
                LastName = "Davis",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.AddRange(teachers);
        context.AddRange(students);
        context.SaveChanges();
    }

    private static Guid CreateFacultyGuid(int facultyId)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(facultyId).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    public void Dispose()
    {
        // InMemory databases are scoped by name; dispose context to release resources.
    }

    private sealed class FixedTenantService : ITenantService
    {
        private readonly int _facultyId;

        public FixedTenantService(int facultyId)
        {
            _facultyId = facultyId;
        }

        public int GetCurrentFacultyId() => _facultyId;
    }
}

