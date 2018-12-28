using System;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.Platform.Modules;
using Xunit;

namespace Module1.Test
{
    public class AssemblyLoadTest
    {
        [Fact]
        public void TestAssemblyLoadThroughReflection()
        {
            var logger = Mock.Of<ILogger<LoadContextAssemblyResolver>>();
            var resolver = new LoadContextAssemblyResolver(logger);
            var webRuntimeAssembly = resolver.LoadAssemblyFrom($"{AppContext.BaseDirectory}..\\..\\..\\Module1.Web\\bin\\netcoreapp2.1\\Module1.Web.dll");

            // Uncommenting this line makes the trick - test passes (as we explicitly load dependency of referenced assembly Module1.Data,
            // that is not loaded by LoadContextAssemblyResolver.LoadAssemblyFrom one level deps resolving now)
            // System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(@"C:\Users\yecli\.nuget\packages\sharpziplib\1.1.0\lib\netstandard2.0\ICSharpCode.SharpZipLib.dll");
            var controllerType = webRuntimeAssembly.GetType("Module1.Web.Controllers.ThirdPartyController");
            var controllerInstance = Activator.CreateInstance(controllerType);
            var stringResult = (string)controllerType.GetMethod("Execute3rdPartyCodeFromDifferentProject").Invoke(controllerInstance, new object[] { "Aloha!" });

            Assert.NotNull(stringResult);
        }
    }
}
