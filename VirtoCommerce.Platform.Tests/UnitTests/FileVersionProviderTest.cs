using System.IO;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;
using FileVersionProvider = VirtoCommerce.Platform.Core.VersionProvider.FileVersionProvider;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class FileVersionProviderTest
    {
        [Fact]
        public void TestVersionAppend()
        {
            var filePath = CreateTemporaryFile();

            var versionProvider = new FileVersionProvider();

            var actualHash = versionProvider.GetFileVersion(filePath);

            var expectedHash = GetFileHash(filePath);

            RemoveDirectoryAndFile(filePath);

            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void TestFileNotExists()
        {
            var versionProvider = new FileVersionProvider();

            var actual = versionProvider.GetFileVersion("wrongPath/test.txt");

            Assert.Null(actual);
        }

        [Fact(Skip = "TODO")]
        public void TestCaching()
        {

        }

        private string CreateTemporaryFile()
        {
            var fullPhysicalPath = Path.GetTempFileName();

            return fullPhysicalPath;
        }

        private void RemoveDirectoryAndFile(string filePath)
        {
            File.Delete(filePath);
        }

        private string GetFileHash(string fullFilePath)
        {
            using (var sha256 = CryptographyAlgorithms.CreateSHA256())
            using (var readStream = new FileInfo(fullFilePath).OpenRead())
            {
                return WebEncoders.Base64UrlEncode(sha256.ComputeHash(readStream));
            }
        }
    }
}
