using Xunit;

namespace RayTracer.Tests
{
    class TestPattern: Pattern
    {
        public TestPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            return new Colour(targetPoint.X, targetPoint.Y, targetPoint.Z);
        }

        public override Pattern Clone()
        {
            return new TestPattern(ColourA.Clone(), ColourB.Clone());
        }
    }

    public class PatternTests
    {
        [Fact]
        public void DefaultPatternTransform()
        {
            // Arrange
            var pattern = new TestPattern(Colour.WHITE, Colour.BLACK);

            // Act

            // Assert
            Assert.Equal(Matrix.Identity(4), pattern.Transform);
        }

        [Fact]
        public void AssigningATransform()
        {
            // Arrange
            var pattern = new TestPattern(Colour.WHITE, Colour.BLACK);
            var expected = Transformations.Translation(1, 2, 3);

            // Act
            pattern.Transform = expected;

            // Assert
            Assert.Equal(expected, pattern.Transform);
        }

        [Fact]
        public void StripesWithAnObjectTransformation()
        {
            // Arrange
            var obj = new Sphere();
            obj.Transform = Transformations.Scaling(2, 2, 2);
            var pattern = new TestPattern(Colour.WHITE, Colour.BLACK);
            var expected = new Colour(1, 1.5, 2);

            // Act
            var result = pattern.ColourAtShape(obj, new Point(2, 3, 4));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void StripesWithAPatternTransformation()
        {
            // Arrange
            var obj = new Sphere();
            var pattern = new TestPattern(Colour.WHITE, Colour.BLACK);
            pattern.Transform = Transformations.Scaling(2, 2, 2);
            var expected = new Colour(1, 1.5, 2);

            // Act
            var result = pattern.ColourAtShape(obj, new Point(2, 3, 4));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void StripesWithAnObjectAndAPatternTransformation()
        {
            // Arrange
            var obj = new Sphere();
            obj.Transform = Transformations.Scaling(2, 2, 2);
            var pattern = new TestPattern(Colour.WHITE, Colour.BLACK);
            pattern.Transform = Transformations.Translation(0.5, 1, 1.5);
            var expected = new Colour(0.75, 0.5, 0.25);

            // Act
            var result = pattern.ColourAtShape(obj, new Point(2.5, 3, 3.5));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CloningAPattern()
        {
            // Arrange
            var colourA = new Colour(0.1, 0.3, 0.5);
            var colourB = new Colour(0.2, 0.4, 0.6);
            var orig = new TestPattern(colourA, colourB);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.ColourA, result.ColourA);
            Assert.Equal(orig.ColourB, result.ColourB);
            Assert.Equal(orig, result);
        }
    }
}
