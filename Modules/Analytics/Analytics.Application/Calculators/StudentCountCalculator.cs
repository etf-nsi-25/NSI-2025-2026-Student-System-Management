using Analytics.Application.Interfaces;
using Analytics.Core.Entities;
using Analytics.Core.Interfaces;
using Analytics.Core.Constants;
using Common.Core.Tenant;
using System.Security.Principal;
using Identity.Application.Interfaces;
using Identity.Core.Enums;
using Identity.Core.DTO;
using System.Text.Json.Nodes;

namespace Analytics.Application.Calculators;

public class StudentCountCalculator : IStatsCalculator
{
    public string MetricCode => MetricKey.CountStudents;
    public Scope Scope => Scope.University;
    private readonly IUserService _userService;

    public StudentCountCalculator(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<JsonObject> CalculateAsync(Guid scopeIdentifier)
    {
        return new JsonObject
        {
            ["count"] = await _userService.CountUsers(new UserFilterRequest
            {
                Role = UserRole.Student
            })
        };
    }
}
