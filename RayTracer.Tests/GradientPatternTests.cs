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
    }
}
