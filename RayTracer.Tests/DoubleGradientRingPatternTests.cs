using System;
using Xunit;

namespace RayTracer.Tests
{
    public class DoubleGradientRingPatternTests
    {
        [Fact]
        public void ADoubleGradientRingPatternLinearlyInterpolatesBetweenColoursInXAndZInFirstHalf()
        {
            // Arrange
            var pattern = new DoubleGradientRingPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(new Colour(0.75, 0.75, 0.75), pattern.ColourAtPoint(new Point(0.125, 0, 0)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(0, 0, 0.25)));
            Assert.Equal(new Colour(0.25, 0.25, 0.25), pattern.ColourAtPoint(new Point(0, 0, -0.375)));
        }

        [Fact]
        public void ADoubleGradientRingPatternLinearlyInterpolatesBetweenColoursInXAndZInSecondHalf()
        {
            // Arrange
            var pattern = new DoubleGradientRingPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0.5, 0, 0)));
            Assert.Equal(new Colour(0.25, 0.25, 0.25), pattern.ColourAtPoint(new Point(-0.625, 0, 0)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(0, 0, -0.75)));
            Assert.Equal(new Colour(0.75, 0.75, 0.75), pattern.ColourAtPoint(new Point(0, 0, 0.875)));
        }

        [Fact]
        public void ADoubleGradientRingPatternRepeatsInBothXAndZ()
        {
            // Arrange
            var pattern = new DoubleGradientRingPattern(Colour.WHITE, Colour.BLACK);
            var val = 1.0 / (4.0 * Math.Sqrt(2.0));

            // Act

            // Assert
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, 0)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(val, 0, -val)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(0.5, 0, 0)));
            Assert.Equal(Colour.BLACK, pattern.ColourAtPoint(new Point(-2 * val, 0, 2 * val)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(-3 * val, 0, -3 * val)));
            Assert.Equal(Colour.WHITE, pattern.ColourAtPoint(new Point(0, 0, -0.999999)));
            Assert.Equal(new Colour(0.5, 0.5, 0.5), pattern.ColourAtPoint(new Point(5 * val, 0, 5 * val)));
        }

        [Fact]
        public void CloningADoubleGradientRingPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new DoubleGradientRingPattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
        }
    }
}
