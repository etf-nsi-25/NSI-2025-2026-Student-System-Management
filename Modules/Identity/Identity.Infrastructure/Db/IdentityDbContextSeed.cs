using Common.Core.Tenant;
using Identity.Core.Enums;
using Identity.Infrastructure.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Infrastructure.Entities;

public class IdentityDbContextSeed
{
    private readonly IScopedTenantContext _tenantContext;

    public IdentityDbContextSeed(IScopedTenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task SeedAsync(
        AuthDbContext context,
        UserManager<ApplicationUser> userManager,
        Guid superAdminId,
        Guid adminId,
        Guid teacherId,
        Guid studentId)
    {
        if (await userManager.Users.AnyAsync())
            return;

        var superAdminAppUser = new ApplicationUser
        {
            Id = superAdminId.ToString(),
            UserName = "superadmin",
            Email = "superadmin@unsa.ba",
            FirstName = "Super",
            LastName = "Admin",
            EmailConfirmed = true,
            Role = UserRole.Superadmin
        };
        await userManager.CreateAsync(superAdminAppUser, "Test123!");

        var adminAppUser = new ApplicationUser
        {
            Id = adminId.ToString(),
            UserName = "admin",
            Email = "admin@unsa.ba",
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            Role = UserRole.Admin
        };
        await userManager.CreateAsync(adminAppUser, "Test123!");

        var teacherAppUser = new ApplicationUser
        {
            Id = teacherId.ToString(),
            UserName = "teacher",
            Email = "emir.buza@unsa.ba",
            FirstName = "Emir",
            LastName = "Buza",
            EmailConfirmed = true,
            Role = UserRole.Teacher
        };
        await userManager.CreateAsync(teacherAppUser, "Test123!");

        var studentAppUser = new ApplicationUser
        {
            Id = studentId.ToString(),
            UserName = "student",
            Email = "niko.nikic@unsa.ba",
            FirstName = "Niko",
            LastName = "Nikic",
            EmailConfirmed = true,
            Role = UserRole.Student,
            IndexNumber = "IB20001"
        };
        await userManager.CreateAsync(studentAppUser, "Test123!");

    }
}