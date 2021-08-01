using System;
using System.IO;
using Xunit;

namespace RayTracer.Tests
{
    public class CanvasTests
    {
        [Fact]
        public void CreatingACanvas()
        {
            // Arrange
            var c = new Canvas(10, 20);

            // Act

            // Assert
            Assert.Equal(10, c.Width);
            Assert.Equal(20, c.Height);
            var allBlack = true;
            for (var y = 0; y < c.Height; y++)
            {
                for (var x = 0; x < c.Width; x++)
                {
                    allBlack &= c.Pixels[y, x] == new Colour(0, 0, 0);
                }
            }
            Assert.True(allBlack);
        }

        [Fact]
        public void WritingPixelsToACanvas()
        {
            // Arrange
            var c = new Canvas(10, 20);
            var red = new Colour(1, 0, 0);
            var expected = new Colour(1, 0, 0);

            // Act
            c.WritePixel(2, 14, red);

            // Assert
            Assert.Equal(expected, c.PixelAt(2, 14));
        }

        [Fact]
        public void AttemptingToWriteToAnOutOfBoundsPixelDoesNotThrowAnException()
        {
            // Arrange
            var c = new Canvas(10, 20);

            // Act
            c.WritePixel(-1, 4, new Colour());

            // Assert - Will not get to the following (always true) assertion if an error is thrown above
            Assert.True(true);
        }

        [Fact]
        public void AttemptingToGetAnOutOfBoundsPixelReturnsNull()
        {
            // Arrange
            var c = new Canvas(10, 20);

            // Act
            var result = c.PixelAt(-1, 4);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ReadingAPpmFileWithTheWrongMagicNumber()
        {
            // Arrange
            const string fileContents = "P32\r\n1 1\r\n255\r\n0 0 0\r\n";

            // Act & Assert
            Assert.Throws<Exception>(() => Canvas.CanvasFromPpm(fileContents));
        }

        [Fact]
        public void ReadingAPpmFileReturnsACanvasOfTheCorrectDimensions()
        {
            // Arrange
            const string fileContents = "P3\r\n10 2\r\n255\r\n0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\r\n0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\r\n0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\r\n0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\r\n";

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            
            // Assert
            Assert.Equal(10, c.Width);
            Assert.Equal(2, c.Height);
        }

        [Fact]
        public void ReadingPixelDataFromAPpmFile()
        {
            // Arrange
            const string fileContents = "P3\r\n4 3\r\n255\r\n255 127 0 0 127 255 127 255 0 255 255 255\r\n0 0 0 255 0 0 0 255 0 0 0 255\r\n255 255 0 0 255 255 255 0 255 127 127 127\r\n";
            var allTrue = true;
            var expected = new[]
            {
                new[] { new Colour(1, 0.49804, 0), new Colour(0, 0.49804, 1), new Colour(0.49804, 1, 0), new Colour(1, 1, 1) },
                new[] { new Colour(0, 0, 0), new Colour(1, 0, 0), new Colour(0, 1, 0), new Colour(0, 0, 1) },
                new[] { new Colour(1, 1, 0), new Colour(0, 1, 1), new Colour(1, 0, 1), new Colour(0.49804, 0.49804, 0.49804) }
            };

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            for (var y = 0; y < c.Height; y++)
            {
                for (var x = 0; x < c.Width; x++)
                {
                    allTrue &= c.Pixels[y, x] == expected[y][x];
                }
            }
            
            // Assert
            Assert.True(allTrue);
        }

        [Fact]
        public void PpmParsingIgnoresCommentLines()
        {
            // Arrange
            const string fileContents = "P3\r\n# this is a comment\r\n2 1\r\n# this is, too\r\n255\r\n# another comment\r\n255 255 255\r\n# oh no! Comments in the pixel data!\r\n255 0 255\r\n";

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            
            // Assert
            Assert.Equal(new Colour(1, 1, 1), c.Pixels[0, 0]);
            Assert.Equal(new Colour(1, 0, 1), c.Pixels[0, 1]);
        }

        [Fact]
        public void PpmParsingAllowsRgbTriplesToSpanMultipleLines()
        {
            // Arrange
            const string fileContents = "P3\r\n2 1\r\n255\r\n51\r\n153\r\n\r\n204\r\n255 127\r\n0\r\n";

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            
            // Assert
            Assert.Equal(new Colour(0.2, 0.6, 0.8), c.Pixels[0, 0]);
            Assert.Equal(new Colour(1, 0.49804, 0), c.Pixels[0, 1]);
        }

        [Fact]
        public void PpmParsingAllowsRgbTriplesToHaveDifferentLayoutToCanvasPixels()
        {
            // Arrange
            const string fileContents = "P3\r\n3 2\r\n255\r\n51 153 204  255 127 0  51 153 204  255\r\n127 0  51 153 204  255 127 0\r\n";

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            
            // Assert
            Assert.Equal(new Colour(0.2, 0.6, 0.8), c.Pixels[0, 0]);
            Assert.Equal(new Colour(1, 0.49804, 0), c.Pixels[0, 1]);
            Assert.Equal(new Colour(0.2, 0.6, 0.8), c.Pixels[0, 2]);
            Assert.Equal(new Colour(1, 0.49804, 0), c.Pixels[1, 0]);
            Assert.Equal(new Colour(0.2, 0.6, 0.8), c.Pixels[1, 1]);
            Assert.Equal(new Colour(1, 0.49804, 0), c.Pixels[1, 2]);
        }

        [Fact]
        public void PpmParsingRespectsTheScaleSetting()
        {
            // Arrange
            const string fileContents = "P3\r\n2 2\r\n100\r\n100 100 100  50 50 50\r\n75 50 25  0 0 0\r\n";

            // Act
            var c = Canvas.CanvasFromPpm(fileContents);
            
            // Assert
            Assert.Equal(new Colour(1, 1, 1), c.Pixels[0, 0]);
            Assert.Equal(new Colour(0.5, 0.5, 0.5), c.Pixels[0, 1]);
            Assert.Equal(new Colour(0.75, 0.5, 0.25), c.Pixels[1, 0]);
            Assert.Equal(new Colour(0, 0, 0), c.Pixels[1, 1]);
        }
    }
}
