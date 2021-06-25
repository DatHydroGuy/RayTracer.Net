using Xunit;

namespace RayTracer.Tests
{
    public class GradientPatternTests
    {
        [Fact]
        public void AGradientPatternLinearlyInterpolatesBetweenColours()
        {
            // Arrange
            var pattern = new GradientPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(new Colour(0.75, 0.75, 0.75), pattern.ColourAtPoint(new Point(0.25, 0, 0)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(0.5, 0, 0)));
            Assert.Equal(new Colour(0.25, 0.25, 0.25), pattern.ColourAtPoint(new Point(0.75, 0, 0)));
        }

        [Fact]
        public void CloningAGradientPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new GradientPattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
        }
    }
}
