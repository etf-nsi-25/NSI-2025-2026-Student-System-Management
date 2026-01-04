using Common.Core.Tenant;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        if (await context.DomainUsers.AnyAsync())
            return;

        var tenantId = _tenantContext.CurrentTenantId()
            ?? throw new InvalidOperationException("Tenant not set");

        var superAdminAppUser = new ApplicationUser
        {
            Id = superAdminId.ToString(),
            UserName = "superadmin",
            Email = "superadmin@unsa.ba",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(superAdminAppUser, "Test123!");

        context.DomainUsers.Add(
            User.Create(
                "superadmin",
                string.Empty,
                "Super",
                "Admin",
                "superadmin@unsa.ba",
                tenantId,
                UserRole.Superadmin
            ).SetId(superAdminId)
        );

        var adminAppUser = new ApplicationUser
        {
            Id = adminId.ToString(),
            UserName = "admin",
            Email = "admin@unsa.ba",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminAppUser, "Test123!");

        context.DomainUsers.Add(
            User.Create(
                "admin",
                string.Empty,
                "Admin",
                "User",
                "admin@unsa.ba",
                tenantId,
                UserRole.Admin
            ).SetId(adminId)
        );

        var teacherAppUser = new ApplicationUser
        {
            Id = teacherId.ToString(),
            UserName = "teacher",
            Email = "emir.buza@unsa.ba",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(teacherAppUser, "Test123!");

        context.DomainUsers.Add(
            User.Create(
                "teacher",
                string.Empty,
                "Emir",
                "Buza",
                "emir.buza@unsa.ba",
                tenantId,
                UserRole.Teacher
            ).SetId(teacherId)
        );

        var studentAppUser = new ApplicationUser
        {
            Id = studentId.ToString(),
            UserName = "student",
            Email = "niko.nikic@unsa.ba",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(studentAppUser, "Test123!");

        context.DomainUsers.Add(
            User.Create(
                "student",
                string.Empty,
                "Niko",
                "Nikic",
                "niko.nikic@unsa.ba",
                tenantId,
                UserRole.Student,
                "IB20001"
            ).SetId(studentId)
        );

        await context.SaveChangesAsync();
    }
}
