using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration;
using Xunit;

namespace VirtoCommerce.ImageToolsModule.Tests
{
    public class ImageResizerTests
    {
        private class ImageResizerTestClass : ImageResizer
        {
            public Image<Rgba32> TransformTest(Image<Rgba32> original, Rectangle source, Rectangle destination, Size canvasSize, Rgba32? backgroundColor)
            {
                return Transform(original, source, destination, canvasSize, backgroundColor);
            }
        }

        [Fact]
        public void ScaleByPercent()
        {
            var resizer = new ImageResizerTestClass();
            var image = new Image<Rgba32>(10, 100);
            var result = resizer.ScaleByPercent(image, 20);
            Assert.True(result.Width == 2 && result.Height == 20);
        }

        [Fact]
        public void FixedHeight()
        {
            var resizer = new ImageResizerTestClass();
            var image = new Image<Rgba32>(10, 100);
            var color = Rgba32.Green;
            var result = resizer.FixedHeight(image, 20, color);
            Assert.True(result.Width == 2 && result.Height == 20);
        }

        [Fact]
        public void FixedWidth()
        {
            var resizer = new ImageResizerTestClass();
            var image = new Image<Rgba32>(100, 10);
            var color = Rgba32.Green;
            var result = resizer.FixedWidth(image, 20, color);
            Assert.True(result.Width == 20 && result.Height == 2);
        }

        [Fact]
        public void FixedSize()
        {
            var resizer = new ImageResizerTestClass();
            var image = new Image<Rgba32>(100, 100);
            var color = Rgba32.Green;
            var result = resizer.FixedSize(image, 20, 10, color);
            Assert.True(result.Width == 20 && result.Height == 10);
        }

        [Fact]
        public void Transform()
        {
            var resizer = new ImageResizerTestClass();
            var image = new Image<Rgba32>(100, 100);

            var source = new Rectangle(10, 10, 20, 20);
            var destination = new Rectangle(10, 10, 20, 20);
            var canvasSize = new Size(50, 50);

            var result = resizer.TransformTest(image, source, destination, canvasSize, null);
            Assert.True(result.Width == 50 && result.Height == 50);
        }
    }
}
