using Xunit;

namespace RayTracer.Tests
{
    public class CubeTests
    {
        [Fact]
        public void ARayIntersectsACube()
        {
            // Arrange
            var c = new Cube();
            bool allTestsPass = true;
            var origins = new Point[] {new Point(5, 0.5, 0), new Point(-5, 0.5, 0), new Point(0.5, 5, 0), new Point(0.5, -5, 0), new Point(0.5, 0, 5), new Point(0.5, 0, -5), new Point(0, 0.5, 0)};
            var directions = new Vector[] {new Vector(-1, 0, 0), new Vector(1, 0, 0), new Vector(0, -1, 0), new Vector(0, 1, 0), new Vector(0, 0, -1), new Vector(0, 0, 1), new Vector(0, 0, 1)};
            var t1s = new double[] {4, 4, 4, 4, 4, 4, -1};
            var t2s = new double[] {6, 6, 6, 6, 6, 6, 1};

            for (int i = 0; i < 7; i++)
            {
                var r = new Ray(origins[i], directions[i]);

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
        public void ARayMissesACube()
        {
            // Arrange
            var c = new Cube();
            bool allTestsPass = true;
            var origins = new Point[] {new Point(-2, 0, 0), new Point(0, -2, 0), new Point(0, 0, -2), new Point(2, 0, 2), new Point(0, 2, 2), new Point(2, 2, 0)};
            var directions = new Vector[] {new Vector(0.2673, 0.5345, 0.8018), new Vector(0.8018, 0.2673, 0.5345), new Vector(0.5345, 0.8018, 0.2673),
                                           new Vector(0, 0, -1), new Vector(0, -1, 0), new Vector(-1, 0, 0)};

            for (int i = 0; i < 6; i++)
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
        public void TheNormalOnTheSurfaceOfACube()
        {
            // Arrange
            var c = new Cube();
            bool allTestsPass = true;
            var points = new Point[] {new Point(1, 0.5, -0.8), new Point(-1, -0.2, 0.9), new Point(-0.4, 1, -0.1), new Point(0.3, -1, -0.7),
                                      new Point(-0.6, 0.3, 1), new Point(0.4, 0.4, -1), new Point(1, 1, 1), new Point(-1, -1, -1)};
            var normals = new Vector[] {new Vector(1, 0, 0), new Vector(-1, 0, 0), new Vector(0, 1, 0), new Vector(0, -1, 0),
                                        new Vector(0, 0, 1), new Vector(0, 0, -1), new Vector(1, 0, 0), new Vector(-1, 0, 0)};

            for (int i = 0; i < 8; i++)
            {
                // Act
                var normal = c.LocalNormalAt(points[i]);

                // Assert
                allTestsPass &= normal == normals[i];
            }
            Assert.True(allTestsPass);
        }

        [Fact]
        public void ACubeHasABoundingBox()
        {
            // Arrange
            var s = new Cube();

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-1, -1, -1), boundingBox.MinPoint);
            Assert.Equal(new Point(1, 1, 1), boundingBox.MaxPoint);
        }

        [Fact]
        public void CloningACube()
        {
            // Arrange
            var orig = new Cube();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
        }
    }
}
