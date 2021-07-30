using System;
using Xunit;

namespace RayTracer.Tests
{
    public class ImagePatternTests
    {
        [Fact]
        public void CheckerPatternIn2D()
        {
            // Arrange
            const string fileContents = "P3\r\n10 10\r\n10\r\n" +
                                        "0 0 0  1 1 1  2 2 2  3 3 3  4 4 4  5 5 5  6 6 6  7 7 7  8 8 8  9 9 9\r\n" +
                                        "1 1 1  2 2 2  3 3 3  4 4 4  5 5 5  6 6 6  7 7 7  8 8 8  9 9 9  0 0 0\r\n" +
                                        "2 2 2  3 3 3  4 4 4  5 5 5  6 6 6  7 7 7  8 8 8  9 9 9  0 0 0  1 1 1\r\n" +
                                        "3 3 3  4 4 4  5 5 5  6 6 6  7 7 7  8 8 8  9 9 9  0 0 0  1 1 1  2 2 2\r\n" +
                                        "4 4 4  5 5 5  6 6 6  7 7 7  8 8 8  9 9 9  0 0 0  1 1 1  2 2 2  3 3 3\r\n" +
                                        "5 5 5  6 6 6  7 7 7  8 8 8  9 9 9  0 0 0  1 1 1  2 2 2  3 3 3  4 4 4\r\n" +
                                        "6 6 6  7 7 7  8 8 8  9 9 9  0 0 0  1 1 1  2 2 2  3 3 3  4 4 4  5 5 5\r\n" +
                                        "7 7 7  8 8 8  9 9 9  0 0 0  1 1 1  2 2 2  3 3 3  4 4 4  5 5 5  6 6 6\r\n" +
                                        "8 8 8  9 9 9  0 0 0  1 1 1  2 2 2  3 3 3  4 4 4  5 5 5  6 6 6  7 7 7\r\n" +
                                        "9 9 9  0 0 0  1 1 1  2 2 2  3 3 3  4 4 4  5 5 5  6 6 6  7 7 7  8 8 8\r\n";
            var canvas = Canvas.CanvasFromPpm(fileContents);
            var uValues = new[] { 0, 0.3, 0.6, 1 };
            var vValues = new[] { 0, 0, 0.3, 1 };
            var expected = new[] { new Colour(0.9, 0.9, 0.9), new Colour(0.2, 0.2, 0.2), new Colour(0.1, 0.1, 0.1), new Colour(0.9, 0.9, 0.9) };
            var allTestsPass = true;

            // Act
            var pattern = new ImagePattern(canvas);
            for (var i = 0; i < expected.Length; i++)
            {
                allTestsPass &= pattern.UvPatternAt(uValues[i], vValues[i]) == expected[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }
    }
}
