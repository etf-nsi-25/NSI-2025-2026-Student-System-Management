using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace UNSA_SMS.ArchitectureTests;

public class ArchitectureTests
{
    private static readonly List<Assembly> Assemblies = AssemblyHelper.GetAssemblies();

    private static readonly Assembly ApplicationAssembly = AssemblyHelper.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Application")!;


    [Fact]
    public void Core_Layers_Should_Not_Depend_On_Infrastructure_Layers()
    {
        var coreAssemblies = Assemblies
            .Where(a => a.FullName!.Contains(".Core"))
            .ToArray();

        var forbiddenDependencies = Assemblies
            .Where(a => a.FullName!.Contains(".Infrastructure"))
            .Select(a => a.GetName().Name!)
            .ToArray();

        var coreClasses = Types.InAssemblies(coreAssemblies);

        var result = coreClasses
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Core layers must not have a dependency on Infrastructure layers. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }

    [Fact]
    public void Infrastructure_Layers_Should_Depend_On_Core_And_Application()
    {
        var infrastructureAssemblies = Assemblies
            .Where(a => a.FullName!.Contains(".Infrastructure"))
            .ToArray();

        var infrastructureClasses = Types.InAssemblies(infrastructureAssemblies)
            .That()
            .ResideInNamespace("*.Infrastructure");
        
        var result = infrastructureClasses
            .Should()
            .OnlyHaveDependenciesOn(
                "System",
                "Microsoft",
                "Application",
                "*.Core",
                "UNSA_SMS.ArchitectureTests",
                "*.Infrastructure",
                "Common") 
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Infrastructure layers should only have dependencies on Core/Common or Application layers. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }

    [Fact]
    public void Application_Project_Should_Not_Depend_On_Infrastructure()
    {
        if (ApplicationAssembly == null) return;
        
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("*.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "The Application project (entry point) must not directly depend on Infrastructure layers. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }


    [Fact]
    public void Module_Core_Layers_Should_Not_Depend_On_Other_Module_Core_Layers()
    {
        var coreAssemblies = Assemblies.Where(a => a.FullName!.Contains(".Core")).ToArray();

        foreach (var assembly in coreAssemblies)
        {
            var moduleName = assembly.GetName().Name!.Split('.').First();
            
            var forbiddenDependencies = Assemblies
                .Where(a => a.FullName!.Contains(".Core") && !a.GetName().Name!.Contains(moduleName))
                .Select(a => a.GetName().Name!)
                .ToArray();

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenDependencies)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Module {moduleName} violates modular isolation (within the Core layer). Violations: " +
                string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
        }
    }
    
    [Fact]
    public void Common_Module_Should_Not_Depend_On_Other_Modules()
    {
        var commonAssembly = AssemblyHelper.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Common");

        if (commonAssembly == null)
        {
            Assert.True(true, "Common assembly not found, test skipped.");
            return;
        }

        var forbiddenDependencies = new List<string>
        {
            "Identity", "University", "Faculty", "Support", "Notifications", "Analytics"
        }.ToArray();
        
        var result = Types.InAssembly(commonAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "The Common module must not have a dependency on any other module. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }


    [Fact]
public void Core_Services_Must_Be_Defined_By_Interfaces()
{
    var coreAssemblies = Assemblies.Where(a => a.FullName!.Contains(".Core")).ToArray();
    
    var candidateClasses = Types.InAssemblies(coreAssemblies)
        .That()
        .ResideInNamespace("*.Core")
        .And()
        .HaveNameEndingWith("Service")
        .Or()
        .HaveNameEndingWith("Handler")
        .GetTypes(); 
    
    var classesToTest = candidateClasses
        .Where(t => !t.FullName!.StartsWith("Microsoft.") && !t.FullName.StartsWith("Azure."))
        .ToList();
    
    var failingTypes = classesToTest
        .Where(t => !t.IsInterface && t.GetInterfaces().Length == 0) 
        .ToList();

    Assert.True(failingTypes.Count == 0,
        "Core business logic classes (Service/Handler) must implement an interface for DI/IoC. Violations: " +
        string.Join(", ", failingTypes.Select(t => t.FullName)));
}
    
    [Fact]
    public void Interfaces_Must_Start_With_I()
    {
        var coreAssemblies = Assemblies.Where(a => a.FullName!.Contains(".Core")).ToArray();

        var result = Types.InAssemblies(coreAssemblies)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "All interfaces within Core layers must start with the 'I' prefix. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }
    
    [Fact]
    public void Repository_Interfaces_Must_Be_In_Core_Layers()
    {
        var coreAssemblies = Assemblies
            .Where(a => a.FullName!.Contains(".Core"))
            .ToArray();

        var result = Types.InAssemblies(coreAssemblies)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .BeInterfaces()
            .GetResult();

        Assert.True(result.IsSuccessful,
            "All Repository definitions (ending with 'Repository') in Core layers must be Interfaces (DIP). Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }
    
    [Fact]
    public void Repository_Implementations_Must_Be_In_Infrastructure_Layers()
    {
        var infrastructureAssemblies = Assemblies
            .Where(a => a.FullName!.Contains(".Infrastructure"))
            .ToArray();
            
        var result = Types.InAssemblies(infrastructureAssemblies)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .BeClasses()
            .GetResult();

        Assert.True(result.IsSuccessful,
            "All Repository implementations (ending with 'Repository') in Infrastructure layers must be Concrete Classes. Violations: " +
            string.Join(", ", result.FailingTypes ?? Enumerable.Empty<Type>()));
    }
}

public static class AssemblyHelper
{
    
    private static List<Assembly>? _assemblies;

    public static List<Assembly> GetAssemblies()
    {
        if (_assemblies != null)
        {
            return _assemblies;
        }

        _assemblies = new List<Assembly>();
        var loadedAssemblyNames = new HashSet<string>();
        
        var solutionDirectory = Directory.GetCurrentDirectory();
        while (!Directory.GetFiles(solutionDirectory, "*.sln").Any() && solutionDirectory != Path.GetPathRoot(solutionDirectory))
        {
            solutionDirectory = Directory.GetParent(solutionDirectory)?.FullName ?? solutionDirectory;
        }
        
        var assemblyPaths = Directory.GetFiles(solutionDirectory, "*.dll", SearchOption.AllDirectories)
            .Where(p => 
                (p.Contains(".Core.dll") || p.Contains(".Infrastructure.dll") || p.Contains("Application.dll") || p.Contains("Common.dll")) && 
                !p.Contains("ref\\") && 
                !p.Contains("xunit") && 
                !p.Contains("ArchitectureTests"))
            .ToList();

        foreach (var path in assemblyPaths)
        {
            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(path);
                
                if (loadedAssemblyNames.Contains(assemblyName.FullName))
                    continue;

                var assembly = Assembly.LoadFrom(path); 
                _assemblies.Add(assembly);
                loadedAssemblyNames.Add(assemblyName.FullName);
            }
            catch (BadImageFormatException)
            {

            }
            catch (Exception)
            {
           
            }
        }
        
        return _assemblies;
    }

    public static Assembly GetAssemblyByName(string name)
    {
        return GetAssemblies().First(a => a.GetName().Name == name);
    }
}