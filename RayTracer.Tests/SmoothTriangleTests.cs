using Xunit;

namespace RayTracer.Tests
{
    public class SmoothTriangleTests
    {
        Point p1;
        Point p2;
        Point p3;
        Vector n1;
        Vector n2;
        Vector n3;

        SmoothTriangle t;

        public SmoothTriangleTests()
        {
            // Arrange
            p1 = new Point(0, 1, 0);
            p2 = new Point(-1, 0, 0);
            p3 = new Point(1, 0, 0);
            n1 = new Vector(0, 1, 0);
            n2 = new Vector(-1, 0, 0);
            n3 = new Vector(1, 0, 0);

            // Act
            t = new SmoothTriangle(p1, p2, p3, n1, n2, n3);
        }

        [Fact]
        public void ConstructingASmoothTriangle()
        {
            // Assert
            Assert.Equal(p1, t.Vertex1);
            Assert.Equal(p2, t.Vertex2);
            Assert.Equal(p3, t.Vertex3);
            Assert.Equal(n1, t.Normal1);
            Assert.Equal(n2, t.Normal2);
            Assert.Equal(n3, t.Normal3);
        }

        [Fact]
        public void AnIntersectionWithASmoothTriangleStoresUAndVCoordinates()
        {
            // Arrange
            var ray = new Ray(new Point(-0.2, 0.3, -2), new Vector(0, 0, 1));

            // Act
            var xs = t.LocalIntersects(ray);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.45, xs[0].U));
            Assert.True(Utilities.AlmostEqual(0.25, xs[0].V));
        }

        [Fact]
        public void ASmoothTriangleUsesUAndVCoordinatesToInterpolateTheNormal()
        {
            // Arrange
            var intersect = Intersection.IntersectionWithUV(1, t, 0.45, 0.25);
            var expected = new Vector(-0.5547, 0.83205, 0);

            // Act
            var norm = t.NormalAt(new Point(0, 0, 0), intersect);

            // Assert
            Assert.Equal(expected, norm);
        }

        [Fact]
        public void PreparingTheNormalOnASmoothTriangle()
        {
            // Arrange
            var intersect = Intersection.IntersectionWithUV(1, t, 0.45, 0.25);
            var ray = new Ray(new Point(-0.2, 0.3, -2), new Vector(0, 0, 1));
            var xs = Intersection.Intersections(intersect);
            var comps = intersect.PrepareComputations(ray, xs);
            var expected = new Vector(-0.5547, 0.83205, 0);

            // Act
            var result = comps.NormalVector;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CloningASmoothTriangle()
        {
            // Arrange
            var p1 = new Point(1, 2, 3);
            var p2 = new Point(2, 3, 4);
            var p3 = new Point(3, 4, 5);
            var n1 = new Vector(1, 2, 3);
            var n2 = new Vector(2, 3, 4);
            var n3 = new Vector(3, 4, 5);
            var orig = new SmoothTriangle(p1, p2, p3, n1, n2, n3);

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
            Assert.Equal(orig.Vertex1, clone.Vertex1);
            Assert.Equal(orig.Vertex2, clone.Vertex2);
            Assert.Equal(orig.Vertex3, clone.Vertex3);
            Assert.Equal(orig.Normal1, clone.Normal1);
            Assert.Equal(orig.Normal2, clone.Normal2);
            Assert.Equal(orig.Normal3, clone.Normal3);
            Assert.Equal(orig.Edge1, clone.Edge1);
            Assert.Equal(orig.Edge2, clone.Edge2);
        }
    }
}
