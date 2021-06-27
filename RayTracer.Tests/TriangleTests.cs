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

        [Fact]
        public void CloningATriangle()
        {
            // Arrange
            var p1 = new Point(1, 2, 3);
            var p2 = new Point(2, 3, 4);
            var p3 = new Point(3, 4, 5);
            var orig = new Triangle(p1, p2, p3);

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
            Assert.Equal(orig.Edge1, clone.Edge1);
            Assert.Equal(orig.Edge2, clone.Edge2);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "[Type:RayTracer.Triangle\nId:637604035323461204\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nV1:[X:1, Y:2, Z:3, W:1]\nV2:[X:2, Y:3, Z:4, W:1]\nV3:[X:3, Y:4, Z:5, W:1]\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var p1 = new Point(1, 2, 3);
            var p2 = new Point(2, 3, 4);
            var p3 = new Point(3, 4, 5);
            var orig = new Triangle(p1, p2, p3);

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(Utilities.ToStringEquals(expected, result));
        }
    }
}
