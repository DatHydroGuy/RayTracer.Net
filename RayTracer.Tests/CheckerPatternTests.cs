using Xunit;

namespace RayTracer.Tests
{
    public class CheckerPatternTests
    {

        [Fact]
        public void ACheckerPatternAlternatesInX()
        {
            // Arrange
            var pattern = new CheckerPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0.99, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(1.01, 0, 0)));
        }
        
        [Fact]
        public void ACheckerPatternAlternatesInY()
        {
            // Arrange
            var pattern = new CheckerPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0.99, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0, 1.01, 0)));
        }

        [Fact]
        public void ACheckerPatternAlternatesInZ()
        {
            // Arrange
            var pattern = new CheckerPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0.99)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0, 0, 1.01)));
        }
    }
}
