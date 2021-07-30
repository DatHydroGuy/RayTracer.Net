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
            var red = (int) (pixel.Red * 255);
            var green = (int) (pixel.Green * 255);
            var blue = (int) (pixel.Blue * 255);
            return Color.FromArgb(red, green, blue);
        }
    }
}
