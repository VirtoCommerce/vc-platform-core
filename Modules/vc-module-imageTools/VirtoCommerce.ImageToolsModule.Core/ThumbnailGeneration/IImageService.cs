using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    /// <summary>
    /// Work with image
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Load to Image from blob.
        /// </summary>
        /// <param name="imageUrl">image url.</param>
        /// <param name="format">image format.</param>
        /// <returns>Image object.</returns>
        Task<Image<Rgba32>> LoadImageAsync(string imageUrl, out IImageFormat format);

        /// <summary>
        /// Save given image to blob storage.
        /// </summary>
        /// <param name="imageUrl">Image url.</param>
        /// <param name="image">Image object.</param>
        /// <param name="format">Image object format.</param>
        /// <param name="jpegQuality">Target image quality.</param>
        Task SaveImageAsync(string imageUrl, Image<Rgba32> image, IImageFormat format, JpegQuality jpegQuality);
    }
}
