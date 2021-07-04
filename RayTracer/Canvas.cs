using System;
using System.IO;
using System.Text;

namespace RayTracer
{
    public class Canvas
    {
        public int Width { get; }

        public int Height { get; }

        public Colour[,] Pixels { get; }

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

        public string WriteToPpm()
        {
            var ppmContent = new StringBuilder();
            ppmContent.AppendLine("P3");
            ppmContent.AppendLine($"{Width} {Height}");
            ppmContent.AppendLine("255");

            for (var y = 0; y < Height; y++)
            {
                var currLine = "";
                for (var x = 0; x < Width; x++)
                {
                    var currCol = Pixels[y, x];
                    currLine += ConvertColourToPpmFormat(currCol);
                    if (currLine.Length <= 70)
                        continue;
                    var breakAt = currLine.LastIndexOf(" ", currLine.Length - 2, 11, StringComparison.Ordinal);
                    var ppmLine = currLine[..breakAt];
                    ppmContent.AppendLine(ppmLine);
                    currLine = currLine[(breakAt + 1)..];
                }
                ppmContent.AppendLine(currLine[..^1]);  // currLine[0..length-1] search for "hat operator"
            }

            return ppmContent.ToString();
        }

        public void WritePpmFile(string ppmFile)
        {
            var filename = ppmFile.EndsWith(".ppm") ? ppmFile : $"{ppmFile}.ppm";
            var ppm = WriteToPpm();
            // Write the string array to a new file with the given name.
            using var outputFile = new StreamWriter(filename);
            foreach (var c in ppm)
                outputFile.Write(c);
        }

        private static string ConvertColourToPpmFormat(Colour col)
        {
            var red = Math.Min(Math.Max((int)(Math.Round(col.Red * 255, MidpointRounding.AwayFromZero)), 0), 255);
            var green = Math.Min(Math.Max((int)(Math.Round(col.Green * 255, MidpointRounding.AwayFromZero)), 0), 255);
            var blue = Math.Min(Math.Max((int)(Math.Round(col.Blue * 255, MidpointRounding.AwayFromZero)), 0), 255);
            return $"{red} {green} {blue} ";
        }
    }
}
