using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.Platform.Modules;
using Xunit;

namespace Module1.Test
{
    [Trait("Category", "CI")]
    public class AssemblyLoadTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadAssemblyFrom_ThirdPartyAssemblyInDependentProject_IsLoadedCorrectly(bool isDevelopmentEnvironment)
        {
            // Arrange
            var loggerStub = Mock.Of<ILogger<LoadContextAssemblyResolver>>();
            var resolver = new LoadContextAssemblyResolver(loggerStub, isDevelopmentEnvironment);

            // Act
            var webRuntimeAssembly = LoadAssemblyInRuntime(resolver);
            // To ensure assembly is loaded correctly with all recursive dependencies, need to instantiate controller and call the method that call third party dll functions
            var stringResult = CallControllerMethod(webRuntimeAssembly);

            // Assert
            Assert.NotNull(stringResult);
        }

        private static Assembly LoadAssemblyInRuntime(LoadContextAssemblyResolver resolver)
        {
            // We need to load assembly that is not referenced by our project, thats why physical path is hardcoded
            var Module1WebModulePath = $"{AppContext.BaseDirectory}..\\..\\..\\Module1.Web\\bin\\netcoreapp2.1\\Module1.Web.dll";
            var webRuntimeAssembly = resolver.LoadAssemblyFrom(Module1WebModulePath);
            return webRuntimeAssembly;
        }

        private static string CallControllerMethod(Assembly webRuntimeAssembly)
        {
            // We need to use type and method string names instead of nameof() here as we should not have the reference to these assemblies for proper load testing
            const string ControllerTypeFullName = "Module1.Web.Controllers.ThirdPartyController";
            const string ControllerMethodName = "Execute3rdPartyCodeFromDifferentProject";
            const string ControllerMethodSampleArgument = "Sample message.";

            var controllerType = webRuntimeAssembly.GetType(ControllerTypeFullName);
            var controllerInstance = Activator.CreateInstance(controllerType);
            var method = controllerType.GetMethod(ControllerMethodName);
            return (string)method.Invoke(controllerInstance, new object[] { ControllerMethodSampleArgument });
        }
    }
}
