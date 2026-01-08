using Faculty.Core.Entities;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Faculty.Tests;

public class FacultyDbContextMultiTenancyTests : IDisposable
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly Guid _facultyAGuid;
    private readonly Guid _facultyBGuid;

    public FacultyDbContextMultiTenancyTests()
    {
        _facultyAGuid = new Guid("215db11f-1204-43b9-80f5-c2941487843c");
        _facultyBGuid = new Guid("67c33340-63f9-4dd4-90e6-663e6f6af387");

        using var seedContext = CreateContext(new FixedTenantService(_facultyAGuid));
        seedContext.Database.EnsureCreated();
        SeedData(seedContext);
    }

    [Fact]
    public async Task TeachersQuery_ShouldReturnOnlyCurrentFacultyRecords()
    {
        await using var context = CreateContext(new FixedTenantService(_facultyAGuid));

        var teachers = await context.Teachers.AsNoTracking().ToListAsync();

        Assert.Single(teachers);
        Assert.Equal(_facultyAGuid, teachers[0].FacultyId);
    }

    [Fact]
    public async Task SwitchingTenant_ShouldReturnDifferentDataset()
    {
        await using var contextA = CreateContext(new FixedTenantService(_facultyAGuid));
        var tenantATeacherIds = (await contextA.Teachers.AsNoTracking().ToListAsync())
            .Select(t => t.Id)
            .ToList();

        await using var contextB = CreateContext(new FixedTenantService(_facultyBGuid), ensureCreated: false);
        var tenantBTeachers = await contextB.Teachers.AsNoTracking().ToListAsync();

        Assert.Single(tenantBTeachers);
        Assert.DoesNotContain(tenantBTeachers[0].Id, tenantATeacherIds);
        Assert.Equal(_facultyBGuid, tenantBTeachers[0].FacultyId);
    }

    [Fact]
    public async Task StudentsQuery_ShouldRespectTenantFilter()
    {
        await using var context = CreateContext(new FixedTenantService(_facultyBGuid));
        var students = await context.Students.AsNoTracking().ToListAsync();

        Assert.Single(students);
        Assert.Equal(_facultyBGuid, students[0].FacultyId);
        Assert.DoesNotContain(students, s => s.FacultyId == _facultyAGuid);
    }

    [Fact]
    public async Task IgnoreQueryFilters_ShouldReturnAllFacultyData_WhenExplicitlyBypassed()
    {
        await using var context = CreateContext(new FixedTenantService(_facultyAGuid));

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
        var teachers = new List<TeacherSchema>
        {
            new TeacherSchema
            {
                FacultyId = _facultyAGuid,
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Alice",
                LastName = "Anderson",
                Title = "Prof.",
                CreatedAt = DateTime.UtcNow
            },
            new TeacherSchema
            {
                FacultyId = _facultyBGuid,
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Bob",
                LastName = "Brown",
                Title = "Dr.",
                CreatedAt = DateTime.UtcNow
            }
        };

        var students = new List<StudentSchema>
        {
            new StudentSchema
            {
                FacultyId = _facultyAGuid,
                UserId = Guid.NewGuid().ToString(),
                IndexNumber = "A-001",
                FirstName = "Charlie",
                LastName = "Clark",
                CreatedAt = DateTime.UtcNow
            },
            new StudentSchema
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

    public void Dispose()
    {
        // InMemory databases are scoped by name; dispose context to release resources.
    }

    private sealed class FixedTenantService : ITenantService
    {
        private readonly Guid _facultyId;

        public FixedTenantService(Guid facultyId)
        {
            _facultyId = facultyId;
        }

        public Guid GetCurrentFacultyId() => _facultyId;
    }
}

