using System;
using System.IO;
using System.Text;

namespace RayTracer
{
    public class Canvas
    {
        private int _width;
        private int _height;
        private Colour[,] _pixels;
        
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public Colour[,] Pixels
        {
            get { return _pixels; }
            set { _pixels = value; }
        }

        public Canvas(int width = 100, int height = 80)
        {
            Width = width;
            Height = height;
            Pixels = new Colour[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Pixels[y, x] = new Colour();
                }
            }
        }

        public void SetColour(Colour col)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
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

            for (int y = 0; y < Height; y++)
            {
                var currLine = "";
                for (int x = 0; x < Width; x++)
                {
                    var currCol = Pixels[y, x];
                    currLine += ConvertColourToPpmFormat(currCol);
                    if (currLine.Length > 70)
                    {
                        var breakAt = currLine.LastIndexOf(" ", currLine.Length - 2, 11);
                        var ppmLine = currLine.Substring(0, breakAt);
                        ppmContent.AppendLine(ppmLine);
                        currLine = currLine.Substring(breakAt + 1);
                    }
                }
                ppmContent.AppendLine(currLine.Substring(0, currLine.Length - 1));
            }

            return ppmContent.ToString();
        }

        public void WritePpmFile(string ppmFile)
        {
            var filename = ppmFile.EndsWith(".ppm") ? ppmFile : $"{ppmFile}.ppm";
            var ppm = WriteToPpm();
            // Write the string array to a new file with the given name.
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                foreach (char c in ppm)
                    outputFile.Write(c);
            }
        }

        private string ConvertColourToPpmFormat(Colour col)
        {
            var red = Math.Min(Math.Max((int)(Math.Round(col.Red * 255, MidpointRounding.AwayFromZero)), 0), 255);
            var green = Math.Min(Math.Max((int)(Math.Round(col.Green * 255, MidpointRounding.AwayFromZero)), 0), 255);
            var blue = Math.Min(Math.Max((int)(Math.Round(col.Blue * 255, MidpointRounding.AwayFromZero)), 0), 255);
            return $"{red} {green} {blue} ";
        }
    }
}
