using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Modules.External;
using Xunit;

namespace VirtoCommerce.Platform.Tests.Modularity
{
    public class ExternalModuleCatalogTests
    {
        [Theory]
        [InlineData("2.2.0", "", "2.12.0", 5)]
        [InlineData("3.2.0", "beta.1", "3.0.0", 5)]
        [InlineData("4.0.0", "", "3.0.0", 6)]
        public void PublishNewVersionTest(string version, string versionTag, string platformVersion, int expectedCount)
        {
            //Arrange
            var moduleManifest = new ModuleManifest
            {
                 Version = version,
                 VersionTag = versionTag,
                 PlatformVersion = platformVersion
            };

            var extModuleManifest = new ExternalModuleManifest
            {
                Id = "A",
                Versions = new List<ExternalModuleManifestVersion>
                 {
                     //2.x
                     new ExternalModuleManifestVersion
                        {
                             Version = "2.1.0",
                             VersionTag = "alpha.1",
                             PlatformVersion = "2.12.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "2.0.0",
                             PlatformVersion = "2.12.0"
                        },
                        //3.x
                        new ExternalModuleManifestVersion
                        {
                             Version = "3.2.0",
                             VersionTag = "alpha.1",
                             PlatformVersion = "3.0.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "3.1.0",
                             PlatformVersion = "3.0.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "3.0.0",
                             PlatformVersion = "3.0.0"
                        },
                    }
            };           

            //Act
            extModuleManifest.PublishNewVersion(moduleManifest);

            //Assert
            Assert.True(extModuleManifest.Versions.Count() == expectedCount);
            Assert.Contains(ExternalModuleManifestVersion.FromManifest(moduleManifest), extModuleManifest.Versions);
        }

        [Theory]
        [InlineData("2.12.0", "1.4.0")]
        [InlineData("3.1.0", "2.0.0")]      
        public void CreateDirectory_CreateTestDirectory(string platformVersion, string effectiveModuleVersion)
        {
            //Arrange
            PlatformVersion.CurrentVersion = SemanticVersion.Parse(platformVersion);
            var modules = new[]
            {
                new ExternalModuleManifest
                {
                    Id = "A",
                    Versions = new []
                    {
                        new ExternalModuleManifestVersion
                        {
                             Version = "1.5.0",
                             PlatformVersion = "2.14.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "1.4.0",
                             PlatformVersion = "2.12.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "1.3.0",
                             PlatformVersion = "2.12.0"
                        },
                        new ExternalModuleManifestVersion
                        {
                             Version = "2.0.0",
                             PlatformVersion = "3.0.0"
                        },
                    }
                }
             };

            //Act
            var extCatalog = CreateExternalModuleCatalog(modules);
            extCatalog.Load();

            //Assert
            var module = extCatalog.Modules.FirstOrDefault() as ManifestModuleInfo;
            Assert.NotNull(module);
            Assert.Equal(module.Version, SemanticVersion.Parse(effectiveModuleVersion));
        }

        private static ExternalModuleCatalog CreateExternalModuleCatalog(ExternalModuleManifest[] manifests)
        {
            var localModulesCatalog = new Moq.Mock<ILocalModuleCatalog>();
            localModulesCatalog.Setup(x => x.Modules).Returns(new List<ManifestModuleInfo>());
            var json = JsonConvert.SerializeObject(manifests);
            var client = new Moq.Mock<IExternalModulesClient>();
            client.Setup(x => x.OpenRead(Moq.It.IsAny<Uri>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes(json ?? "")));
            var logger = new Moq.Mock<ILogger<ExternalModuleCatalog>>();

            var options = Options.Create(new ExternalModuleCatalogOptions());
            var result = new ExternalModuleCatalog(localModulesCatalog.Object, client.Object, options, logger.Object);
            return result;
        }

    }
}
