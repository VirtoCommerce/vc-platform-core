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
        public DefaultThumbnailGenerator(IImageService storageProvider, IImageResizer imageResizer)
        {
            ImageService = storageProvider;
            ImageResizer = imageResizer;
        }

        protected object ProgressLock { get; } = new object();

        protected IImageService ImageService { get; }
        protected IImageResizer ImageResizer { get; }

        /// <summary>
        /// Generates thumbnails asynchronously
        /// </summary>
        /// <param name="sourcePath">Path to source picture</param>
        /// <param name="destPath">Target for generated thumbnail</param>
        /// <param name="options">Represents generation options</param>
        /// <param name="token">Allows cancel operation</param>
        /// <returns></returns>
        public virtual async Task<ThumbnailGenerationResult> GenerateThumbnailsAsync(string sourcePath, string destPath, IList<ThumbnailOption> options, ICancellationToken token)
        {
            token?.ThrowIfCancellationRequested();

            var originalImage = await ImageService.LoadImageAsync(sourcePath);
            if (originalImage == null)
            {
                return new ThumbnailGenerationResult
                {
                    Errors = { $"Cannot generate thumbnail: {sourcePath} does not have a valid image format" }
                };
            }

            var result = new ThumbnailGenerationResult();

            //one process only can use an Image object at the same time.
            Image clone;
            lock (ProgressLock)
            {
                clone = (Image)originalImage.Clone();
            }

            foreach (var option in options)
            {
                var thumbnail = GenerateThumbnail(clone, option);
                var thumbnailUrl = sourcePath.GenerateThumbnailName(option.FileSuffix);

                if (thumbnail != null)
                {
                    await ImageService.SaveImageAsync(thumbnailUrl, thumbnail, clone.RawFormat, option.JpegQuality);
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
                    thumbnail = ImageResizer.FixedSize(image, width, height, color);
                    break;
                case ResizeMethod.FixedWidth:
                    thumbnail = ImageResizer.FixedWidth(image, width, color);
                    break;
                case ResizeMethod.FixedHeight:
                    thumbnail = ImageResizer.FixedHeight(image, height, color);
                    break;
                case ResizeMethod.Crop:
                    thumbnail = ImageResizer.Crop(image, width, height, option.AnchorPosition);
                    break;
            }
            return thumbnail;
        }
    }
}
