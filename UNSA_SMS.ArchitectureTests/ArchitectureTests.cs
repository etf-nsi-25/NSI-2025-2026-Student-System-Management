namespace UNSA_SMS.ArchitectureTests;

using NetArchTest.Rules;
using Xunit;

public class ArchitectureTests
{
    private static System.Reflection.Assembly GetAssembly<T>() => typeof(T).Assembly;

    [Fact]
    public void Identity_Core_ShouldNotDependOn_Identity_Application()
    {
        var coreAssembly = GetAssembly<Identity.Core.Class1>();
        var appAssemblyName = GetAssembly<Identity.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Identity.Core should not depend on Identity.Application");
    }

    [Fact]
    public void Identity_Application_ShouldNotDependOn_Identity_Infrastructure()
    {
        var appAssembly = GetAssembly<Identity.Application.Class1>();
        var infraAssemblyName = GetAssembly<Identity.Infrastructure.Class1>().GetName().Name;

        var result = Types.InAssembly(appAssembly)
            .Should()
            .NotHaveDependencyOn(infraAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Identity.Application should not depend on Identity.Infrastructure");
    }

    [Fact]
    public void University_Core_ShouldNotDependOn_University_Application()
    {
        var coreAssembly = GetAssembly<University.Core.Class1>();
        var appAssemblyName = GetAssembly<University.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "University.Core should not depend on University.Application");
    }

    [Fact]
    public void Analytics_Core_ShouldNotDependOn_Analytics_Application()
    {
        var coreAssembly = GetAssembly<Analytics.Core.Class1>();
        var appAssemblyName = GetAssembly<Analytics.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Analytics.Core should not depend on Analytics.Application");
    }

    [Fact]
    public void Faculty_Core_ShouldNotDependOn_Faculty_Application()
    {
        var coreAssembly = GetAssembly<Faculty.Core.Class1>();
        var appAssemblyName = GetAssembly<Faculty.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Faculty.Core should not depend on Faculty.Application");
    }

    [Fact]
    public void Notifications_Core_ShouldNotDependOn_Notifications_Application()
    {
        var coreAssembly = GetAssembly<Notifications.Core.Class1>();
        var appAssemblyName = GetAssembly<Notifications.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Notifications.Core should not depend on Notifications.Application");
    }

    [Fact]
    public void Support_Core_ShouldNotDependOn_Support_Application()
    {
        var coreAssembly = GetAssembly<Support.Core.Class1>();
        var appAssemblyName = GetAssembly<Support.Application.Class1>().GetName().Name;

        var result = Types.InAssembly(coreAssembly)
            .Should()
            .NotHaveDependencyOn(appAssemblyName)
            .GetResult();

        Assert.True(result.IsSuccessful, "Support.Core should not depend on Support.Application");
    }
}