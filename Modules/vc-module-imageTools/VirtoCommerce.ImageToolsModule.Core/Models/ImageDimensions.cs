using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VirtoCommerce.ImageToolsModule.Core.Models
{
    public class ImageDimensions
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

        public Size Size => new Size { Width = Width, Height = Height };
    }
}
