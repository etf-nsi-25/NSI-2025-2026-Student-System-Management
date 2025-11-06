
using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string AnalyticsNamespace = "Analytics";
    private const string CommonNamespace = "Common";
    private const string FacultyNamespace = "Faculty";
    private const string IdentityNamespace = "Identity";
    private const string NotificationsNamespace = "Notifications";
    private const string SupportNamespace = "Support";
    private const string UniversityNamespace = "University";

    private static readonly Assembly AnalyticsApplicationAssembly = typeof(Analytics.Application.Class1).Assembly;
    private static readonly Assembly AnalyticsCoreAssembly = typeof(Analytics.Core.Class1).Assembly;
    private static readonly Assembly AnalyticsInfrastructureAssembly = typeof(Analytics.Infrastructure.Class1).Assembly;
    private static readonly Assembly AnalyticsApiAssembly = typeof(Analytics.API.Controllers.AnalyticsController).Assembly;

    private static readonly Assembly CommonApplicationAssembly = typeof(Common.Application.Class1).Assembly;
    private static readonly Assembly CommonCoreAssembly = typeof(Common.Core.Class1).Assembly;
    private static readonly Assembly CommonInfrastructureAssembly = typeof(Common.Infrastructure.Class1).Assembly;
    private static readonly Assembly CommonApiAssembly = typeof(Common.API.Class1).Assembly;

    private static readonly Assembly FacultyApplicationAssembly = typeof(Faculty.Application.Class1).Assembly;
    private static readonly Assembly FacultyCoreAssembly = typeof(Faculty.Core.Class1).Assembly;
    private static readonly Assembly FacultyInfrastructureAssembly = typeof(Faculty.Infrastructure.Class1).Assembly;
    private static readonly Assembly FacultyApiAssembly = typeof(Faculty.API.Controllers.FacultyController).Assembly;

    private static readonly Assembly IdentityApplicationAssembly = typeof(Identity.Application.Class1).Assembly;
    private static readonly Assembly IdentityCoreAssembly = typeof(Identity.Core.Class1).Assembly;
    private static readonly Assembly IdentityInfrastructureAssembly = typeof(Identity.Infrastructure.Class1).Assembly;
    private static readonly Assembly IdentityApiAssembly = typeof(Identity.API.Controllers.IdentityController).Assembly;

    private static readonly Assembly NotificationsApplicationAssembly = typeof(Notifications.Application.Class1).Assembly;
    private static readonly Assembly NotificationsCoreAssembly = typeof(Notifications.Core.Class1).Assembly;
    private static readonly Assembly NotificationsInfrastructureAssembly = typeof(Notifications.Infrastructure.Class1).Assembly;
    private static readonly Assembly NotificationsApiAssembly = typeof(Notifications.API.Controllers.NotificationsController).Assembly;

    private static readonly Assembly SupportApplicationAssembly = typeof(Support.Application.Class1).Assembly;
    private static readonly Assembly SupportCoreAssembly = typeof(Support.Core.Class1).Assembly;
    private static readonly Assembly SupportInfrastructureAssembly = typeof(Support.Infrastructure.Class1).Assembly;
    private static readonly Assembly SupportApiAssembly = typeof(Support.API.Controllers.SupportController).Assembly;

    private static readonly Assembly UniversityApplicationAssembly = typeof(University.Application.Class1).Assembly;
    private static readonly Assembly UniversityCoreAssembly = typeof(University.Core.Class1).Assembly;
    private static readonly Assembly UniversityInfrastructureAssembly = typeof(University.Infrastructure.Class1).Assembly;
    private static readonly Assembly UniversityApiAssembly = typeof(University.API.Controllers.UniversityController).Assembly;


    [Fact]
    public void Core_Should_Not_HaveDependencyOnApplication()
    {
        var coreAssemblies = new[]
        {
            AnalyticsCoreAssembly,
            CommonCoreAssembly,
            FacultyCoreAssembly,
            IdentityCoreAssembly,
            NotificationsCoreAssembly,
            SupportCoreAssembly,
            UniversityCoreAssembly
        };

        var applicationAssemblyNames = new[]
        {
            AnalyticsApplicationAssembly.GetName().Name,
            CommonApplicationAssembly.GetName().Name,
            FacultyApplicationAssembly.GetName().Name,
            IdentityApplicationAssembly.GetName().Name,
            NotificationsApplicationAssembly.GetName().Name,
            SupportApplicationAssembly.GetName().Name,
            UniversityApplicationAssembly.GetName().Name
        };

        var result = Types.InAssemblies(coreAssemblies)
            .Should()
            .NotHaveDependencyOnAll(applicationAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Core_Should_Not_HaveDependencyOnInfrastructure()
    {
        var coreAssemblies = new[]
        {
            AnalyticsCoreAssembly,
            CommonCoreAssembly,
            FacultyCoreAssembly,
            IdentityCoreAssembly,
            NotificationsCoreAssembly,
            SupportCoreAssembly,
            UniversityCoreAssembly
        };

        var infrastructureAssemblyNames = new[]
        {
            AnalyticsInfrastructureAssembly.GetName().Name,
            CommonInfrastructureAssembly.GetName().Name,
            FacultyInfrastructureAssembly.GetName().Name,
            IdentityInfrastructureAssembly.GetName().Name,
            NotificationsInfrastructureAssembly.GetName().Name,
            SupportInfrastructureAssembly.GetName().Name,
            UniversityInfrastructureAssembly.GetName().Name
        };

        var result = Types.InAssemblies(coreAssemblies)
            .Should()
            .NotHaveDependencyOnAll(infrastructureAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnInfrastructure()
    {
        var applicationAssemblies = new[]
        {
            AnalyticsApplicationAssembly,
            CommonApplicationAssembly,
            FacultyApplicationAssembly,
            IdentityApplicationAssembly,
            NotificationsApplicationAssembly,
            SupportApplicationAssembly,
            UniversityApplicationAssembly
        };

        var infrastructureAssemblyNames = new[]
        {
            AnalyticsInfrastructureAssembly.GetName().Name,
            CommonInfrastructureAssembly.GetName().Name,
            FacultyInfrastructureAssembly.GetName().Name,
            IdentityInfrastructureAssembly.GetName().Name,
            NotificationsInfrastructureAssembly.GetName().Name,
            SupportInfrastructureAssembly.GetName().Name,
            UniversityInfrastructureAssembly.GetName().Name
        };

        var result = Types.InAssemblies(applicationAssemblies)
            .Should()
            .NotHaveDependencyOnAll(infrastructureAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Core_Should_Not_HaveDependencyOnApi()
    {
        var coreAssemblies = new[]
        {
            AnalyticsCoreAssembly,
            CommonCoreAssembly,
            FacultyCoreAssembly,
            IdentityCoreAssembly,
            NotificationsCoreAssembly,
            SupportCoreAssembly,
            UniversityCoreAssembly
        };

        var apiAssemblyNames = new[]
        {
            AnalyticsApiAssembly.GetName().Name,
            CommonApiAssembly.GetName().Name,
            FacultyApiAssembly.GetName().Name,
            IdentityApiAssembly.GetName().Name,
            NotificationsApiAssembly.GetName().Name,
            SupportApiAssembly.GetName().Name,
            UniversityApiAssembly.GetName().Name
        };

        var result = Types.InAssemblies(coreAssemblies)
            .Should()
            .NotHaveDependencyOnAll(apiAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnApi()
    {
        var applicationAssemblies = new[]
        {
            AnalyticsApplicationAssembly,
            CommonApplicationAssembly,
            FacultyApplicationAssembly,
            IdentityApplicationAssembly,
            NotificationsApplicationAssembly,
            SupportApplicationAssembly,
            UniversityApplicationAssembly
        };

        var apiAssemblyNames = new[]
        {
            AnalyticsApiAssembly.GetName().Name,
            CommonApiAssembly.GetName().Name,
            FacultyApiAssembly.GetName().Name,
            IdentityApiAssembly.GetName().Name,
            NotificationsApiAssembly.GetName().Name,
            SupportApiAssembly.GetName().Name,
            UniversityApiAssembly.GetName().Name
        };

        var result = Types.InAssemblies(applicationAssemblies)
            .Should()
            .NotHaveDependencyOnAll(apiAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnApi()
    {
        var infrastructureAssemblies = new[]
        {
            AnalyticsInfrastructureAssembly,
            CommonInfrastructureAssembly,
            FacultyInfrastructureAssembly,
            IdentityInfrastructureAssembly,
            NotificationsInfrastructureAssembly,
            SupportInfrastructureAssembly,
            UniversityInfrastructureAssembly
        };

        var apiAssemblyNames = new[]
        {
            AnalyticsApiAssembly.GetName().Name,
            CommonApiAssembly.GetName().Name,
            FacultyApiAssembly.GetName().Name,
            IdentityApiAssembly.GetName().Name,
            NotificationsApiAssembly.GetName().Name,
            SupportApiAssembly.GetName().Name,
            UniversityApiAssembly.GetName().Name
        };

        var result = Types.InAssemblies(infrastructureAssemblies)
            .Should()
            .NotHaveDependencyOnAll(apiAssemblyNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_Should_BeInApiLayer()
    {
        var apiAssemblies = new[]
        {
            AnalyticsApiAssembly,
            CommonApiAssembly,
            FacultyApiAssembly,
            IdentityApiAssembly,
            NotificationsApiAssembly,
            SupportApiAssembly,
            UniversityApiAssembly
        };

        var result = Types.InAssemblies(apiAssemblies)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .Should()
            .BeClasses()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_Should_HaveNameEndingWithController()
    {
        var apiAssemblies = new[]
        {
            AnalyticsApiAssembly,
            CommonApiAssembly,
            FacultyApiAssembly,
            IdentityApiAssembly,
            NotificationsApiAssembly,
            SupportApiAssembly,
            UniversityApiAssembly
        };

        var result = Types.InAssemblies(apiAssemblies)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
