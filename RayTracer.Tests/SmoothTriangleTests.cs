using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class SmoothTriangleTests
    {
        private readonly Point _p1;
        private readonly Point _p2;
        private readonly Point _p3;
        private readonly Vector _n1;
        private readonly Vector _n2;
        private readonly Vector _n3;

        private readonly SmoothTriangle _t;

        public SmoothTriangleTests()
        {
            // Arrange
            _p1 = new Point(0, 1, 0);
            _p2 = new Point(-1, 0, 0);
            _p3 = new Point(1, 0, 0);
            _n1 = new Vector(0, 1, 0);
            _n2 = new Vector(-1, 0, 0);
            _n3 = new Vector(1, 0, 0);

            // Act
            _t = new SmoothTriangle(_p1, _p2, _p3, _n1, _n2, _n3);
        }

        [Fact]
        public void ConstructingASmoothTriangle()
        {
            // Assert
            Assert.Equal(_p1, _t.Vertex1);
            Assert.Equal(_p2, _t.Vertex2);
            Assert.Equal(_p3, _t.Vertex3);
            Assert.Equal(_n1, _t.Normal1);
            Assert.Equal(_n2, _t.Normal2);
            Assert.Equal(_n3, _t.Normal3);
        }

        [Fact]
        public void AnIntersectionWithASmoothTriangleStoresUAndVCoordinates()
        {
            // Arrange
            var ray = new Ray(new Point(-0.2, 0.3, -2), new Vector(0, 0, 1));

            // Act
            var xs = _t.LocalIntersects(ray);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.45, xs[0].U));
            Assert.True(Utilities.AlmostEqual(0.25, xs[0].V));
        }

        [Fact]
        public void ASmoothTriangleUsesUAndVCoordinatesToInterpolateTheNormal()
        {
            // Arrange
            var intersect = Intersection.IntersectionWithUv(1, _t, 0.45, 0.25);
            var expected = new Vector(-0.5547, 0.83205, 0);

            // Act
            var norm = _t.NormalAt(new Point(0, 0, 0), intersect);

            // Assert
            Assert.Equal(expected, norm);
        }

        [Fact]
        public void PreparingTheNormalOnASmoothTriangle()
        {
            // Arrange
            var intersect = Intersection.IntersectionWithUv(1, _t, 0.45, 0.25);
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

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "[Type:RayTracer.Shapes.SmoothTriangle\nId:637604035323461204\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nV1:[X:1, Y:2, Z:3, W:1]\nV2:[X:2, Y:3, Z:4, W:1]\nV3:[X:3, Y:4, Z:5, W:1]\nN1:[X:1, Y:2, Z:3, W:0]\nN2:[X:2, Y:3, Z:4, W:0]\nN3:[X:3, Y:4, Z:5, W:0]\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var p1 = new Point(1, 2, 3);
            var p2 = new Point(2, 3, 4);
            var p3 = new Point(3, 4, 5);
            var n1 = new Vector(1, 2, 3);
            var n2 = new Vector(2, 3, 4);
            var n3 = new Vector(3, 4, 5);
            var orig = new SmoothTriangle(p1, p2, p3, n1, n2, n3);

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
