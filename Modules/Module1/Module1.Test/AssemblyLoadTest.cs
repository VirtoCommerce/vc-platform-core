using System;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.Platform.Modules;
using Xunit;

namespace Module1.Test
{
    [Trait("Category", "CI")]
    public class AssemblyLoadTest
    {
        [Fact]
        public void TestAssemblyLoadThroughReflection()
        {
            var logger = Mock.Of<ILogger<LoadContextAssemblyResolver>>();
            var resolver = new LoadContextAssemblyResolver(logger);
            var webRuntimeAssembly = resolver.LoadAssemblyFrom($"{AppContext.BaseDirectory}..\\..\\..\\Module1.Web\\bin\\netcoreapp2.1\\Module1.Web.dll");

            // Explicit loading of "ICSharpCode.SharpZipLib.dll" (dependency of referenced assembly Module1.Data) to default ALC would make test passing even without proper dependency loading:
            // System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(Environment.ExpandEnvironmentVariables(@"%userprofile%\.nuget\packages\sharpziplib\1.1.0\lib\netstandard2.0\ICSharpCode.SharpZipLib.dll"));
            var controllerType = webRuntimeAssembly.GetType("Module1.Web.Controllers.ThirdPartyController");
            Assert.NotNull(controllerType);
            var controllerInstance = Activator.CreateInstance(controllerType);
            var method = controllerType.GetMethod("Execute3rdPartyCodeFromDifferentProject");
            Assert.NotNull(method);
            var stringResult = (string)method.Invoke(controllerInstance, new object[] { "Aloha!" });

            Assert.NotNull(stringResult);
        }
    }
}
