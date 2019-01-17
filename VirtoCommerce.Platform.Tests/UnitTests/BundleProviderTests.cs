using System;
using System.IO;
using Moq;
using VirtoCommerce.Platform.Core.VersionProvider;
using VirtoCommerce.Platform.Modules.Bundling;
using Xunit;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class BundleProviderTests : IDisposable
    {
        private string _destinationPath;
        private string _temporaryFolderName;
        private string _fullPhysicalPathToDistFolder;

        private readonly Mock<IFileVersionProvider> _fileVersionProvider;

        public BundleProviderTests()
        {
            _fileVersionProvider = CreateMockedFileVersionProvider();
        }

        [Fact]
        public void TestCollectorWithOneModuleWithAppAndVendorNoVersionAppend()
        {
            var file1 = "app.js";
            var file2 = "vendor.js";

            CreateTemporaryFiles(new[] { file1, file2 });

            var modulesMetadata = new[]
            {
                CreateModuleMetadata(_destinationPath, _temporaryFolderName, file1, file2)
            };

            var provider = CreateBundleProvider(_fileVersionProvider.Object);

            var result = provider.Collect(modulesMetadata, false);

            Assert.Equal(2, result.Length);
            _fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestCollectorAppendVersionCallVersionProvider()
        {
            var file1 = "app.js";

            CreateTemporaryFiles(new[] { file1 });

            var modulesMetadata = new[]
            {
                CreateModuleMetadata(_destinationPath, _temporaryFolderName, file1)
            };

            var provider = CreateBundleProvider(_fileVersionProvider.Object);

            provider.Collect(modulesMetadata, true);

            _fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void TestCollectorWithoutScripts()
        {
            var modulesMetadata = new[]
            {
                CreateModuleMetadata(_destinationPath, null, null)
            };

            var provider = CreateBundleProvider(_fileVersionProvider.Object);

            var scripts = provider.Collect(modulesMetadata, true);

            Assert.Empty(scripts);
            _fileVersionProvider.Verify(f => f.GetFileVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestComplexDestinationFolder()
        {
            var file1 = "app.js";
            var file2 = "vendor.js";
            var distFolder = "ComplexDist";

            CreateTemporaryFiles(new[] { file1, file2 }, distFolderName: distFolder);

            var modulesMetadata = new[]
            {
                CreateModuleMetadata(_destinationPath, $"{_temporaryFolderName}/{distFolder}", file1, file2)
            };

            var provider = CreateBundleProvider(_fileVersionProvider.Object);

            var result = provider.Collect(modulesMetadata, false);

            Assert.Equal(2, result.Length);
        }

        private BundleProvider CreateBundleProvider(IFileVersionProvider fileVersionProvider)
        {
            return new BundleProvider(fileVersionProvider);
        }

        private Mock<IFileVersionProvider> CreateMockedFileVersionProvider()
        {
            return new Mock<IFileVersionProvider>();
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

        private static ModuleMetadata CreateModuleMetadata(string destinationPath, string virtualPath, params string[] fileNames)
        {
            return new ModuleMetadata
            {
                FileNames = fileNames,
                FullPhysicalModulePath = destinationPath,
                VirtualPath = virtualPath,
                ModuleName = "ModuleName"
            };
        }

        public void Dispose()
        {
            if (_fullPhysicalPathToDistFolder == null)
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
    }
}
