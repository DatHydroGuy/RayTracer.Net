using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracer
{
    public abstract class ImageFileWriter
    {
        protected Colour[,] Pixels { get; }
        protected int Width { get; }
        protected int Height { get; }

        protected ImageFileWriter(Colour[,] pixels)
        {
            Pixels = pixels;
            Height = pixels.GetLength(0);
            Width = pixels.GetLength(1);
        }

        public abstract void WriteImageFile(string imageFile);
    }
}
