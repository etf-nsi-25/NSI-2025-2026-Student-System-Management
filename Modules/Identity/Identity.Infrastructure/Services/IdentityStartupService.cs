using Identity.Core.Enums;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services;

public class IdentityStartupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentityStartupService> _logger;

    public IdentityStartupService(IServiceProvider serviceProvider, ILogger<IdentityStartupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("IdentityStartupService is starting.");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var settings = EnvConfigProvider.GetSuperAdminSettings();

            if (!settings.IsValid())
            {
                _logger.LogWarning("SuperAdmin settings are incomplete in environment variables. Skipping SuperAdmin creation.");
                return;
            }

            var existingUser = await userManager.FindByEmailAsync(settings.Email);

            if (existingUser == null)
            {
                _logger.LogInformation("Creating SuperAdmin user: {Email}", settings.Email);

                var superAdmin = new ApplicationUser
                {
                    UserName = settings.Username,
                    Email = settings.Email,
                    FirstName = settings.FirstName,
                    LastName = settings.LastName,
                    EmailConfirmed = true,
                    Role = UserRole.Superadmin,
                    FacultyId = Guid.Empty,
                    Status = UserStatus.Active
                };

                var result = await userManager.CreateAsync(superAdmin, settings.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("SuperAdmin user created successfully.");
                }
                else
                {
                    _logger.LogError("Failed to create SuperAdmin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("SuperAdmin user already exists.");

                // Ensure the user has the Superadmin role if they exist but role is different
                if (existingUser.Role != UserRole.Superadmin)
                {
                    _logger.LogInformation("Updating existing user {Email} to Superadmin role.", existingUser.Email);
                    existingUser.Role = UserRole.Superadmin;
                    await userManager.UpdateAsync(existingUser);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred during the automatic SuperAdmin creation process. The application will continue to start, but SuperAdmin may not be available.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
