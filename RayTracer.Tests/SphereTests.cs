using System;
using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class SphereTests
    {
        [Fact]
        public void ARayIntersectsASphereAtTwoPoints()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.True(Utilities.AlmostEqual(4.0, xs[0].T));
            Assert.True(Utilities.AlmostEqual(6.0, xs[1].T));
        }

        [Fact]
        public void ARayIntersectsASphereAtATangent()
        {
            // Arrange
            var r = new Ray(new Point(0, 1, -5), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.True(Utilities.AlmostEqual(5.0, xs[0].T));
            Assert.True(Utilities.AlmostEqual(5.0, xs[1].T));
        }

        [Fact]
        public void ARayMissesASphere()
        {
            // Arrange
            var r = new Ray(new Point(0, 2, -5), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayOriginatesInsideASphere()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.True(Utilities.AlmostEqual(-1.0, xs[0].T));
            Assert.True(Utilities.AlmostEqual(1.0, xs[1].T));
        }

        [Fact]
        public void ASphereIsBehindARay()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, 5), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.True(Utilities.AlmostEqual(-6.0, xs[0].T));
            Assert.True(Utilities.AlmostEqual(-4.0, xs[1].T));
        }

        [Fact]
        public void IntersectSetsTheObjectOnTheIntersection()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();

            // Act
            var xs = s.LocalIntersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.Equal(s, xs[0].Obj);
            Assert.Equal(s, xs[1].Obj);
        }

        [Fact]
        public void ASphereHasADefaultTransformation()
        {
            // Arrange
            var s = new Sphere();
            var expected = Matrix.Identity(4);

            // Act

            // Assert
            Assert.Equal(expected, s.Transform);
        }

        [Fact]
        public void ChangingASpheresTransformation()
        {
            // Arrange
            var s = new Sphere();
            var t = Transformations.Translation(2, 3, 4);

            // Act
            s.Transform = t;

            // Assert
            Assert.Equal(t, s.Transform);
        }

        [Fact]
        public void TheNormalOnASphereAtAPointOnTheXAxis()
        {
            // Arrange
            var s = new Sphere();
            var expected = new Vector(1, 0, 0);

            // Act
            var result = s.LocalNormalAt(new Point(1, 0, 0));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheNormalOnASphereAtAPointOnTheYAxis()
        {
            // Arrange
            var s = new Sphere();
            var expected = new Vector(0, 1, 0);

            // Act
            var result = s.LocalNormalAt(new Point(0, 1, 0));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheNormalOnASphereAtAPointOnTheZAxis()
        {
            // Arrange
            var s = new Sphere();
            var expected = new Vector(0, 0, 1);

            // Act
            var result = s.LocalNormalAt(new Point(0, 0, 1));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheNormalOnASphereAtANonaxialPoint()
        {
            // Arrange
            var thirdSqrtThree = Math.Sqrt(3) / 3.0;
            var s = new Sphere();
            var expected = new Vector(thirdSqrtThree, thirdSqrtThree, thirdSqrtThree);

            // Act
            var result = s.LocalNormalAt(new Point(thirdSqrtThree, thirdSqrtThree, thirdSqrtThree));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheNormalIsANormalisedVector()
        {
            // Arrange
            var thirdSqrtThree = Math.Sqrt(3) / 3.0;
            var s = new Sphere();

            // Act
            var result = s.LocalNormalAt(new Point(thirdSqrtThree, thirdSqrtThree, thirdSqrtThree));

            // Assert
            Assert.Equal(result, result.Normalise());
        }

        [Fact]
        public void ASphereIsAShape()
        {
            // Arrange
            var s = new Sphere();

            // Act

            // Assert
            Assert.IsAssignableFrom<Shape>(s);
        }

        [Fact]
        public void ASphereHasABoundingBox()
        {
            // Arrange
            var s = new Sphere();

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-1, -1, -1), boundingBox.MinPoint);
            Assert.Equal(new Point(1, 1, 1), boundingBox.MaxPoint);
        }

        [Fact]
        public void CloningASphere()
        {
            // Arrange
            var orig = new Sphere();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
            Assert.Equal(orig.Radius, clone.Radius);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "[Type:RayTracer.Shapes.Sphere\nId:637602294772396341\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nRadius:1\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var orig = new Sphere();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
