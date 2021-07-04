using System;
using RayTracer.Patterns;
using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class BlendedPatternTests
    {
        [Fact]
        public void BlendedStripePatternsAtRightAnglesAlternateInX()
        {
            // Arrange
            var ground = new Plane();
            var pattern1 = new StripePattern(Colour.WHITE, Colour.BLACK);
            var pattern2 = new StripePattern(Colour.WHITE, Colour.BLACK)
            {
                Transform = Transformations.RotationY(Math.PI / 2.0)
            };
            var groundPattern = new BlendedPattern(pattern1, pattern2);
            ground.Material = new Material
            {
                Pattern = groundPattern
            };
            var results1 = new Colour[11];
            var results2 = new Colour[11];
            var result1 = true;
            var result2 = true;
            var result3 = true;
            var result4 = true;

            // Act
            for (var i = -5; i < 6; i++)
            {
                results1[i + 5] = ground.Material.Pattern.ColourAtShape(ground, new Point(i + 0.5, 0, 0.5));
                results2[i + 5] = ground.Material.Pattern.ColourAtShape(ground, new Point(i + 0.5, 0, 1.5));
                if ((i + 5) % 2 == 0)
                {
                    result1 &= results1[i + 5] == Colour.BLACK;
                    result3 &= results2[i + 5] == new Colour(0.5, 0.5, 0.5);
                }
                else
                {
                    result2 &= results1[i + 5] == new Colour(0.5, 0.5, 0.5);
                    result4 &= results2[i + 5] == Colour.WHITE;
                }
            }

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.True(result4);
        }

        [Fact]
        public void BlendedStripePatternsAtRightAnglesAlternateInZ()
        {
            // Arrange
            var ground = new Plane();
            var pattern1 = new StripePattern(Colour.WHITE, Colour.BLACK);
            var pattern2 = new StripePattern(Colour.WHITE, Colour.BLACK)
            {
                Transform = Transformations.RotationY(Math.PI / 2.0)
            };
            var groundPattern = new BlendedPattern(pattern1, pattern2);
            ground.Material = new Material
            {
                Pattern = groundPattern
            };
            var results1 = new Colour[11];
            var results2 = new Colour[11];
            var result1 = true;
            var result2 = true;
            var result3 = true;
            var result4 = true;

            // Act
            for (var i = -5; i < 6; i++)
            {
                results1[i + 5] = ground.Material.Pattern.ColourAtShape(ground, new Point(0.5, 0, i + 0.5));
                results2[i + 5] = ground.Material.Pattern.ColourAtShape(ground, new Point(1.5, 0, i + 0.5));
                if ((i + 5) % 2 == 0)
                {
                    result1 &= results1[i + 5] == Colour.WHITE;
                    result3 &= results2[i + 5] == new Colour(0.5, 0.5, 0.5);
                }
                else
                {
                    result2 &= results1[i + 5] == new Colour(0.5, 0.5, 0.5);
                    result4 &= results2[i + 5] == Colour.BLACK;
                }
            }

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.True(result4);
        }

        [Fact]
        public void CloningABlendedPattern()
        {
            // Arrange
            var colourAa = new Colour(0.1, 0.3, 0.5);
            var colourAb = new Colour(0.2, 0.4, 0.6);
            var colourBa = new Colour(0.1, 0.4, 0.7);
            var colourBb = new Colour(0.2, 0.5, 0.8);
            var patternA = new TestPattern(colourAa, colourAb);
            var patternB = new TestPattern(colourBa, colourBb);
            var orig = new BlendedPattern(patternA, patternB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.PatternA, result.PatternA);
            Assert.Equal(orig.PatternB, result.PatternB);
        }
    }
}
