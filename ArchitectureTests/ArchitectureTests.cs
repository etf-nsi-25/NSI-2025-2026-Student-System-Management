namespace UNSA_SMS.ArchitectureTests;

using NetArchTest.Rules;
using Xunit;

public class ArchitectureTests
{
    private readonly string[] _modules = { "Identity", "Faculty", "Analytics", "Notifications", "Support", "University" };

    private static System.Reflection.Assembly GetAssembly(string module, string layer)
    {
        var assemblyName = $"{module}.{layer}";
        return System.Reflection.Assembly.Load(assemblyName);
    }

    [Fact]
    public void Core_ShouldNotDependOn_OtherLayers()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Core");
            var forbidden = new[] { $"{module}.Application", $"{module}.Infrastructure", $"{module}.API" };

            foreach (var dependency in forbidden)
            {
                var result = Types.InAssembly(assembly).Should().NotHaveDependencyOn(dependency).GetResult();
                Assert.True(result.IsSuccessful, $"{module}.Core should not depend on {dependency}");
            }
        }
    }

    [Fact]
    public void Application_ShouldNotDependOn_ApiOrInfrastructure()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Application");
            var forbidden = new[] { $"{module}.API", $"{module}.Infrastructure" };

            foreach (var dependency in forbidden)
            {
                var result = Types.InAssembly(assembly).Should().NotHaveDependencyOn(dependency).GetResult();
                Assert.True(result.IsSuccessful, $"{module}.Application should not depend on {dependency}");
            }
        }
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_API()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Infrastructure");
            var result = Types.InAssembly(assembly).Should().NotHaveDependencyOn($"{module}.API").GetResult();
            Assert.True(result.IsSuccessful, $"{module}.Infrastructure should not depend on {module}.API");
        }
    }

    [Fact]
    public void DtoClasses_ShouldHaveNameEndingWith_DTO()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Application");
            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Application.DTOs")
                .Should().HaveNameEndingWith("DTO")
                .GetResult();

            Assert.True(result.IsSuccessful, $"All DTO classes in {module}.Application.DTOs should end with 'DTO'");
        }
    }

    [Fact]
    public void Services_ShouldHaveNameEndingWith_Service()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Application");
            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Application.Services")
                .Should().HaveNameEndingWith("Service")
                .GetResult();

            Assert.True(result.IsSuccessful, $"All services in {module}.Application.Services should end with 'Service'");
        }
    }

    [Fact]
    public void Interfaces_ShouldHaveNameStartingWith_I()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Core");
            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Core.Interfaces").And().AreInterfaces()
                .Should().HaveNameStartingWith("I")
                .GetResult();

            Assert.True(result.IsSuccessful, $"All interfaces in {module}.Core.Interfaces should start with 'I'");
        }
    }

    [Fact]
    public void Services_ShouldBeClassesOrInterfaces()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Application");

            var types = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Application.Services")
                .GetTypes();

            bool allAreClassOrInterface = types.All(t => t.IsClass || t.IsInterface);

            Assert.True(allAreClassOrInterface, $"All types in {module}.Application.Services should be classes or interfaces");
        }
    }

    [Fact]
    public void Modules_ShouldNotDependOnEachOther()
    {
        foreach (var module in _modules)
        {
            var otherModules = _modules.Where(m => m != module);

            foreach (var layer in new[] { "Core", "Application", "Infrastructure" })
            {
                var assembly = GetAssembly(module, layer);

                foreach (var other in otherModules)
                {
                    var forbidden = new[] { $"{other}.Infrastructure", $"{other}.API"};

                    foreach (var dependency in forbidden)
                    {
                        var result = Types.InAssembly(assembly).Should().NotHaveDependencyOn(dependency).GetResult();
                        Assert.True(result.IsSuccessful, $"{module}.{layer} should not depend on {dependency}");
                    }
                }
            }
        }
    }

    [Fact]
    public void Controllers_ShouldHaveNameEndingWith_Controller()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "API");

            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.API.Controllers")
                .Should().HaveNameEndingWith("Controller")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All controllers in {module}.API.Controllers should end with 'Controller'");
        }
    }

    [Fact]
    public void Repositories_ShouldHaveNameEndingWith_Repository()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Infrastructure");

            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Infrastructure.Repositories")
                .Should().HaveNameEndingWith("Repository")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All repositories in {module}.Infrastructure.Repositories should end with 'Repository'");
        }
    }

    [Fact]
    public void Entities_ShouldLiveIn_CoreEntities_Namespace()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Core");

            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Core.Entities")
                .Should().BeClasses()
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All entities in {module}.Core.Entities should be classes");
        }
    }

    [Fact]
    public void CqrsHandlers_ShouldFollowNamingConvention()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Application");

            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Application.Handlers")
                .Should().HaveNameEndingWith("Handler")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All CQRS handlers in {module}.Application.Handlers should end with 'Handler'");
        }
    }

    [Fact]
    public void DomainServices_ShouldFollowNamingConvention()
    {
        foreach (var module in _modules)
        {
            var assembly = GetAssembly(module, "Core");

            var result = Types.InAssembly(assembly)
                .That().ResideInNamespace($"{module}.Core.Services")
                .Should().HaveNameEndingWith("Service")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All domain services in {module}.Core.Services should end with 'Service'");
        }
    }


}