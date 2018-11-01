using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    public class ImageService : IImageService
    {
        public ImageService(IBlobStorageProvider storageProvider)
        {
            StorageProvider = storageProvider;
        }

        protected IBlobStorageProvider StorageProvider { get; }

        #region Implementation of IImageService

        /// <summary>
        /// Load to Image from blob.
        /// </summary>
        /// <param name="imageUrl">image url.</param>
        /// <returns>Image object.</returns>
        public virtual async Task<Image> LoadImageAsync(string imageUrl)
        {
            try
            {
                using (var blobStream = StorageProvider.OpenRead(imageUrl))
                using (var stream = new MemoryStream())
                {
                    await blobStream.CopyToAsync(stream);
                    var result = Image.FromStream(stream);
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save given image to blob storage.
        /// </summary>
        /// <param name="imageUrl">Image url.</param>
        /// <param name="image">Image object.</param>
        /// <param name="format">Image object format.</param>
        /// <param name="quality">Target image quality.</param>
        public virtual async Task SaveImageAsync(string imageUrl, Image image, ImageFormat format, JpegQuality quality)
        {
            using (var blobStream = StorageProvider.OpenWrite(imageUrl))
            using (var stream = new MemoryStream())
            {
                if (format.Guid == ImageFormat.Jpeg.Guid)
                {
                    var codecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == format.Guid);
                    var encoderParams = new EncoderParameters
                    {
                        Param = new[] { new EncoderParameter(Encoder.Quality, (int)quality) }
                    };
                    image.Save(stream, codecInfo, encoderParams);
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
