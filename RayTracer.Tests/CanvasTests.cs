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
        public void ConstructingThePpmHeader()
        {
            // Arrange
            var c = new Canvas(5, 3);
            var expected = new string[] {"P3", "5 3", "255"};
            var result = new string[3];

            // Act
            var ppmContent = c.WriteToPpm();
            using (var sr = new StringReader(ppmContent))
            {
                result[0] = sr.ReadLine();
                result[1] = sr.ReadLine();
                result[2] = sr.ReadLine();
            }

            // Assert
            Assert.Equal(expected[0], result[0]);
            Assert.Equal(expected[1], result[1]);
            Assert.Equal(expected[2], result[2]);
        }

        [Fact]
        public void ConstructingThePpmPixelData()
        {
            // Arrange
            var c = new Canvas(5, 3);
            const string expected = "255 0 0 0 0 0 0 0 0 0 0 0 0 0 0\r\n0 0 0 0 0 0 0 128 0 0 0 0 0 0 0\r\n0 0 0 0 0 0 0 0 0 0 0 0 0 0 255\r\n";
            var c1 = new Colour(1.5, 0, 0);
            var c2 = new Colour(0, 0.5, 0);
            var c3 = new Colour(-0.5, 0, 1);
            string result;

            // Act
            c.WritePixel(0, 0, c1);
            c.WritePixel(2, 1, c2);
            c.WritePixel(4, 2, c3);
            var ppmContent = c.WriteToPpm();
            using (var sr = new StringReader(ppmContent))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                result = sr.ReadToEnd();
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SplittingLongLinesInPpmFiles()
        {
            // Arrange
            var c = new Canvas(10, 2);
            const string expected = "255 204 153 255 204 153 255 204 153 255 204 153 255 204 153 255 204\r\n153 255 204 153 255 204 153 255 204 153 255 204 153\r\n255 204 153 255 204 153 255 204 153 255 204 153 255 204 153 255 204\r\n153 255 204 153 255 204 153 255 204 153 255 204 153\r\n";
            var c1 = new Colour(1, 0.8, 0.6);
            string result;

            // Act
            c.SetColour(c1);
            var ppmContent = c.WriteToPpm();
            using (var sr = new StringReader(ppmContent))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                result = sr.ReadToEnd();
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void PpmFilesEndWithANewlineCharacter()
        {
            // Arrange
            var c = new Canvas(5, 3);
            const string expected = "\r\n";
            string result;

            // Act
            var ppmContent = c.WriteToPpm();
            using (var sr = new StringReader(ppmContent))
            {
                result = sr.ReadToEnd();
            }

            // Assert
            Assert.EndsWith(expected, result);
        }
    }
}
