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

        [Fact]
        public void CloningACheckerPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new CheckerPattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
        }
    }
}
