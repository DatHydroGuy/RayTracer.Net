using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracer
{
    public class PngFileWriter : ImageFileWriter
    {
        public PngFileWriter(Colour[,] pixels) : base(pixels)
        {}

        public override void WriteImageFile(string pngFile)
        {
            var filename = pngFile.EndsWith(".png") ? pngFile : $"{pngFile}.png";
            using var b = new Bitmap(Width, Height);
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var currCol = ConvertColourToPngFormat(Pixels[y, x]);
                    b.SetPixel(x, y, currCol);
                }
            }
            b.Save(filename, ImageFormat.Png);
        }

        private static Color ConvertColourToPngFormat(Colour pixel)
        {
            var red = Math.Max(Math.Min((int) (pixel.Red * 255), 255), 0);
            var green = Math.Max(Math.Min((int) (pixel.Green * 255), 255), 0);
            var blue = Math.Max(Math.Min((int) (pixel.Blue * 255), 255), 0);
            return Color.FromArgb(red, green, blue);
        }
    }
}
