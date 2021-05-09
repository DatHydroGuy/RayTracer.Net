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
    }
}
