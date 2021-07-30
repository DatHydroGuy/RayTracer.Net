using System;
using System.IO;
using System.Text;

namespace RayTracer
{
    public class Canvas
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public Colour[,] Pixels { get; set; }

        public Canvas(int width = 100, int height = 80)
        {
            Width = width;
            Height = height;
            Pixels = new Colour[height, width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Pixels[y, x] = new Colour();
                }
            }
        }

        public void SetColour(Colour col)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    WritePixel(x, y, col);
                }
            }
        }

        public void WritePixel(int x, int y, Colour col)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;
            Pixels[y, x] = col;
        }

        public Colour PixelAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
                
            return Pixels[y, x];
        }
        
        public void WriteImageFile(string imageFileName)
        {
            ImageFileWriter writer;
            if (imageFileName.EndsWith(".ppm"))
            {
                writer = new PpmFileWriter(Pixels);
            }
            else
            {
                writer = new PngFileWriter(Pixels);
            }
            writer.WriteImageFile(imageFileName);
        }

        public Colour[,] CanvasFromPpm(string fileContents)
        {
            using var sr = new StringReader(fileContents);
            var header = sr.ReadLine();
            if (header != "P3")
                throw new Exception($"Incorrect PPM file header in line 0: {header}");
            return new[,] { { Colour.RED } };
        }
    }
}
