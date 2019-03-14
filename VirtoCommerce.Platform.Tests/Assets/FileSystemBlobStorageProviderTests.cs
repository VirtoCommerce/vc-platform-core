using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.Platform.Assets.FileSystem;
using Xunit;

namespace VirtoCommerce.Platform.Tests.Assets
{
    [Trait("Category", "Unit")]
    public class BlobStorageProviderTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly IOptions<FileSystemBlobContentOptions> _options;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public BlobStorageProviderTests()
        {
            var tempPath = Path.GetTempPath();
            _tempDirectory = Path.Combine(tempPath, "FileSystemBlobProviderTests");
            Directory.CreateDirectory(_tempDirectory);

            _options = BuildOptions(_tempDirectory);
            _httpContextAccessor = BuildHttpContextAccessor();
        }

        private static IOptions<FileSystemBlobContentOptions> BuildOptions(string tempDirectory)
        {
            var blobContentOptions = new FileSystemBlobContentOptions
            {
                PublicPath = "some-public-path",
                RootPath = tempDirectory
            };
            return new OptionsWrapper<FileSystemBlobContentOptions>(blobContentOptions);
        }

        private static Mock<IHttpContextAccessor> BuildHttpContextAccessor()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(new HostString("some-test-host.testcompany.com"));

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request).Returns(request.Object);

            var result = new Mock<IHttpContextAccessor>();
            result.Setup(x => x.HttpContext).Returns(httpContext.Object);

            return result;
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }

        /// <summary>
        /// `OpenWrite` method should return write-only stream.
        /// </summary>
        [Fact]
        public void FileSystemBlobProviderStreamWritePermissionsTest()
        {
            var fsbProvider = new FileSystemBlobProvider(_options, _httpContextAccessor.Object);

            using (var actualStream = fsbProvider.OpenWrite("file-write.tmp"))
            {
                Assert.True(actualStream.CanWrite, "'OpenWrite' stream should be writable.");
                Assert.False(actualStream.CanRead, "'OpenWrite' stream should be write-only.");
            }
        }

        /// <summary>
        /// `OpenRead` method should return read-only stream.
        /// </summary>
        [Fact]
        public void FileSystemBlobProviderStreamReadPermissionsTest()
        {
            var fsbProvider = new FileSystemBlobProvider(_options, _httpContextAccessor.Object);
            const string fileForRead = "file-read.tmp";

            // Creating empty file.
            File.WriteAllText(Path.Combine(_tempDirectory, fileForRead), string.Empty);

            using (var actualStream = fsbProvider.OpenRead(fileForRead))
            {
                Assert.True(actualStream.CanRead, "'OpenRead' stream should be readable.");
                Assert.False(actualStream.CanWrite, "'OpenRead' stream should be read-only.");
            }
        }
    }
}
