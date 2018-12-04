using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.ModuleFileCollector;
using VirtoCommerce.Platform.Core.ModuleFileCollector.Normalizer;
using VirtoCommerce.Platform.Core.VersionProvider;
using Xunit;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class ScriptCollectorTest
    {
        private string _destinationPath { get; set; }
        private string _temporaryFolderName { get; set; }
        private string _fullPhysicalPathToDistFolder { get; set; }

        [Fact]
        public void TestCollectorWithOneModuleWithAppAndVendorNoVersionAppend()
        {
            CreateTemporaryFiles(new[] { "app.js", "vendor.js" });

            var fileVersionProvider = CreateMockedFileVersionProvider();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new []{ CreateScriptBundleItem(_temporaryFolderName) })
            });

            var pathNormalizer = CreateMockedModuleFilePathNormalizer();

            var collector = CreateScriptCollector(
                fileVersionProvider.Object,
                localModuleCatalog.Object,
                pathNormalizer.Object
            );

            var result = collector.Collect(false);

            TryRemoveTemporaryFiles();

            Assert.Equal(2, result.Length);
            fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestCollectorAppendVersionCallVersionProvider()
        {
            CreateTemporaryFiles(new []{ "app.js" });

            var fileVersionProvider = CreateMockedFileVersionProvider();
            var pathNormalizer = CreateMockedModuleFilePathNormalizer();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new []{ CreateScriptBundleItem(_temporaryFolderName) })
            });

            var collector = CreateScriptCollector(fileVersionProvider.Object, localModuleCatalog.Object, pathNormalizer.Object);

            collector.Collect(true);

            TryRemoveTemporaryFiles();

            fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void TestCollectorWithoutScripts()
        {
            var fileVersionProvider = CreateMockedFileVersionProvider();
            var pathNormalizer = CreateMockedModuleFilePathNormalizer();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new ManifestBundleItem[]{})
            });

            var collector = CreateScriptCollector(fileVersionProvider.Object, localModuleCatalog.Object, pathNormalizer.Object);

            var scripts = collector.Collect(true);

            Assert.Empty(scripts);
            fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestWrongFilenames()
        {
            CreateTemporaryFiles(new []{ "wrongApp.js", "wrongVendor.js" });

            var fileVersionProvider = CreateMockedFileVersionProvider();
            var pathNormalizer = CreateMockedModuleFilePathNormalizer();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new [] { CreateScriptBundleItem(_temporaryFolderName) })
            });

            var collector = CreateScriptCollector(fileVersionProvider.Object, localModuleCatalog.Object, pathNormalizer.Object);

            var result = collector.Collect(default(bool));

            TryRemoveTemporaryFiles();

            Assert.Empty(result);
        }

        [Fact]
        public void TestMultipleScriptFolder()
        {
            CreateTemporaryFiles(new [] { "app.js", "vendor.js" });

            var fileVersionProvider = CreateMockedFileVersionProvider();
            var pathNormalizer = CreateMockedModuleFilePathNormalizer();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new [] { CreateScriptBundleItem(_temporaryFolderName), CreateScriptBundleItem(_temporaryFolderName) })
            });

            var collector = CreateScriptCollector(fileVersionProvider.Object, localModuleCatalog.Object, pathNormalizer.Object);

            Assert.Throws<InvalidOperationException>(() => collector.Collect(default(bool)));

            TryRemoveTemporaryFiles();
        }

        [Fact]
        public void TestDestinationFolderWithoutDist()
        {
            CreateTemporaryFiles(new[] { "app.js", "vendor.js" }, distFolderName: "wrongDist");

            var fileVersionProvider = CreateMockedFileVersionProvider();
            var pathNormalizer = CreateMockedModuleFilePathNormalizer();
            var localModuleCatalog = CreateMockedLocalModuleCatalog();

            localModuleCatalog.Setup(l => l.Modules).Returns(new List<ModuleInfo>
            {
                CreateModuleInfo(_destinationPath, new [] { CreateScriptBundleItem(_temporaryFolderName) })
            });

            var collector = CreateScriptCollector(fileVersionProvider.Object, localModuleCatalog.Object, pathNormalizer.Object);

            var result = collector.Collect(default(bool));

            TryRemoveTemporaryFiles();

            Assert.Empty(result);
        }

        private ScriptCollector CreateScriptCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog, IModuleFilePathNormalizer pathNormalizer)
        {
            return new ScriptCollector(
                fileVersionProvider,
                localModuleCatalog,
                CreateScriptPathNormalizerFactory(pathNormalizer)
            );
        }

        private Mock<IFileVersionProvider> CreateMockedFileVersionProvider()
        {
            return new Mock<IFileVersionProvider>();
        }

        private Mock<ILocalModuleCatalog> CreateMockedLocalModuleCatalog()
        {
            return new Mock<ILocalModuleCatalog>();
        }

        private Mock<IModuleFilePathNormalizer> CreateMockedModuleFilePathNormalizer()
        {
            return new Mock<IModuleFilePathNormalizer>();
        }

        private IModuleFilePathNormalizerFactory CreateScriptPathNormalizerFactory(IModuleFilePathNormalizer normalizer)
        {
            var factory = new Mock<IModuleFilePathNormalizerFactory>();

            factory
                .Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(normalizer);

            return factory.Object;
        }

        private void CreateTemporaryFiles(string[] fileNames, string distFolderName = null)
        {
            var tempPath = Path.GetTempPath();

            _temporaryFolderName = Guid.NewGuid().ToString();
            _destinationPath = tempPath;
            _fullPhysicalPathToDistFolder = Path.Join(tempPath, _temporaryFolderName, distFolderName);
            
            Directory.CreateDirectory(_fullPhysicalPathToDistFolder);

            foreach (var fileName in fileNames)
            {
                File.Create(Path.Join(_fullPhysicalPathToDistFolder, fileName)).Dispose();
            }
            
        }

        private void TryRemoveTemporaryFiles()
        {
            if (null == _fullPhysicalPathToDistFolder)
            {
                return;
            }

            var files = Directory.GetFiles(_fullPhysicalPathToDistFolder);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            Directory.Delete(Path.Join(_destinationPath, _temporaryFolderName), true);
        }

        private static ModuleInfo CreateModuleInfo(string fullPhysicalPath, ManifestBundleItem[] scripts)
        {
            return new ManifestModuleInfo(new ModuleManifest
            {
                Version = "1.0.0",
                PlatformVersion = "1.0.0",
                Scripts = scripts
            })
            {
                FullPhysicalPath = fullPhysicalPath,
                Assembly = Assembly.GetAssembly(typeof(ManifestModuleInfo)),
                ModuleName = "ModuleName"
            };
        }

        private static ManifestBundleItem CreateScriptBundleItem(string virtualPath)
        {
            return new ManifestBundleItem
            {
                VirtualPath = virtualPath
            };
        }
    }
}
