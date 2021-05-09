using Xunit;

namespace RayTracer.Tests
{
    public class ConeTests
    {
        [Fact]
        public void AConeHasDefaultMinimumAndMaximumExtents()
        {
            // Arrange
            var c = new Cone();

            // Act

            // Assert
            Assert.Equal(double.NegativeInfinity, c.Minimum);
            Assert.Equal(double.PositiveInfinity, c.Maximum);
        }

        [Fact]
        public void AConeHasDefaultClosedValue()
        {
            // Arrange
            var c = new Cone();

            // Act

            // Assert
            Assert.False(c.Closed);
        }

        [Fact]
        public void ARayMissesACone()
        {
            // Arrange
            var c = new Cone();
            bool allTestsPass = true;
            var origins = new Point[] {new Point(1, 0, 0), new Point(-2, 0, 1), new Point(0, 1, -5)};
            var directions = new Vector[] {new Vector(0, 0.5, -0.5), new Vector(-1, 1, -1), new Vector(-1, 0.5, 0)};

            for (int i = 0; i < origins.Length; i++)
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
        public void ARayIntersectsACone()
        {
            // Arrange
            var c = new Cone();
            bool allTestsPass = true;
            var origins = new Point[] {new Point(0, 0, -5), new Point(0, 0, -5), new Point(1, 1, -5)};
            var directions = new Vector[] {new Vector(0, 0, 1), new Vector(1, 1, 1), new Vector(-0.5, -1, 1)};
            var t1s = new double[] {5, 8.66025, 4.55006};
            var t2s = new double[] {5, 8.66025, 49.44994};

            for (int i = 0; i < origins.Length; i++)
            {
                var r = new Ray(origins[i], directions[i].Normalise());     // MUST remember to normalise rays!!!

                // Act
                var xs = c.LocalIntersects(r);

                // Assert
                allTestsPass &= xs.Length == 2;
                allTestsPass &= Utilities.AlmostEqual(t1s[i], xs[0].T);
                allTestsPass &= Utilities.AlmostEqual(t2s[i], xs[1].T);
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void IntersectingAConeWithARayRunningParallelToOneOfTheSlopedSurfaces()
        {
            // Arrange
            var c = new Cone();
            var r = new Ray(new Point(0, 0, -1), new Vector(0, 1, 1).Normalise());     // MUST remember to normalise rays!!!

            // Act
            var xs = c.LocalIntersects(r);

            // Assert
            Assert.Single(xs);
            Assert.True(Utilities.AlmostEqual(0.35355, xs[0].T));
        }

        [Fact]
        public void IntersectingAConstrainedCone()
        {
            // Arrange
            var c = new Cone();
            c.Minimum = 1;
            c.Maximum = 2;
            bool allTestsPass = true;
            var origins = new Point[] {new Point(0, 1.5, 0), new Point(0, 3, -5), new Point(0, 0, -5), new Point(0, 2, -5), new Point(0, 1, -5), new Point(0, 1.5, -2)};
            var directions = new Vector[] {new Vector(0.1, 1, 0), new Vector(0, 0, 1), new Vector(0, 0, 1), new Vector(0, 0, 1), new Vector(0, 0, 1), new Vector(0, 0, 1)};
            var counts = new int[] {0, 0, 0, 0, 0, 2};

            for (int i = 0; i < origins.Length; i++)
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
        public void IntersectingTheEndCapsOfAClosedCone()
        {
            // Arrange
            var c = new Cone();
            c.Minimum = -0.5;
            c.Maximum = 0.5;
            c.Closed = true;
            bool allTestsPass = true;
            var origins = new Point[] {new Point(0, 0, -5), new Point(0, 0, -0.25), new Point(0, 0, -0.25)};
            var directions = new Vector[] {new Vector(0, 1, 0), new Vector(0, 1, 1), new Vector(0, 1, 0)};
            var counts = new int[] {0, 2, 4};

            for (int i = 0; i < origins.Length; i++)
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
        public void TheNormalOnTheSurfaceOfACone()
        {
            // Arrange
            var c = new Cone();
            bool allTestsPass = true;
            var points = new Point[] {new Point(0, 0, 0), new Point(1, 1, 1), new Point(-1, -1, 0)};
            var normals = new Vector[] {new Vector(0, 0, 0), new Vector(1, -System.Math.Sqrt(2), 1), new Vector(-1, 1, 0)};

            for (int i = 0; i < points.Length; i++)
            {
                // Act
                var normal = c.LocalNormalAt(points[i]);

                // Assert
                allTestsPass &= normal == normals[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void TheNormalOnTheEndCapsOfACone()
        {
            // Arrange
            var c = new Cone();
            c.Minimum = 1;
            c.Maximum = 2;
            c.Closed = true;
            bool allTestsPass = true;
            var points = new Point[] {new Point(0, 1, 0), new Point(0.5, 1, 0), new Point(0, 1, 0.5), new Point(0, 2, 0), new Point(0.5, 2, 0), new Point(0, 2, 0.5)};
            var normals = new Vector[] {new Vector(0, -1, 0), new Vector(0, -1, 0), new Vector(0, -1, 0), new Vector(0, 1, 0), new Vector(0, 1, 0), new Vector(0, 1, 0)};

            for (int i = 0; i < points.Length; i++)
            {
                // Act
                var normal = c.LocalNormalAt(points[i]);

                // Assert
                allTestsPass &= normal == normals[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void AnUnboundedConeHasABoundingBox()
        {
            // Arrange
            var s = new Cone();

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
        public void ABoundedConeHasABoundingBox()
        {
            // Arrange
            var s = new Cone();
            s.Minimum = -5;
            s.Maximum = 3;

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-5, -5, -5), boundingBox.MinPoint);
            Assert.Equal(new Point(5, 3, 5), boundingBox.MaxPoint);
        }
    }
}
