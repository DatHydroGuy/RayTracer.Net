using System;
using System.IO;
using System.Text;

namespace RayTracer
{
    public class PpmFileWriter : ImageFileWriter
    {
        public PpmFileWriter(Colour[,] pixels) : base(pixels)
        {}

        public override void WriteImageFile(string ppmFile)
        {
            var filename = ppmFile.EndsWith(".ppm") ? ppmFile : $"{ppmFile}.ppm";
            var ppm = WriteToPpm();
            // Write the string array to a new file with the given name.
            using var outputFile = new StreamWriter(filename);
            foreach (var c in ppm)
                outputFile.Write(c);
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
                ppmContent.AppendLine(currLine[..^1]);  // All elements except the last one
            }

            return ppmContent.ToString();
        }

        private static string ConvertColourToPpmFormat(Colour col)
        {
            var red = Math.Min(Math.Max((int)Math.Round(col.Red * 255, MidpointRounding.AwayFromZero), 0), 255);
            var green = Math.Min(Math.Max((int)Math.Round(col.Green * 255, MidpointRounding.AwayFromZero), 0), 255);
            var blue = Math.Min(Math.Max((int)Math.Round(col.Blue * 255, MidpointRounding.AwayFromZero), 0), 255);
            return $"{red} {green} {blue} ";
        }
    }
}
