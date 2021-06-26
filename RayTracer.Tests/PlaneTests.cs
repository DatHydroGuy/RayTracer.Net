using Xunit;

namespace RayTracer.Tests
{
    public class PlaneTests
    {
        [Fact]
        public void TheNormalOfAPlaneIsConstantEverywhere()
        {
            // Arrange
            var p = new Plane();
            var expected = new Vector(0, 1, 0);

            // Act
            var result1 = p.LocalNormalAt(new Point(0, 0, 0));
            var result2 = p.LocalNormalAt(new Point(10, 0, -10));
            var result3 = p.LocalNormalAt(new Point(-5, 0, 150));

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
            Assert.Equal(expected, result3);
        }

        [Fact]
        public void IntersectARayParallelToThePlane()
        {
            // Arrange
            var p = new Plane();
            var ray = new Ray(new Point(0, 10, 0), new Vector(0, 0, 1));

            // Act
            var xs = p.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void IntersectWithACoplanarRay()
        {
            // Arrange
            var p = new Plane();
            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));

            // Act
            var xs = p.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayIntersectingAPlaneFromAbove()
        {
            // Arrange
            var p = new Plane();
            var ray = new Ray(new Point(0, 1, 0), new Vector(0, -1, 0));

            // Act
            var xs = p.LocalIntersects(ray);

            // Assert
            Assert.Single(xs);
            Assert.Equal(1, xs[0].T);
            Assert.Equal(p, xs[0].Obj);
        }

        [Fact]
        public void ARayIntersectingAPlaneFromBelow()
        {
            // Arrange
            var p = new Plane();
            var ray = new Ray(new Point(0, -1, 0), new Vector(0, 1, 0));

            // Act
            var xs = p.LocalIntersects(ray);

            // Assert
            Assert.Single(xs);
            Assert.Equal(1, xs[0].T);
            Assert.Equal(p, xs[0].Obj);
        }

        [Fact]
        public void APlaneHasABoundingBox()
        {
            // Arrange
            var s = new Plane();

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.True(double.IsNegativeInfinity(boundingBox.MinPoint.X));
            Assert.True(double.IsNegativeInfinity(boundingBox.MinPoint.Y));
            Assert.True(double.IsNegativeInfinity(boundingBox.MinPoint.Z));
            Assert.True(double.IsPositiveInfinity(boundingBox.MaxPoint.X));
            Assert.True(double.IsPositiveInfinity(boundingBox.MaxPoint.Y));
            Assert.True(double.IsPositiveInfinity(boundingBox.MaxPoint.Z));
        }

        [Fact]
        public void CloningAPlane()
        {
            // Arrange
            var orig = new Plane();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "[Type:RayTracer.Plane\nId:637602294772396341\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var orig = new Plane();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(Utilities.ToStringEquals(expected, result));
        }
    }
}
