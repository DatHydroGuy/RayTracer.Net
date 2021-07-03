using RayTracer.Patterns;
using Xunit;

namespace RayTracer.Tests
{
    public class RingPatternTests
    {
        [Fact]
        public void ARingPatternVariesInBothXAndZ()
        {
            // Arrange
            var pattern = new RingPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(1, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0, 0, 1)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0.708, 0, 0.708)));
        }

        [Fact]
        public void CloningARingPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new RingPattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
        }
    }
}
