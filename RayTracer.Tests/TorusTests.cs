// using System;
// using Xunit;

// namespace RayTracer.Tests
// {
//     public class TorusTests
//     {
//         [Fact]
//         public void ARayIntersectsATorusAtFourPoints()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.True(Utilities.AlmostEqual(3.0, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(4.0, xs[1].T));
//             Assert.True(Utilities.AlmostEqual(6.0, xs[2].T));
//             Assert.True(Utilities.AlmostEqual(7.0, xs[3].T));
//         }

//         [Fact]
//         public void ARayIntersectsATorusAtATangent()
//         {
//             // Arrange
//             var r = new Ray(new Point(2, 0, -5), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(2, xs.Length);
//             Assert.True(Utilities.AlmostEqual(5.0, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(5.0, xs[1].T));
//         }

//         [Fact]
//         public void ARayIntersectsATorusAtATangentToTheInnerRing()
//         {
//             // Arrange
//             var r = new Ray(new Point(1, 0, -5), new Vector(0, 0, 1));
//             var s = new Torus();
//             var root3 = Math.Sqrt(3.0);

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.True(Utilities.AlmostEqual(5.0 - root3, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(5.0, xs[1].T));
//             Assert.True(Utilities.AlmostEqual(5.0, xs[2].T));
//             Assert.True(Utilities.AlmostEqual(5.0 + root3, xs[3].T));
//         }

//         [Fact]
//         public void ARayMissesATorus()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, -10, -5), new Vector(0, 2, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Empty(xs);
//         }

//         [Fact]
//         public void ARayOriginatesInsideATorusHole()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.True(Utilities.AlmostEqual(-2.0, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(-1.0, xs[1].T));
//             Assert.True(Utilities.AlmostEqual(1.0, xs[2].T));
//             Assert.True(Utilities.AlmostEqual(2.0, xs[3].T));
//         }

//         [Fact]
//         public void ARayOriginatesInsideATorusRing()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, 0, -1.5), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.True(Utilities.AlmostEqual(-0.5, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(0.5, xs[1].T));
//             Assert.True(Utilities.AlmostEqual(2.5, xs[2].T));
//             Assert.True(Utilities.AlmostEqual(3.5, xs[3].T));
//         }

//         [Fact]
//         public void ATorusIsBehindARay()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, 0, 5), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.True(Utilities.AlmostEqual(-7.0, xs[0].T));
//             Assert.True(Utilities.AlmostEqual(-6.0, xs[1].T));
//             Assert.True(Utilities.AlmostEqual(-4.0, xs[2].T));
//             Assert.True(Utilities.AlmostEqual(-3.0, xs[3].T));
//         }

//         [Fact]
//         public void IntersectSetsTheObjectOnTheIntersection()
//         {
//             // Arrange
//             var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
//             var s = new Torus();

//             // Act
//             var xs = s.LocalIntersects(r);

//             // Assert
//             Assert.Equal(4, xs.Length);
//             Assert.Equal(s, xs[0].Obj);
//             Assert.Equal(s, xs[1].Obj);
//             Assert.Equal(s, xs[2].Obj);
//             Assert.Equal(s, xs[3].Obj);
//         }

//         [Fact]
//         public void ATorusHasADefaultTransformation()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = Matrix.Identity(4);

//             // Act

//             // Assert
//             Assert.Equal(expected, s.Transform);
//         }

//         [Fact]
//         public void ChangingATorusTransformation()
//         {
//             // Arrange
//             var s = new Torus();
//             var t = Transformations.Translation(2, 3, 4);

//             // Act
//             s.Transform = t;

//             // Assert
//             Assert.Equal(t, s.Transform);
//         }

//         [Fact]
//         public void TheNormalOnATorusOuterEdgeAtAPointOnTheXAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(1, 0, 0);

//             // Act
//             var result = s.LocalNormalAt(new Point(2, 0, 0));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusInnerEdgeAtAPointOnTheXAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(-1, 0, 0);

//             // Act
//             var result = s.LocalNormalAt(new Point(1, 0, 0));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusAtAPointOnThePositiveYAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(0, 1, 0);

//             // Act
//             var result = s.LocalNormalAt(new Point(1.5, 0.5, 0));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusAtAPointOnTheNegativeYAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(0, -1, 0);

//             // Act
//             var result = s.LocalNormalAt(new Point(0, -0.5, 1.5));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusOuterEdgeAtAPointOnTheZAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(0, 0, 1);

//             // Act
//             var result = s.LocalNormalAt(new Point(0, 0, 2));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusInnerEdgeAtAPointOnTheZAxis()
//         {
//             // Arrange
//             var s = new Torus();
//             var expected = new Vector(0, 0, -1);

//             // Act
//             var result = s.LocalNormalAt(new Point(0, 0, 1));

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusAtANonaxialPoint()
//         {
//             // Arrange
//             var halfSqrt2 = Math.Sqrt(2) / 2.0;
//             var s = new Torus();
//             var surfacePoint = new Point((s.MajorRadius - s.MinorRadius) * halfSqrt2, s.MinorRadius * halfSqrt2, (s.MajorRadius - s.MinorRadius) * halfSqrt2);
//             var expected = new Vector(-0.55121, 0.62637, -0.55121);

//             // Act
//             var result = s.LocalNormalAt(surfacePoint);

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalOnATorusAtAnotherNonaxialPoint()
//         {
//             // Arrange
//             var halfSqrt2 = Math.Sqrt(2) / 2.0;
//             var s = new Torus();
//             var surfacePoint = new Point((-s.MajorRadius - s.MinorRadius) * halfSqrt2, -s.MinorRadius * halfSqrt2, (-s.MajorRadius - s.MinorRadius) * halfSqrt2);
//             var expected = new Vector(-0.58844, -0.5545, -0.58844);

//             // Act
//             var result = s.LocalNormalAt(surfacePoint);

//             // Assert
//             Assert.Equal(expected, result);
//         }

//         [Fact]
//         public void TheNormalIsANormalisedVector()
//         {
//             // Arrange
//             var halfSqrt2 = Math.Sqrt(2) / 2.0;
//             var s = new Torus();
//             var surfacePoint = new Point((s.MajorRadius - s.MinorRadius) * halfSqrt2, s.MinorRadius * halfSqrt2, (s.MajorRadius - s.MinorRadius) * halfSqrt2);

//             // Act
//             var result = s.LocalNormalAt(surfacePoint);

//             // Assert
//             Assert.Equal(result, result.Normalise());
//         }

//         [Fact]
//         public void ATorusIsAShape()
//         {
//             // Arrange
//             var s = new Torus();

//             // Act

//             // Assert
//             Assert.IsAssignableFrom<Shape>(s);
//         }

        // [Fact]
        // public void CloningATorus()
        // {
        //     // Arrange
        //     var minRadius = 1.2;
        //     var maxRadius = 3.4;
        //     var orig = new Torus(minRadius, maxRadius);

        //     // Act
        //     var clone = orig.Clone();

        //     // Assert
        //     Assert.Equal(orig.Origin, clone.Origin);
        //     Assert.Equal(orig.Material, clone.Material);
        //     Assert.Equal(orig.Transform, clone.Transform);
        //     Assert.Equal(orig.Parent, clone.Parent);
        //     Assert.Equal(orig.MinRadius, clone.MinRadius);
        //     Assert.Equal(orig.MaxRadius, clone.MaxRadius);
        // }
//     }
// }
