using Xunit;

namespace RayTracer.Tests
{
    public class TriangleTests
    {
        [Fact]
        public void ConstructingATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);

            // Act
            var t = new Triangle(p1, p2, p3);

            // Assert
            Assert.Equal(p1, t.Vertex1);
            Assert.Equal(p2, t.Vertex2);
            Assert.Equal(p3, t.Vertex3);
            Assert.Equal(new Vector(-1, -1, 0), t.Edge1);
            Assert.Equal(new Vector(1, -1, 0), t.Edge2);
            Assert.Equal(new Vector(0, 0, -1), t.Normal);
        }

        [Fact]
        public void NormalVectorForATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);

            // Act
            var norm1 = t.LocalNormalAt(new Point(0, 0.5, 0));
            var norm2 = t.LocalNormalAt(new Point(-0.5, 0.75, 0));
            var norm3 = t.LocalNormalAt(new Point(0.5, 0.25, 0));

            // Assert
            Assert.Equal(norm1, t.Normal);
            Assert.Equal(norm2, t.Normal);
            Assert.Equal(norm3, t.Normal);
        }

        [Fact]
        public void AParallelRayMissesATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            var ray = new Ray(new Point(0, -1, -2), new Vector(0, 1, 0));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayMissesTheVertex1Vertex3EdgeOfATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            var ray = new Ray(new Point(1, 1, -2), new Vector(0, 0, 1));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayMissesTheVertex1Vertex2EdgeOfATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            var ray = new Ray(new Point(-1, 1, -2), new Vector(0, 0, 1));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayMissesTheVertex2Vertex3EdgeOfATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            var ray = new Ray(new Point(0, -1, -2), new Vector(0, 0, 1));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayIntersectsATriangle()
        {
            // Arrange
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            var ray = new Ray(new Point(0, 0.5, -2), new Vector(0, 0, 1));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.Single(xs);
            Assert.Equal(2, xs[0].T);
        }
    }
}
