using Shouldly;
using NetArchTest.Rules;
using System.Reflection;

namespace Tests
{
    public class ArchitectureTests
    {
        private static readonly string[] Modules =
        [
            "Analytics",
            "Identity",
            "Faculty",
            "University",
            "Notifications",
            "Support"
        ];

        public static IEnumerable<object[]> ModuleNames()
        {
            foreach (var module in Modules)
                yield return new object[] { module };
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void Api_ShouldNotDependOn_Infrastructure(string module)
        {
            var api = Assembly.Load($"{module}.API");
            var infra = Assembly.Load($"{module}.Infrastructure");

            var result = Types.InAssembly(api)
                .Should()
                .NotHaveDependencyOn(infra.GetName().Name)
                .GetResult();

            result.IsSuccessful.ShouldBeTrue($"{module}: API should not depend on Infrastructure");
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void Core_ShouldNotDependOn_ApplicationInfrastructureOrAPI(string module)
        {
            var core = Assembly.Load($"{module}.Core");
            var app = Assembly.Load($"{module}.Application");
            var infra = Assembly.Load($"{module}.Infrastructure");
            var api = Assembly.Load($"{module}.API");

            Types.InAssembly(core)
                .Should()
                .NotHaveDependencyOn(app.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Core should not depend on Application");

            Types.InAssembly(core)
                .Should()
                .NotHaveDependencyOn(infra.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Core should not depend on Infrastructure");

            Types.InAssembly(core)
                .Should()
                .NotHaveDependencyOn(api.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Core should not depend on API");
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void Application_ShouldNotDependOnAPIOrInfrastructure(string module)
        {
            var app = Assembly.Load($"{module}.Application");
            var infra = Assembly.Load($"{module}.Infrastructure");
            var api = Assembly.Load($"{module}.API");

            Types.InAssembly(app)
                .Should()
                .NotHaveDependencyOn(api.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Application should not depend on API");

            Types.InAssembly(app)
                .Should()
                .NotHaveDependencyOn(infra.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Application should not depend on Infrastructure");
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void Infrastructure_ShouldNotDependOn_API(string module)
        {
            var infra = Assembly.Load($"{module}.Infrastructure");
            var api = Assembly.Load($"{module}.API");

            Types.InAssembly(infra)
                .Should()
                .NotHaveDependencyOn(api.GetName().Name)
                .GetResult()
                .IsSuccessful.ShouldBeTrue($"{module}: Infrastructure should not depend on API");
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void CoreInterfaces_ShouldHaveImplementations(string module)
        {
            var coreAssembly = Assembly.Load($"{module}.Core");
            var appAssembly = Assembly.Load($"{module}.Application");

            var coreInterfaces = coreAssembly
                .GetTypes()
                .Where(t => t.IsInterface && !t.IsGenericType)
                .ToList();

            var implementations = appAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var iface in coreInterfaces)
            {
                bool implemented = implementations.Any(c => iface.IsAssignableFrom(c));
                implemented.ShouldBeTrue(
                    $"{module}: Interface {iface.FullName} has no implementation in Application."
                );
            }
        }

        [Theory]
        [MemberData(nameof(ModuleNames))]
        public void CoreInterfaces_ShouldResideInInterfacesNamespace(string module)
        {
            var coreAssembly = Assembly.Load($"{module}.Core");

            var interfaces = coreAssembly.GetTypes().Where(t => t.IsInterface).ToList();

            foreach (var iface in interfaces)
            {
                iface.Namespace.ShouldNotBeNull($"{module}: Interface {iface.Name} has no namespace.");

                iface.Namespace.ShouldContain(".Interfaces", Case.Sensitive, $"{module}: Interface {iface.FullName} is not inside the '.Interfaces' namespace/folder.");
            }
        }
    }
}
