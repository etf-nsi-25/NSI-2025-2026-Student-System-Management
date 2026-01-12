using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Identity.Application.Services;
using Identity.Core.Repositories;
using Identity.Core.Enums;
using Identity.Core.DTO;
using Identity.Core.Entities;
using Identity.Core.Events;
using Identity.Core.Services;
using Identity.Infrastructure.Entities;
using System.Linq;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;   
    private readonly Mock<IIdentityHasherService> _mockHasher;
    private readonly Mock<IEventPublisher> _mockPublisher;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockHasher = new Mock<IIdentityHasherService>();
        _mockPublisher = new Mock<IEventPublisher>();

        _userService = new UserService(_mockRepo.Object, _mockHasher.Object, _mockPublisher.Object);
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ShouldReturnUserId_OnSuccessfulCreation()
    {
        var username = "testuser";
        var password = "Password123";
        var role = UserRole.Teacher;
        var expectedHash = "base64hashedpasswordstring";
        
        _mockRepo.Setup(r => r.IsUsernameTakenAsync(username)).ReturnsAsync(false);
       _mockHasher.Setup(h => h.HashPassword(password)).Returns(expectedHash) ;   
       
        var userId = await _userService.CreateUserAsync(
            username, password, "FName", "LName", Guid.NewGuid(), null, role
        );

        Assert.NotEqual(Guid.Empty, userId);
        
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        
        _mockPublisher.Verify(p => p.PublishAsync(It.IsAny<UserCreatedEvent>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldThrowArgumentException_WhenUsernameIsTaken()
    {
        var username = "taken_username";
        
        _mockRepo.Setup(r => r.IsUsernameTakenAsync(username)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ArgumentException>(() => 
            _userService.CreateUserAsync(username, "Pwd123", "F", "L", Guid.NewGuid(), null, UserRole.Student)
        );
        
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldThrowInvalidOperationException_WhenRoleIsAdmin()
    {
        var role = UserRole.Admin;

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _userService.CreateUserAsync("u", "p", "f", "l", Guid.NewGuid(), null, role)
        );
    }

    #endregion

    #region UpdateUserAsync Tests

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnTrueAndPublishEvent_WhenRoleChanges()
    {
        var userId = Guid.NewGuid();
        var oldRole = UserRole.Student;
        var newRole = UserRole.Assistant;

        var existingUser = User.Create("u1", string.Empty, "F", "L", Guid.NewGuid(), oldRole, "12345");
        typeof(User).GetProperty("Id")?.SetValue(existingUser, userId); 

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        
        var request = new UpdateUserRequest 
        { 
            FirstName = "NewF", LastName = "NewL", FacultyId = existingUser.FacultyId, 
            Role = newRole, Status = UserStatus.Active 
        };

        var result = await _userService.UpdateUserAsync(userId, request);

        Assert.True(result);
        
        _mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        
        _mockPublisher.Verify(p => p.PublishAsync(
            It.Is<UserRoleAssignedEvent>(e => e.PreviousRole == oldRole && e.NewRole == newRole)
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);
        
        var request = new UpdateUserRequest 
        { 
            FirstName = "N", LastName = "L", FacultyId = Guid.NewGuid(), 
            Role = UserRole.Teacher, Status = UserStatus.Active 
        };

        var result = await _userService.UpdateUserAsync(userId, request);

        Assert.False(result);
        _mockRepo.Verify(r => r.SaveAsync(), Times.Never); 
    }
    
    
    #endregion

#region Deactivate & Delete Tests

[Fact]
public async Task DeactivateUserAsync_ShouldReturnTrue_WhenUserIsActive()
{
    var userId = Guid.NewGuid();
    var activeUser = User.Create("u", string.Empty, "F", "L", Guid.NewGuid(), UserRole.Teacher);
    typeof(User).GetProperty("Id")?.SetValue(activeUser, userId); 

    _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(activeUser);

    var result = await _userService.DeactivateUserAsync(userId);

    Assert.True(result);
    _mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Status == UserStatus.Inactive)), Times.Once);
    _mockRepo.Verify(r => r.SaveAsync(), Times.Once);
}

[Fact]
public async Task DeactivateUserAsync_ShouldThrowException_WhenSuperadminIsTargeted()
{
    var userId = Guid.NewGuid();
    var superadmin = User.Create("sa", string.Empty, "SA", "L", Guid.NewGuid(), UserRole.Superadmin);
    typeof(User).GetProperty("Id")?.SetValue(superadmin, userId); 

    _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(superadmin);

    await Assert.ThrowsAsync<InvalidOperationException>(() => 
        _userService.DeactivateUserAsync(userId)
    );
    _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
}

[Fact]
public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
{
    _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);


    var result = await _userService.DeleteUserAsync(Guid.NewGuid());

    Assert.False(result);
    _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Never);
}

[Fact]
public async Task DeleteUserAsync_ShouldPublishEvent_OnSuccess()
{
    var userId = Guid.NewGuid();
    var user = User.Create("u", string.Empty, "F", "L", Guid.NewGuid(), UserRole.Teacher);
    typeof(User).GetProperty("Id")?.SetValue(user, userId); 

    _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

    await _userService.DeleteUserAsync(userId);

    _mockRepo.Verify(r => r.DeleteAsync(user), Times.Once);
    _mockPublisher.Verify(p => p.PublishAsync(
        It.Is<UserDeletedEvent>(e => e.UserId == userId)
    ), Times.Once);
}

#endregion


#region GetAllUsersAsync Tests

[Fact]
public async Task GetAllUsersAsync_ShouldPassFilterToRepoAndReturnList()
{
    var facultyId = Guid.NewGuid();
    var filter = new UserFilterRequest 
    { 
        FacultyId = facultyId, 
        Role = UserRole.Student,
        PageNumber = 2,
        PageSize = 10,
        SortBy = "username"
    };

    var mockUsers = new List<User> 
    { 
         User.Create("u1", string.Empty, "A", "B", facultyId, UserRole.Student, "001").SetId(Guid.NewGuid()),
         User.Create("u2", string.Empty, "C", "D", facultyId, UserRole.Student, "002").SetId(Guid.NewGuid()),
         User.Create("u3", string.Empty, "E", "F", facultyId, UserRole.Student, "003").SetId(Guid.NewGuid())
    };
    var totalCount = 50;
    
    _mockRepo.Setup(r => r.GetAllFilteredAsync(It.IsAny<UserFilterRequest>()))
             .ReturnsAsync(mockUsers.AsReadOnly());
             
    _mockRepo.Setup(r => r.CountAsync(It.IsAny<UserFilterRequest>()))
             .ReturnsAsync(totalCount);
             
    var response = await _userService.GetAllUsersAsync(filter);

    Assert.Equal(totalCount, response.TotalCount);
    Assert.Equal(5, response.TotalPages);
    Assert.Equal(filter.PageNumber, response.PageNumber);
    
    Assert.Equal(mockUsers.Count, response.Items.Count);
    Assert.Equal("A", response.Items.First().FirstName); 
    
    _mockRepo.Verify(r => r.GetAllFilteredAsync(
        It.Is<UserFilterRequest>(f => f.FacultyId == facultyId && f.SortBy == "username")
    ), Times.Once);
}

#endregion
}