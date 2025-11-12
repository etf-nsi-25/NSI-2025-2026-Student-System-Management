using Identity.Core.Entities;
using Identity.Core.Repositories;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(AuthDbContext context, UserManager<ApplicationUser> userManager)
        : base(context)
    {
        _userManager = userManager;
    }

    public async Task<User> CreateUser(string email)
    {
        ApplicationUser newUser = new ApplicationUser { UserName = email };
        await context.AddAsync(newUser);

 
        return new User(newUser.Id, newUser.UserName);
    }

    public Task Save()
    {
        return context.SaveChangesAsync();
    }
}