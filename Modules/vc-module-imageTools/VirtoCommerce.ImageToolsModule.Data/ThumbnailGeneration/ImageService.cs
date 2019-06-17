using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    public class ImageService : IImageService
    {
        private readonly IBlobStorageProvider _storageProvider;
        public ImageService(IBlobStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }


        #region Implementation of IImageService

        /// <summary>
        /// Load to Image from blob.
        /// </summary>
        /// <param name="imageUrl">image url.</param>
        /// <param name="format">image format.</param>
        /// <returns>Image object.</returns>
        public virtual Task<Image<Rgba32>> LoadImageAsync(string imageUrl, out IImageFormat format)
        {
            try
            {
                using (var blobStream = _storageProvider.OpenRead(imageUrl))
                {
                    var result = Image.Load(blobStream, out format);
                    return Task.FromResult(result);
                }
            }
            catch (Exception)
            {
                format = null;
                return Task.FromResult<Image<Rgba32>>(null);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Save given image to blob storage.
        /// </summary>
        /// <param name="imageUrl">Image url.</param>
        /// <param name="image">Image object.</param>
        /// <param name="format">Image object format.</param>
        /// <param name="jpegQuality">Target image quality.</param>
        public virtual async Task SaveImageAsync(string imageUrl, Image<Rgba32> image, IImageFormat format, JpegQuality jpegQuality)
        {
            using (var blobStream = _storageProvider.OpenWrite(imageUrl))
            using (var stream = new MemoryStream())
            {
                if (format.DefaultMimeType == "image/jpeg")
                {
                    var options = new JpegEncoder
                    {
                        Quality = (int)jpegQuality
                    };

                    image.Save(stream, options);
                }
                else
                {
                    image.Save(stream, format);
                }
                stream.Position = 0;
                await stream.CopyToAsync(blobStream);
            }
        }

        #endregion
    }
}
