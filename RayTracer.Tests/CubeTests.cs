using RayTracer.Shapes;
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
            var allTestsPass = true;
            var origins = new Point[] {new(5, 0.5, 0), new(-5, 0.5, 0), new(0.5, 5, 0), new(0.5, -5, 0), new(0.5, 0, 5), new(0.5, 0, -5), new(0, 0.5, 0)};
            var directions = new Vector[] {new(-1, 0, 0), new(1, 0, 0), new(0, -1, 0), new(0, 1, 0), new(0, 0, -1), new(0, 0, 1), new(0, 0, 1)};
            var t1Values = new double[] {4, 4, 4, 4, 4, 4, -1};
            var t2Values = new double[] {6, 6, 6, 6, 6, 6, 1};

            for (var i = 0; i < 7; i++)
            {
                var r = new Ray(origins[i], directions[i]);

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
        public void ARayMissesACube()
        {
            // Arrange
            var c = new Cube();
            var allTestsPass = true;
            var origins = new Point[] {new(-2, 0, 0), new(0, -2, 0), new(0, 0, -2), new(2, 0, 2), new(0, 2, 2), new(2, 2, 0)};
            var directions = new Vector[] {new(0.2673, 0.5345, 0.8018), new(0.8018, 0.2673, 0.5345), new(0.5345, 0.8018, 0.2673),
                                           new(0, 0, -1), new(0, -1, 0), new(-1, 0, 0)};

            for (var i = 0; i < 6; i++)
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
            var allTestsPass = true;
            var points = new Point[] {new(1, 0.5, -0.8), new(-1, -0.2, 0.9), new(-0.4, 1, -0.1), new(0.3, -1, -0.7),
                                      new(-0.6, 0.3, 1), new(0.4, 0.4, -1), new(1, 1, 1), new(-1, -1, -1)};
            var normals = new Vector[] {new(1, 0, 0), new(-1, 0, 0), new(0, 1, 0), new(0, -1, 0),
                                        new(0, 0, 1), new(0, 0, -1), new(1, 0, 0), new(-1, 0, 0)};

            for (var i = 0; i < 8; i++)
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

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "[Type:RayTracer.Shapes.Cube\nId:637602294772396341\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var orig = new Cube();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
