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

        public static Canvas CanvasFromPpm(string fileContents)
        {
            fileContents = RemoveCommentLines(fileContents);
            using var sr = new StringReader(fileContents);
            var header = sr.ReadLine();
            if (header != "P3")
                throw new Exception($"Incorrect PPM file header in line 0: {header}");
            var dimensions = sr.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (!int.TryParse(dimensions?[0], out var width))
                throw new Exception($"Cannot parse canvas width: {dimensions?[0]}");
            if (!int.TryParse(dimensions?[1], out var height))
                throw new Exception($"Cannot parse canvas height: {dimensions?[1]}");
            var colourMaxStr = sr.ReadLine();
            if (!double.TryParse(colourMaxStr, out var colourMax))
                throw new Exception($"Cannot parse maximum colour value: {colourMaxStr}");
            var canvas = new Canvas(width, height);

            var pixelArray = new string[width * 3 * height];
            
            // var pixelRow = new string[width * 3];
            var offset = 0;
            while (offset < width * 3 * height)
            {
                var row = sr.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (row == null)
                    throw new Exception($"Cannot parse colour values at index {offset}");
                Array.Copy(row, 0, pixelArray, offset, row.Length);
                offset += row.Length;
            }
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var currIndex = y * width * 3 + x * 3;
                    if (!int.TryParse(pixelArray?[currIndex], out var red))
                        throw new Exception($"Cannot parse red colour value: {pixelArray?[currIndex]} at column {y}, row {x}");
                    if (!int.TryParse(pixelArray?[currIndex + 1], out var green))
                        throw new Exception($"Cannot parse green colour value: {pixelArray?[currIndex + 1]} at column {y}, row {x}");
                    if (!int.TryParse(pixelArray?[currIndex + 2], out var blue))
                        throw new Exception($"Cannot parse blue colour value: {pixelArray?[currIndex + 2]} at column {y}, row {x}");
                    canvas.Pixels[y, x] = new Colour(red / colourMax, green / colourMax, blue / colourMax);
                }
            }
            return canvas;
        }

        private static string RemoveCommentLines(string fileContents)
        {
            if (fileContents.StartsWith("#"))
            {
                var firstReturn = fileContents.IndexOf("\n", StringComparison.Ordinal);
                fileContents = fileContents[(firstReturn + 1)..];
            }

            while (fileContents.Contains("\n#"))
            {
                var firstReturn = fileContents.IndexOf("\n#", StringComparison.Ordinal);
                var nextReturn = fileContents.IndexOf("\n", firstReturn + 1, StringComparison.Ordinal);
                fileContents = fileContents[..firstReturn] + fileContents[nextReturn..];
            }

            return fileContents;
        }
    }
}
