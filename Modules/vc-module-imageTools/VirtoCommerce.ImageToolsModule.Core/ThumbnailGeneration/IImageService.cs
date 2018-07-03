using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>Image object.</returns>
        Task<Image> LoadImageAsync(string imageUrl);

        /// <summary>
        /// Save given image to blob storage.
        /// </summary>
        /// <param name="imageUrl">Image url.</param>
        /// <param name="image">Image object.</param>
        /// <param name="format">Image object format.</param>
        Task SaveImageAsync(string imageUrl, Image image, ImageFormat format);

        /// <summary>
        /// Get image format by Image object.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        ImageFormat GetImageFormat(Image image);
    }
}
