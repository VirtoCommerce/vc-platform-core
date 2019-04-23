using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public interface IImageResizer
    {
        /// <summary>
        /// Scale image by given percent
        /// </summary>
        Image<Rgba32> ScaleByPercent(Image<Rgba32> image, int percent);

        /// <summary>
        /// Resize image vertically with keeping it aspect rate.
        /// </summary>
        Image<Rgba32> FixedHeight(Image<Rgba32> image, int height, Rgba32 backgroung);

        /// <summary>
        /// Resize image horizontally with keeping it aspect rate
        /// </summary>
        Image<Rgba32> FixedWidth(Image<Rgba32> image, int width, Rgba32 backgroung);

        /// <summary>
        /// Resize image.
        /// Original image will be resized proportionally to fit given Width, Height.
        /// Original image will not be cropped.
        /// If the original image has an aspect ratio different from thumbnail then thumbnail will contain empty spaces (top and bottom or left and right). 
        /// The empty spaces will be filled with given color.
        /// </summary>
        Image<Rgba32> FixedSize(Image<Rgba32> image, int width, int height, Rgba32 backgroung);

        /// <summary>
        /// Resize and trim excess.
        /// The image will have given size
        /// </summary>
        Image<Rgba32> Crop(Image<Rgba32> image, int width, int height, AnchorPosition anchor);
    }
}
