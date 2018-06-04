using System.IO;
using VirtoCommerce.Platform.Core.FileManager;
using Xunit;

namespace VirtoCommerce.Platform.Tests.IntegrationTests
{
    public class FileManagerIntegrationTests
    {
        private readonly FileManager _fileManager;

        public FileManagerIntegrationTests()
        {
            _fileManager = new FileManager();
        }

        [Fact]
        public void CreateDirectory_CreateTestDirectory()
        {
            //Arrange
            var path = Path.GetFullPath("Test");

            //Act
            _fileManager.CreateDirectory(path);

            //Assert
            Assert.True(Directory.Exists(path));
        }

        [Fact]
        public void Delete_DeleteTestDirectory()
        {
            //Arrange
            var path = Path.GetFullPath("Test");

            //Act
            _fileManager.Delete(path);

            //Assert
            Assert.False(Directory.Exists(path));
        }

        [Fact]
        public void SafeDelete_CreateAndSafeDeleteDirectory()
        {
            //Arrange
            var testDir = Path.GetFullPath("TestWithSub");
            var subDir = Path.GetFullPath("TestWithSub\\Sub");

            //Act
            _fileManager.CreateDirectory(testDir);
            _fileManager.CreateDirectory(subDir);
            _fileManager.SafeDelete(testDir);

            //Assert
            Assert.False(Directory.Exists(subDir));
            Assert.False(Directory.Exists(testDir));
        }


    }
}
