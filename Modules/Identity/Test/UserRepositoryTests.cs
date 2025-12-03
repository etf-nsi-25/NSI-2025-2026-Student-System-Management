using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Identity.Core.Entities;
using Identity.Core.DTO;
using Identity.Core.Enums;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;
using System.Collections.Generic;

public class UserRepositoryTests : IDisposable
{
    private readonly AuthDbContext _context;
    private readonly UserRepository _repository;
    private readonly Guid FacultyA = Guid.NewGuid();
    private readonly Guid FacultyB = Guid.NewGuid();

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AuthDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new UserRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        
        _context.DomainUsers.Add(User.Create("teacher_a", string.Empty, "Teacher", "A", FacultyA, UserRole.Teacher).SetId(Guid.NewGuid()));
        
        _context.DomainUsers.Add(User.Create("student_b", string.Empty, "Student", "B", FacultyB, UserRole.Student).SetId(Guid.NewGuid()));
        
        _context.DomainUsers.Add(User.Create("student_a", string.Empty, "Student", "A", FacultyA, UserRole.Student).SetId(Guid.NewGuid()));
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        var user = _context.DomainUsers.First(u => u.Username == "teacher_a");

        var result = await _repository.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Teacher", result!.FirstName);
    }

    [Fact]
    public async Task GetAllFilteredAsync_ShouldFilterByFacultyAndRole()
    {
        var filter = new UserFilterRequest 
        { 
            FacultyId = FacultyA, 
            Role = UserRole.Student,
            PageNumber = 1,
            PageSize = 10
        };

        var result = await _repository.GetAllFilteredAsync(filter);

        Assert.Single(result);
        Assert.Equal("student_a", result.First().Username);
    }
    
    [Fact]
    public async Task GetAllFilteredAsync_ShouldSortByUsernameDescending()
    {
        var filter = new UserFilterRequest 
        { 
            PageNumber = 1,
            PageSize = 10,
            SortBy = "username",
            SortOrder = "desc"
        };

        var result = await _repository.GetAllFilteredAsync(filter);

        Assert.Equal("teacher_a", result.First().Username);
        Assert.Equal("student_a", result.Last().Username);
    }
    
    [Fact]
    public async Task CountAsync_ShouldReturnCorrectTotalCount()
    {
        var filter = new UserFilterRequest { Role = UserRole.Student };

        var count = await _repository.CountAsync(filter);

        Assert.Equal(2, count); 
    }
    
    [Fact]
public async Task UpdateAsync_ShouldChangeUserData()
{
    var userToUpdate = _context.DomainUsers.First(u => u.Username == "teacher_a");
    var newFirstName = "NewName";
    
    userToUpdate.FullUpdate(
        newFirstName, 
        userToUpdate.LastName, 
        userToUpdate.FacultyId, 
        userToUpdate.Role, 
        userToUpdate.Status, 
        userToUpdate.IndexNumber
    );
    
    await _repository.UpdateAsync(userToUpdate);
    await _repository.SaveAsync(); 

    var updatedUser = await _context.DomainUsers.AsNoTracking().FirstAsync(u => u.Id == userToUpdate.Id);
    Assert.Equal(newFirstName, updatedUser.FirstName);
    Assert.Equal("teacher_a", updatedUser.Username); 
}

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}