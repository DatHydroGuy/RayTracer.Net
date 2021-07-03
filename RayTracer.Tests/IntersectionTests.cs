using System;
using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class IntersectionTests
    {
        [Fact]
        public void AnIntersectionEncapsulatesTAndObject()
        {
            // Arrange
            var s = new Sphere();

            // Act
            var i = new Intersection(3.5, s);

            // Assert
            Assert.Equal(3.5, i.T);
            Assert.Equal(s, i.Obj);
        }

        [Fact]
        public void AggregatingIntersections()
        {
            // Arrange
            var s = new Sphere();
            var i1 = new Intersection(1, s);
            var i2 = new Intersection(2, s);

            // Act
            var xs = Intersection.Intersections(i1, i2);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.Equal(1, xs[0].T);
            Assert.Equal(2, xs[1].T);
        }

        [Fact]
        public void FindingTheHitWhenAllIntersectionsHavePositiveTValues()
        {
            // Arrange
            var s = new Sphere();
            var i1 = new Intersection(1, s);
            var i2 = new Intersection(2, s);
            var xs = Intersection.Intersections(i1, i2);

            // Act
            var i = Intersection.Hit(xs);

            // Assert
            Assert.Equal(i1, i);
        }

        [Fact]
        public void FindingTheHitWhenSomeIntersectionsHaveNegativeTValues()
        {
            // Arrange
            var s = new Sphere();
            var i1 = new Intersection(-1, s);
            var i2 = new Intersection(1, s);
            var xs = Intersection.Intersections(i1, i2);

            // Act
            var i = Intersection.Hit(xs);

            // Assert
            Assert.Equal(i2, i);
        }

        [Fact]
        public void FindingTheHitWhenAllIntersectionsHaveNegativeTValues()
        {
            // Arrange
            var s = new Sphere();
            var i1 = new Intersection(-2, s);
            var i2 = new Intersection(-1, s);
            var xs = Intersection.Intersections(i1, i2);

            // Act
            var i = Intersection.Hit(xs);

            // Assert
            Assert.Null(i);
        }

        [Fact]
        public void TheHitIsAlwaysTheLowestNonNegativeIntersection()
        {
            // Arrange
            var s = new Sphere();
            var i1 = new Intersection(5, s);
            var i2 = new Intersection(7, s);
            var i3 = new Intersection(-3, s);
            var i4 = new Intersection(2, s);
            var xs = Intersection.Intersections(i1, i2, i3, i4);

            // Act
            var i = Intersection.Hit(xs);

            // Assert
            Assert.Equal(i4, i);
        }

        [Fact]
        public void PrecomputingTheStateOfAnIntersection()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();
            var i = new Intersection(4, s);

            // Act
            var comps = i.PrepareComputations(r);

            // Assert
            Assert.Equal(i.T, comps.T);
            Assert.Equal(i.Obj, comps.Obj);
            Assert.Equal(new Point(0, 0, -1), comps.TargetPoint);
            Assert.Equal(new Vector(0, 0, -1), comps.EyeVector);
            Assert.Equal(new Vector(0, 0, -1), comps.NormalVector);
        }

        [Fact]
        public void CalculateTheHitWhenIntersectionIsOnOutsideOfShape()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();
            var i = new Intersection(4, s);

            // Act
            var comps = i.PrepareComputations(r);

            // Assert
            Assert.False(comps.Inside);
        }

        [Fact]
        public void CalculateTheHitWhenIntersectionIsOnInsideOfShape()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var s = new Sphere();
            var i = new Intersection(1, s);

            // Act
            var comps = i.PrepareComputations(r);

            // Assert
            Assert.Equal(new Point(0, 0, 1), comps.TargetPoint);
            Assert.Equal(new Vector(0, 0, -1), comps.EyeVector);
            Assert.Equal(new Vector(0, 0, -1), comps.NormalVector);
            Assert.True(comps.Inside);
        }

        [Fact]
        public void TheHitShouldBeCalculatedAgainstOverPointRatherThanPoint()
        {
            // Arrange
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();
            s.Transform = Transformations.Translation(0, 0, 1);
            var i = new Intersection(5, s);

            // Act
            var comps = i.PrepareComputations(r);

            // Assert
            Assert.True(comps.OverPoint.Z < -Utilities.EPSILON / 2.0);
            Assert.True(comps.TargetPoint.Z > comps.OverPoint.Z);
        }

        [Fact]
        public void PrecomputingTheReflectionVector()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var s = new Plane();
            var r = new Ray(new Point(0, 1, -1), new Vector(0, -halfSqrt2, halfSqrt2));
            var i = new Intersection(Math.Sqrt(2), s);
            var expected = new Vector(0, halfSqrt2, halfSqrt2);

            // Act
            var comps = i.PrepareComputations(r);

            // Assert
            Assert.Equal(expected, comps.ReflectVector);
        }

        [Fact]
        public void FindingN1AndN2AtIntersectionsOfDifferingRefractiveIndices()
        {
            // Arrange
            var sphereA = Sphere.GlassSphere();
            sphereA.Transform = Transformations.Scaling(2, 2, 2);
            sphereA.Material.RefractiveIndex = 1.5;
            var sphereB = Sphere.GlassSphere();
            sphereB.Transform = Transformations.Translation(0, 0, -0.25);
            sphereB.Material.RefractiveIndex = 2;
            var sphereC = Sphere.GlassSphere();
            sphereC.Transform = Transformations.Translation(0, 0, 0.25);
            sphereC.Material.RefractiveIndex = 2.5;
            var r = new Ray(new Point(0, 0, -4), new Vector(0, 0, 1));
            var expectedN1s = new double[] {1.0, 1.5, 2.0, 2.5, 2.5, 1.5};
            var expectedN2s = new double[] {1.5, 2.0, 2.5, 2.5, 1.5, 1.0};
            var xs = Intersection.Intersections(new Intersection(2.0, sphereA), new Intersection(2.75, sphereB), new Intersection(3.25, sphereC),
                                                new Intersection(4.75, sphereB), new Intersection(5.25, sphereC), new Intersection(6.0, sphereA));
            bool testsPassing = true;

            for (int i = 0; i < 6; i++)
            {
                // Act
                var comps = xs[i].PrepareComputations(r, xs);
                testsPassing &= Utilities.AlmostEqual(expectedN1s[i], comps.N1);
                testsPassing &= Utilities.AlmostEqual(expectedN2s[i], comps.N2);
            }

            // Assert
            Assert.True(testsPassing);
        }

        [Fact]
        public void TheUnderPointIsOffsetBelowTheSurface()
        {
            // Arrange
            var s = Sphere.GlassSphere();
            s.Transform = Transformations.Translation(0, 0, 1);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var i = new Intersection(5, s);
            var xs = Intersection.Intersections(i);

            // Act
            var comps = i.PrepareComputations(r, xs);

            // Assert
            Assert.True(comps.UnderPoint.Z > Utilities.EPSILON / 2.0);
            Assert.True(comps.TargetPoint.Z < comps.UnderPoint.Z);
        }

        [Fact]
        public void SchlickApproximationUnderTotalInternalReflection()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var s = Sphere.GlassSphere();
            var r = new Ray(new Point(0, 0, halfSqrt2), new Vector(0, 1, 0));
            var xs = Intersection.Intersections(new Intersection(-halfSqrt2, s), new Intersection(halfSqrt2, s));

            // Act
            var comps = xs[1].PrepareComputations(r, xs);
            var reflectance = Intersection.Schlick(comps);

            // Assert
            Assert.True(Utilities.AlmostEqual(1.0, reflectance));
        }

        [Fact]
        public void SchlickApproximationWithAPerpendicularReflection()
        {
            // Arrange
            var s = Sphere.GlassSphere();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 1, 0));
            var xs = Intersection.Intersections(new Intersection(-1, s), new Intersection(1, s));

            // Act
            var comps = xs[1].PrepareComputations(r, xs);
            var reflectance = Intersection.Schlick(comps);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.04, reflectance));
        }

        [Fact]
        public void SchlickApproximationWithASmallAngleOfReflectionAndN2GreaterThanN1()
        {
            // Arrange
            var s = Sphere.GlassSphere();
            var r = new Ray(new Point(0, 0.99, -2), new Vector(0, 0, 1));
            var xs = Intersection.Intersections(new Intersection(1.8589, s));

            // Act
            var comps = xs[0].PrepareComputations(r, xs);
            var reflectance = Intersection.Schlick(comps);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.48873, reflectance));
        }

        [Fact]
        public void AnIntersectionWithATriangleCanEncapsulateUAndVCoordinates()
        {
            // Arrange
            var s = new Triangle(new Point(0, 1, 0), new Point(-1, 0, 0), new Point(1, 0, 0));

            // Act
            var i = Intersection.IntersectionWithUV(3.5, s, 0.2, 0.4);

            // Assert
            Assert.Equal(0.2, i.U);
            Assert.Equal(0.4, i.V);
        }
    }
}
