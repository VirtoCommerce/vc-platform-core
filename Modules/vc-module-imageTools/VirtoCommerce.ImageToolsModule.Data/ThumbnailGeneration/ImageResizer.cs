using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    /// <summary>
    /// Image resize library
    /// </summary>
    public class ImageResizer : IImageResizer
    {
        /// <summary>
        /// Scale image by given percent
        /// </summary>
        public virtual Image ScaleByPercent(Image image, int percent)
        {
            var nPercent = (float)percent / 100;

            var source = new ImageDimensions { Width = image.Width, Height = image.Height };
            var destination = new ImageDimensions { Width = (int)(source.Width * nPercent), Height = (int)(source.Height * nPercent) };

            return Transform(image, source, destination, destination.Size, null);
        }

        /// <summary>
        /// Resize image vertically with keeping it aspect rate.
        /// </summary>
        public virtual Image FixedHeight(Image image, int height, Color backgroung)
        {
            var source = new ImageDimensions { Width = image.Width, Height = image.Height };
            var destination = new ImageDimensions();

            var nPercent = height / (float)source.Height;

            destination.Width = (int)(source.Width * nPercent);
            destination.Height = (int)(source.Height * nPercent);

            return Transform(image, source, destination, destination.Size, backgroung);
        }

        /// <summary>
        /// Resize image horizontally with keeping it aspect rate
        /// </summary>
        public virtual Image FixedWidth(Image image, int width, Color backgroung)
        {
            var source = new ImageDimensions { Width = image.Width, Height = image.Height };
            var destination = new ImageDimensions();

            var nPercent = width / (float)source.Width;

            destination.Width = (int)(source.Width * nPercent);
            destination.Height = (int)(source.Height * nPercent);

            return Transform(image, source, destination, destination.Size, backgroung);
        }

        /// <summary>
        /// Resize image.
        /// Original image will be resized proportionally to fit given Width, Height.
        /// Original image will not be cropped.
        /// If the original image has an aspect ratio different from thumbnail then thumbnail will contain empty spaces (top and bottom or left and right). 
        /// The empty spaces will be filled with given color.
        /// </summary>
        public virtual Image FixedSize(Image image, int width, int height, Color backgroung)
        {
            var source = new ImageDimensions { Width = image.Width, Height = image.Height };
            var destination = new ImageDimensions();

            float nPercent;
            var nPercentW = width / (float)source.Width;
            var nPercentH = height / (float)source.Height;

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destination.X = (int)((width - source.Width * nPercent) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destination.Y = (int)((height - source.Height * nPercent) / 2);
            }

            destination.Width = (int)(source.Width * nPercent);
            destination.Height = (int)(source.Height * nPercent);

            return Transform(image, source, destination, new Size { Height = height, Width = width }, backgroung);
        }

        /// <summary>
        /// Resize and trim excess.
        /// The image will have given size
        /// </summary>
        public virtual Image Crop(Image image, int width, int height, AnchorPosition anchor)
        {
            var source = new ImageDimensions { Width = image.Width, Height = image.Height };
            var destination = new ImageDimensions();

            float nPercent;
            var nPercentW = width / (float)source.Width;
            var nPercentH = height / (float)source.Height;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                if (anchor == AnchorPosition.TopLeft || anchor == AnchorPosition.TopCenter || anchor == AnchorPosition.TopRight)
                {
                    destination.Y = 0;
                }
                else if (anchor == AnchorPosition.BottomLeft || anchor == AnchorPosition.BottomCenter || anchor == AnchorPosition.BottomRight)
                {
                    destination.Y = (int)(height - source.Height * nPercent);
                }
                else
                {
                    destination.Y = (int)((height - source.Height * nPercent) / 2);
                }
            }
            else
            {
                nPercent = nPercentH;
                if (anchor == AnchorPosition.TopLeft || anchor == AnchorPosition.CenterLeft || anchor == AnchorPosition.BottomLeft)
                {
                    destination.X = 0;
                }
                else if (anchor == AnchorPosition.TopRight || anchor == AnchorPosition.CenterRight || anchor == AnchorPosition.BottomRight)
                {
                    destination.X = (int)(width - source.Width * nPercent);
                }
                else
                {
                    destination.X = (int)((width - source.Width * nPercent) / 2);
                }
            }

            destination.Width = (int)(source.Width * nPercent);
            destination.Height = (int)(source.Height * nPercent);

            return Transform(image, source, destination, new Size { Height = height, Width = width }, null);
        }

        protected virtual Image Transform(Image original, ImageDimensions source, ImageDimensions destination, Size canvasSize, Color? backgroundColor)
        {
            var bitmap = new Bitmap(canvasSize.Width, canvasSize.Height, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                if (backgroundColor != null)
                {
                    graphics.Clear(backgroundColor.Value);
                }
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(original, destination.Rectangle, source.Rectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }
    }
}
