using Xunit;

namespace RayTracer.Tests
{
    public class RayTests
    {
        [Fact]
        public void CreatingAndQueryingARay()
        {
            // Arrange
            var origin = new Point(1, 2, 3);
            var direction = new Vector(4, 5, 6);

            // Act
            var ray = new Ray(origin, direction);

            // Assert
            Assert.Equal(origin, ray.Origin);
            Assert.Equal(direction, ray.Direction);
        }

        [Fact]
        public void ComputingAPointFromADistance()
        {
            // Arrange
            var origin = new Point(2, 3, 4);
            var direction = new Vector(1, 0, 0);
            var ray = new Ray(origin, direction);
            var expected0 = new Point(2, 3, 4);
            var expected1 = new Point(3, 3, 4);
            var expectedMinus1 = new Point(1, 3, 4);
            var expectedTwoPtFive = new Point(4.5, 3, 4);

            // Act

            // Assert
            Assert.Equal(expected0, ray.Position(0));
            Assert.Equal(expected1, ray.Position(1));
            Assert.Equal(expectedMinus1, ray.Position(-1));
            Assert.Equal(expectedTwoPtFive, ray.Position(2.5));
        }

        [Fact]
        public void TranslatingARay()
        {
            // Arrange
            var origin = new Point(1, 2, 3);
            var direction = new Vector(0, 1, 0);
            var ray = new Ray(origin, direction);
            var m = Transformations.Translation(3, 4, 5);
            var expectedOrigin = new Point(4, 6, 8);

            // Act
            var r2 = ray.Transform(m);

            // Assert
            Assert.Equal(expectedOrigin, r2.Origin);
            Assert.Equal(direction, r2.Direction);
        }

        [Fact]
        public void ScalingARay()
        {
            // Arrange
            var origin = new Point(1, 2, 3);
            var direction = new Vector(0, 1, 0);
            var ray = new Ray(origin, direction);
            var m = Transformations.Scaling(2, 3, 4);
            var expectedOrigin = new Point(2, 6, 12);
            var expectedDirection = new Vector(0, 3, 0);

            // Act
            var r2 = ray.Transform(m);

            // Assert
            Assert.Equal(expectedOrigin, r2.Origin);
            Assert.Equal(expectedDirection, r2.Direction);
        }
    }
}
