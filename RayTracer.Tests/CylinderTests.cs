using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class CylinderTests
    {
        [Fact]
        public void ACylinderHasDefaultMinimumAndMaximumExtents()
        {
            // Arrange
            var c = new Cylinder();

            // Act

            // Assert
            Assert.Equal(double.NegativeInfinity, c.Minimum);
            Assert.Equal(double.PositiveInfinity, c.Maximum);
        }
        
        [Fact]
        public void ACylinderHasDefaultClosedValue()
        {
            // Arrange
            var c = new Cylinder();

            // Act

            // Assert
            Assert.False(c.Closed);
        }

        [Fact]
        public void ARayMissesACylinder()
        {
            // Arrange
            var c = new Cylinder();
            var allTestsPass = true;
            var origins = new Point[] {new(1, 0, 0), new(0, 0, 0), new(0, 0, -5)};
            var directions = new Vector[] {new(0, 1, 0), new(0, 1, 0), new(1, 1, 1)};

            for (var i = 0; i < origins.Length; i++)
            {
                var r = new Ray(origins[i], directions[i]);

                // Act
                var xs = c.LocalIntersects(r);

                // Assert
                allTestsPass &= xs.Length == 0;
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void ARayIntersectsACylinder()
        {
            // Arrange
            var c = new Cylinder();
            var allTestsPass = true;
            var origins = new Point[] {new(1, 0, -5), new(0, 0, -5), new(0.5, 0, -5)};
            var directions = new Vector[] {new(0, 0, 1), new(0, 0, 1), new(0.1, 1, 1)};
            var t1Values = new[] {5, 4, 6.80798};
            var t2Values = new[] {5, 6, 7.08872};

            for (var i = 0; i < origins.Length; i++)
            {
                var r = new Ray(origins[i], directions[i].Normalise());     // MUST remember to normalise rays!!!

                // Act
                var xs = c.LocalIntersects(r);

                // Assert
                allTestsPass &= xs.Length == 2;
                allTestsPass &= Utilities.AlmostEqual(t1Values[i], xs[0].T);
                allTestsPass &= Utilities.AlmostEqual(t2Values[i], xs[1].T);
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void IntersectingAConstrainedCylinder()
        {
            // Arrange
            var c = new Cylinder
            {
                Minimum = 1,
                Maximum = 2
            };
            var allTestsPass = true;
            var origins = new Point[] {new(0, 1.5, 0), new(0, 3, -5), new(0, 0, -5), new(0, 2, -5), new(0, 1, -5), new(0, 1.5, -2)};
            var directions = new Vector[] {new(0.1, 1, 0), new(0, 0, 1), new(0, 0, 1), new(0, 0, 1), new(0, 0, 1), new(0, 0, 1)};
            var counts = new[] {0, 0, 0, 0, 0, 2};

            for (var i = 0; i < origins.Length; i++)
            {
                var r = new Ray(origins[i], directions[i].Normalise());     // MUST remember to normalise rays!!!

                // Act
                var xs = c.LocalIntersects(r);

                // Assert
                allTestsPass &= xs.Length == counts[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void IntersectingTheEndCapsOfAClosedCylinder()
        {
            // Arrange
            var c = new Cylinder
            {
                Minimum = 1,
                Maximum = 2,
                Closed = true
            };
            var allTestsPass = true;
            var origins = new Point[] {new(0, 3, 0), new(0, 3, -2), new(0, 4, -2), new(0, 0, -2), new(0, -1, -2)};
            var directions = new Vector[] {new(0, -1, 0), new(0, -1, 2), new(0, -1, 1), new(0, 1, 2), new(0, 1, 1)};
            var counts = new[] {2, 2, 2, 2, 2};

            for (var i = 0; i < origins.Length; i++)
            {
                var r = new Ray(origins[i], directions[i].Normalise());     // MUST remember to normalise rays!!!

                // Act
                var xs = c.LocalIntersects(r);

                // Assert
                allTestsPass &= xs.Length == counts[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void TheNormalOnTheSurfaceOfACylinder()
        {
            // Arrange
            var c = new Cylinder();
            var allTestsPass = true;
            var points = new Point[] {new(1, 0, 0), new(0, 5, -1), new(0, -2, 1), new(-1, 1, 0)};
            var normals = new Vector[] {new(1, 0, 0), new(0, 0, -1), new(0, 0, 1), new(-1, 0, 0)};

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var normal = c.LocalNormalAt(points[i]);

                // Assert
                allTestsPass &= normal == normals[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void TheNormalOnTheEndCapsOfACylinder()
        {
            // Arrange
            var c = new Cylinder
            {
                Minimum = 1,
                Maximum = 2,
                Closed = true
            };
            var allTestsPass = true;
            var points = new Point[] {new(0, 1, 0), new(0.5, 1, 0), new(0, 1, 0.5), new(0, 2, 0), new(0.5, 2, 0), new(0, 2, 0.5)};
            var normals = new Vector[] {new(0, -1, 0), new(0, -1, 0), new(0, -1, 0), new(0, 1, 0), new(0, 1, 0), new(0, 1, 0)};

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var normal = c.LocalNormalAt(points[i]);

                // Assert
                allTestsPass &= normal == normals[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void AnUnboundedCylinderHasABoundingBox()
        {
            // Arrange
            var s = new Cylinder();

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.True(Utilities.AlmostEqual(-1, boundingBox.MinPoint.X));
            Assert.True(double.IsNegativeInfinity(boundingBox.MinPoint.Y));
            Assert.True(Utilities.AlmostEqual(-1, boundingBox.MinPoint.Z));
            Assert.True(Utilities.AlmostEqual(1, boundingBox.MaxPoint.X));
            Assert.True(double.IsPositiveInfinity(boundingBox.MaxPoint.Y));
            Assert.True(Utilities.AlmostEqual(1, boundingBox.MaxPoint.Z));
        }

        [Fact]
        public void ABoundedCylinderHasABoundingBox()
        {
            // Arrange
            var s = new Cylinder
            {
                Minimum = -5,
                Maximum = 3
            };

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-1, -5, -1), boundingBox.MinPoint);
            Assert.Equal(new Point(1, 3, 1), boundingBox.MaxPoint);
        }

        [Fact]
        public void CloningACylinder()
        {
            // Arrange
            var orig = new Cylinder();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
            Assert.Equal(orig.Radius, clone.Radius);
            Assert.Equal(orig.Minimum, clone.Minimum);
            Assert.Equal(orig.Maximum, clone.Maximum);
            Assert.Equal(orig.Closed, clone.Closed);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "[Type:RayTracer.Shapes.Cylinder\nId:637602294772396341\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nRadius:1,Min:-∞,Max:∞,Closed:False\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var orig = new Cylinder();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
