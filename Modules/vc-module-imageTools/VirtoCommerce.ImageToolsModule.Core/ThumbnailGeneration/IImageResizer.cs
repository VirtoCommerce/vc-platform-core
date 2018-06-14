using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public interface IImageResizer
    {
        /// <summary>
        /// Scale image by given percent
        /// </summary>
        Image ScaleByPercent(Image image, int percent);

        /// <summary>
        /// Resize image vertically with keeping it aspect rate.
        /// </summary>
        Image FixedHeight(Image image, int height, Color backgroung);

        /// <summary>
        /// Resize image horizontally with keeping it aspect rate
        /// </summary>
        Image FixedWidth(Image image, int width, Color backgroung);

        /// <summary>
        /// Resize image.
        /// Original image will be resized proportionally to fit given Width, Height.
        /// Original image will not be cropped.
        /// If the original image has an aspect ratio different from thumbnail then thumbnail will contain empty spaces (top and bottom or left and right). 
        /// The empty spaces will be filled with given color.
        /// </summary>
        Image FixedSize(Image image, int width, int height, Color backgroung);

        /// <summary>
        /// Resize and trim excess.
        /// The image will have given size
        /// </summary>
        Image Crop(Image image, int width, int height, AnchorPosition anchor);
    }
}
