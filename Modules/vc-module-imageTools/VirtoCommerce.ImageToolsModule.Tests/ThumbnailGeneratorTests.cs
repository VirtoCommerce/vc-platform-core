using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.ImageToolsModule.Tests
{
    public class DefaultThumbnailGeneratorTests
    {

        [Fact]
        public async void GenerateThumbnailsAsync_CancellationTokenNotNull_CancellationFired()
        {
            var options = new List<ThumbnailOption>();
            var token = new CancellationToken(true);
            var tokenWrapper = new CancellationTokenWrapper(token);

            var mockStorage = new Mock<IImageService>();
            var mockResizer = new Mock<IImageResizer>();

            var target = new DefaultThumbnailGenerator(mockStorage.Object, mockResizer.Object);
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await target.GenerateThumbnailsAsync("http://pathToFile.bmp", "dest", options, tokenWrapper));
        }

        [Fact]
        public async void GenerateThumbnailsAsync_FixedSize_FixedSizeCalled()
        {
            var options = new List<ThumbnailOption>
            {
                 new ThumbnailOption
                 {
                     ResizeMethod = ResizeMethod.FixedSize,
                     FileSuffix = "test",
                 }
             };

            var image = new Image<Rgba32>(50, 50);
            IImageFormat format;

            var mockStorage = new Mock<IImageService>();
            mockStorage.Setup(x => x.LoadImageAsync(It.IsAny<string>(), out format))
                       .Returns(Task.FromResult(image));


            var mockResizer = new Mock<IImageResizer>();
            mockResizer.Setup(x => x.FixedSize(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Rgba32>())).Returns(image);

            var target = new DefaultThumbnailGenerator(mockStorage.Object, mockResizer.Object);
            var result = await target.GenerateThumbnailsAsync("http://pathToFile.bmp", "dest", options, null);

            mockResizer.Verify(x => x.FixedSize(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Rgba32>()));
        }

        [Fact]
        public async void GenerateThumbnailsAsync_Crop_CropCalled()
        {
            var options = new List<ThumbnailOption>
            {
                new ThumbnailOption
                {
                    ResizeMethod = ResizeMethod.Crop,
                    FileSuffix = "test",
                    Width = 10,
                    Height = 20
                }
            };

            var image = new Image<Rgba32>(50, 50);
            IImageFormat format;

            var mockStorage = new Mock<IImageService>();
            mockStorage.Setup(x => x.LoadImageAsync(It.IsAny<string>(), out format)).Returns(Task.FromResult(image));

            var mockResizer = new Mock<IImageResizer>();
            mockResizer.Setup(x => x.Crop(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AnchorPosition>())).Returns(image);

            var target = new DefaultThumbnailGenerator(mockStorage.Object, mockResizer.Object);
            var result = await target.GenerateThumbnailsAsync("http://pathToFile.bmp", "dest", options, null);

            mockResizer.Verify(x => x.Crop(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AnchorPosition>()));
        }

        [Fact]
        public async void GenerateThumbnailsAsync_FixedWidth_FFixedWidthCalled()
        {
            var options = new List<ThumbnailOption>
            {
                new ThumbnailOption
                {
                    ResizeMethod = ResizeMethod.FixedWidth,
                    FileSuffix = "test",
                }
            };

            var image = new Image<Rgba32>(50, 50);
            IImageFormat format;

            var mockStorage = new Mock<IImageService>();
            mockStorage.Setup(x => x.LoadImageAsync(It.IsAny<string>(), out format)).Returns(Task.FromResult(image));

            var mockResizer = new Mock<IImageResizer>();
            mockResizer.Setup(x => x.FixedWidth(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<Rgba32>())).Returns(image);

            var target = new DefaultThumbnailGenerator(mockStorage.Object, mockResizer.Object);
            var result = await target.GenerateThumbnailsAsync("http://pathToFile.bmp", "dest", options, null);

            mockResizer.Verify(x => x.FixedWidth(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<Rgba32>()));
        }

        [Fact]
        public async void GenerateThumbnailsAsync_FixedHeight_FixedHeightCalled()
        {
            var options = new List<ThumbnailOption>
            {
                new ThumbnailOption
                {
                    ResizeMethod = ResizeMethod.FixedHeight,
                    FileSuffix = "test",
                }
            };

            var image = new Image<Rgba32>(50, 50);
            IImageFormat format;

            var mockStorage = new Mock<IImageService>();
            mockStorage.Setup(x => x.LoadImageAsync(It.IsAny<string>(), out format)).Returns(Task.FromResult(image));

            var mockResizer = new Mock<IImageResizer>();
            mockResizer.Setup(x => x.FixedHeight(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<Rgba32>())).Returns(image);

            var target = new DefaultThumbnailGenerator(mockStorage.Object, mockResizer.Object);
            var result = await target.GenerateThumbnailsAsync("http://pathToFile.bmp", "dest", options, null);

            mockResizer.Verify(x => x.FixedHeight(It.IsAny<Image<Rgba32>>(), It.IsAny<int>(), It.IsAny<Rgba32>()));
        }
    }
}
