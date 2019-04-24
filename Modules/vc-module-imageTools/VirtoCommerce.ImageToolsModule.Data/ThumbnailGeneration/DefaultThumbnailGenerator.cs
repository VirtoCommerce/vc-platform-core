using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

            var originalImage = await ImageService.LoadImageAsync(sourcePath, out var format);
            if (originalImage == null)
            {
                return new ThumbnailGenerationResult
                {
                    Errors = { $"Cannot generate thumbnail: {sourcePath} does not have a valid image format" }
                };
            }

            var result = new ThumbnailGenerationResult();

            foreach (var option in options)
            {
                var thumbnail = GenerateThumbnail(originalImage, option);
                var thumbnailUrl = sourcePath.GenerateThumbnailName(option.FileSuffix);

                if (thumbnail != null)
                {
                    await ImageService.SaveImageAsync(thumbnailUrl, thumbnail, format, option.JpegQuality);
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
        protected virtual Image<Rgba32> GenerateThumbnail(Image<Rgba32> image, ThumbnailOption option)
        {
            var height = option.Height ?? image.Height;
            var width = option.Width ?? image.Width;

            var color = Rgba32.Transparent;
            if (!string.IsNullOrWhiteSpace(option.BackgroundColor))
            {
                color = Rgba32.FromHex(option.BackgroundColor);
            }

            Image<Rgba32> result;
            switch (option.ResizeMethod)
            {
                case ResizeMethod.FixedSize:
                    result = ImageResizer.FixedSize(image, width, height, color);
                    break;
                case ResizeMethod.FixedWidth:
                    result = ImageResizer.FixedWidth(image, width, color);
                    break;
                case ResizeMethod.FixedHeight:
                    result = ImageResizer.FixedHeight(image, height, color);
                    break;
                case ResizeMethod.Crop:
                    result = ImageResizer.Crop(image, width, height, option.AnchorPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"ResizeMethod {option.ResizeMethod.ToString()} not supported.");
            }

            return result;
        }
    }
}
