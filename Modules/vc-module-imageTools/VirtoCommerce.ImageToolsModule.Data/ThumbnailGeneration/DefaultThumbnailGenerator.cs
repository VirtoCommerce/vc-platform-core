using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    /// <summary>
    /// Generates thumbnails by certain criteria
    /// </summary>
    public class DefaultThumbnailGenerator : IThumbnailGenerator
    {
        private readonly object _progressLock = new object();

        private readonly IImageService _imageService;
        private readonly IImageResizer _imageResizer;

        public DefaultThumbnailGenerator(IImageService storageProvider, IImageResizer imageResizer)
        {
            _imageService = storageProvider;
            _imageResizer = imageResizer;
        }

        /// <summary>
        /// Generates thumbnails asynchronously
        /// </summary>
        /// <param name="sourcePath">Path to source picture</param>
        /// <param name="destPath">Target for generated thumbnail</param>
        /// <param name="options">Represents generation options</param>
        /// <param name="token">Allows cancel operation</param>
        /// <returns></returns>
        public async Task<ThumbnailGenerationResult> GenerateThumbnailsAsync(string sourcePath, string destPath, IList<ThumbnailOption> options, ICancellationToken token)
        {
            token?.ThrowIfCancellationRequested();

            var originalImage = await _imageService.LoadImageAsync(sourcePath);
            if (originalImage == null)
            {
                return new ThumbnailGenerationResult
                {
                    Errors = { $"Cannot generate thumbnail: {sourcePath} does not have a valid image format" }
                };
            }

            var result = new ThumbnailGenerationResult();

            var format = _imageService.GetImageFormat(originalImage);

            //one process only can use an Image object at the same time.
            Image clone;
            lock (_progressLock)
            {
                clone = (Image)originalImage.Clone();
            }

            foreach (var option in options)
            {
                var thumbnail = GenerateThumbnail(clone, option);
                var thumbnailUrl = sourcePath.GenerateThumbnailName(option.FileSuffix);

                if (thumbnail != null)
                {
                    await _imageService.SaveImage(thumbnailUrl, thumbnail, format);
                }
                else
                {
                    throw new Exception($"Cannot save thumbnail image {thumbnailUrl}");
                }

                result.GeneratedThumbnails.Add(thumbnailUrl);
            }

            return result;
        }

        /// <summary>
        ///Generates a Thumbnail
        /// </summary>
        /// <param name="image"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        protected virtual Image GenerateThumbnail(Image image, ThumbnailOption option)
        {
            var height = option.Height ?? image.Height;
            var width = option.Width ?? image.Width;
            var color = ColorTranslator.FromHtml(option.BackgroundColor);

            Image thumbnail = null;
            switch (option.ResizeMethod)
            {
                case ResizeMethod.FixedSize:
                    thumbnail = _imageResizer.FixedSize(image, width, height, color);
                    break;
                case ResizeMethod.FixedWidth:
                    thumbnail = _imageResizer.FixedWidth(image, width, color);
                    break;
                case ResizeMethod.FixedHeight:
                    thumbnail = _imageResizer.FixedHeight(image, height, color);
                    break;
                case ResizeMethod.Crop:
                    thumbnail = _imageResizer.Crop(image, width, height, option.AnchorPosition);
                    break;
            }
            return thumbnail;
        }
    }
}
