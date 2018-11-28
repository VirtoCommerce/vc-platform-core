using System;
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
            const string fileName = "test.txt";

            var path = CreateTemporaryFile(fileName);

            var versionProvider = new FileVersionProvider();

            var actualHash = versionProvider.GetFileVersion(Path.Join(path, fileName));

            var expectedHash = GetFileHash(Path.Join(path, fileName));

            RemoveDirectoryAndFile(path, fileName);

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

        private string CreateTemporaryFile(string fileName)
        {
            var tempPath = Path.GetTempPath();

            var fullPhysicalPath = Path.Join(tempPath, Guid.NewGuid().ToString());

            Directory.CreateDirectory(fullPhysicalPath);

            File.Create(Path.Join(fullPhysicalPath, fileName)).Dispose();

            return fullPhysicalPath;
        }

        private void RemoveDirectoryAndFile(string dirFullPath, string fileName)
        {
            File.Delete(Path.Join(dirFullPath, fileName));

            Directory.Delete(dirFullPath);
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
