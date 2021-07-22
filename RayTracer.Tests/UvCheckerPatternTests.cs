using Xunit;

namespace RayTracer.Tests
{
    public class UvCheckerPatternTests
    {
        [Fact]
        public void ACheckerPatternIn2D()
        {
            // Arrange
            var uvCheckers = new UvCheckerPattern(2, 2, Colour.BLACK, Colour.WHITE);
            var uValues = new [] {0.0, 0.5, 0.0, 0.5, 1.0};
            var vValues = new [] {0.0, 0.0, 0.5, 0.5, 1.0};
            var expecteds = new [] {Colour.BLACK, Colour.WHITE, Colour.WHITE, Colour.BLACK, Colour.BLACK};
            var allTestsPass = true;

            for (var i = 0; i < expecteds.Length; i++)
            {
                // Act
                var result = uvCheckers.ColourAtUv(uValues[i], vValues[i]);
                allTestsPass &= result == expecteds[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }
        
        [Fact]
        public void UsingATextureMapWithASphericalMap()
        {
            // Arrange
            var uvCheckers = new UvCheckerPattern(16, 8, Colour.BLACK, Colour.WHITE, UvPatternMapType.Spherical);
            var colours = new [] {Colour.WHITE, Colour.BLACK, Colour.WHITE, Colour.BLACK, Colour.BLACK, Colour.BLACK, Colour.BLACK, Colour.WHITE, Colour.BLACK, Colour.BLACK};
            var points = new [] {new Point(0.4315,0.4670,0.7719), new Point(-0.9654,0.2552,-0.0534),
                new Point(0.1039,0.7090,0.6975), new Point(-0.4986,-0.7856,-0.3663),
                new Point(-0.0317,-0.9395,0.3411), new Point(0.4809,-0.7721,0.4154),
                new Point(0.0285,-0.9612,-0.2745), new Point(-0.5734,-0.2162,-0.7903),
                new Point(0.7688,-0.1470,0.6223), new Point(-0.7652,0.2175,0.6060)
            };
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var result = uvCheckers.ColourAtPoint(points[i]);
                allTestsPass &= result == colours[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void CloningAUvCheckerPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new UvCheckerPattern(3, 5, colourA, colourB, UvPatternMapType.Cylindrical);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.Width, result.Width);
            Assert.Equal(orig.Height, result.Height);
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
            Assert.Equal(orig.UvPatternMapType, result.UvPatternMapType);
        }
    }
}
