using Xunit;

namespace RayTracer.Tests
{
    public class StripePatternTests
    {
        [Fact]
        public void CreatingAStripePattern()
        {
            // Arrange
            var pattern = new StripePattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourA);
            Assert.Equal(Colour.BLACK, pattern.ColourB);
        }

        [Fact]
        public void AStripePatternIsConstantInY()
        {
            // Arrange
            var pattern = new StripePattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 1, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 2, 0)));
        }

        [Fact]
        public void AStripePatternIsConstantInZ()
        {
            // Arrange
            var pattern = new StripePattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 1)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 2)));
        }

        [Fact]
        public void AStripePatternAlternatesInX()
        {
            // Arrange
            var pattern = new StripePattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0.9, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(1, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(-0.1, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(-1, 0, 0)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(-1.1, 0, 0)));
        }

        [Fact]
        public void CloningAStripePattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new StripePattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
        }    }
}
