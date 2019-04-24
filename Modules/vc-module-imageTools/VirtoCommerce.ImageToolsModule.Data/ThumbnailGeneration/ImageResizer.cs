using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
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
        public virtual Image<Rgba32> ScaleByPercent(Image<Rgba32> image, int percent)
        {
            var nPercent = (float)percent / 100;
            var newWidth = (int)(image.Width * nPercent);
            var newHeight = (int)(image.Height * nPercent);

            var result = image.Clone(ctx =>
             {
                 ctx.Resize(newWidth, newHeight);
             });

            return result;
        }

        /// <summary>
        /// Resize image vertically with keeping it aspect rate.
        /// </summary>
        public virtual Image<Rgba32> FixedHeight(Image<Rgba32> image, int height, Rgba32 backgroung)
        {
            var options = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size { Height = height, Width = image.Width }
            };

            var result = image.Clone(ctx =>
            {
                ctx.Resize(options);
                ctx.BackgroundColor(backgroung);
            });

            return result;
        }

        /// <summary>
        /// Resize image horizontally with keeping it aspect rate
        /// </summary>
        public virtual Image<Rgba32> FixedWidth(Image<Rgba32> image, int width, Rgba32 backgroung)
        {
            var options = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size { Height = image.Height, Width = width }
            };

            var result = image.Clone(ctx =>
            {
                ctx.Resize(options);
                ctx.BackgroundColor(backgroung);
            });

            return result;
        }

        /// <summary>
        /// Resize image.
        /// Original image will be resized proportionally to fit given Width, Height.
        /// Original image will not be cropped.
        /// If the original image has an aspect ratio different from thumbnail then thumbnail will contain empty spaces (top and bottom or left and right). 
        /// The empty spaces will be filled with given color.
        /// </summary>
        public virtual Image<Rgba32> FixedSize(Image<Rgba32> image, int width, int height, Rgba32 backgroung)
        {
            var options = new ResizeOptions
            {
                Mode = ResizeMode.Pad,
                Size = new Size { Height = height, Width = width }
            };

            var result = image.Clone(ctx =>
            {
                ctx.Resize(options);
                ctx.BackgroundColor(backgroung);
            });

            return result;
        }

        /// <summary>
        /// Resize and trim excess.
        /// The image will have given size
        /// </summary>
        public virtual Image<Rgba32> Crop(Image<Rgba32> image, int width, int height, AnchorPosition anchor)
        {
            var options = new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size { Height = height, Width = width },
                Position = GetAnchorPositionMode(anchor)
            };

            var result = image.Clone(ctx =>
            {
                ctx.Resize(options);
            });

            return result;
        }

        protected virtual Image<Rgba32> Transform(Image<Rgba32> original, Rectangle source, Rectangle destination, Size canvasSize, Rgba32? backgroundColor)
        {
            if (!backgroundColor.HasValue)
            {
                backgroundColor = Rgba32.Transparent;
            }

            var result = new Image<Rgba32>(new Configuration(), canvasSize.Width, canvasSize.Height, backgroundColor.Value);
            result.MetaData.HorizontalResolution = original.MetaData.HorizontalResolution;
            result.MetaData.VerticalResolution = original.MetaData.VerticalResolution;

            var imgToDraw = original.Clone(ctx =>
            {
                ctx.Crop(source);
                ctx.Resize(destination.Size);
                ctx.BackgroundColor(backgroundColor.Value);
            });

            result.Mutate(ctx =>
            {
                ctx.DrawImage(imgToDraw, destination.Location, new GraphicsOptions(true));
            });

            return result;
        }

        private AnchorPositionMode GetAnchorPositionMode(AnchorPosition anchorPosition)
        {
            switch (anchorPosition)
            {
                case AnchorPosition.TopLeft:
                    return AnchorPositionMode.TopLeft;
                case AnchorPosition.TopCenter:
                    return AnchorPositionMode.Top;
                case AnchorPosition.TopRight:
                    return AnchorPositionMode.TopRight;
                case AnchorPosition.CenterLeft:
                    return AnchorPositionMode.Left;
                case AnchorPosition.Center:
                    return AnchorPositionMode.Center;
                case AnchorPosition.CenterRight:
                    return AnchorPositionMode.Right;
                case AnchorPosition.BottomLeft:
                    return AnchorPositionMode.BottomLeft;
                case AnchorPosition.BottomCenter:
                    return AnchorPositionMode.Bottom;
                case AnchorPosition.BottomRight:
                    return AnchorPositionMode.BottomRight;
                default:
                    throw new ArgumentOutOfRangeException($"AnchorPosition {anchorPosition.ToString()} not supported.");
            }
        }
    }
}
