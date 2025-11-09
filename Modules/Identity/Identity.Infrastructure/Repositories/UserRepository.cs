using Identity.Core.Entities;
using Identity.Core.Repositories;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Entities;

namespace Identity.Infrastructure.Repositories;

public class UserRepository(AuthDbContext context) : IUserRepository
{
    // TODO: think about creating BaseRepository abstract class as there will be a lot of repetetive code
    // A guide can be found here: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    public async Task<User> CreateUser(string email)
    {
        ApplicationUser newUser = new ApplicationUser { UserName = email };
        await context.Users.AddAsync(newUser);

        // TODO: implement mappers
        return new User(newUser.Id, newUser.UserName);
    }

    public Task Save()
    {
        return context.SaveChangesAsync();
    }
}