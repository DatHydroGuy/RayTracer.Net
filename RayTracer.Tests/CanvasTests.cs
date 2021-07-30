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
            var c = new Canvas(1, 1);
            const string fileContents = "P32\r\n1 1\r\n255\r\n0 0 0\r\n";

            // Act & Assert
            Assert.Throws<Exception>(() => c.CanvasFromPpm(fileContents));
        }
    }
}
