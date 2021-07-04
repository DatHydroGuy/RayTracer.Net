using Xunit;

namespace RayTracer.Tests
{
    public class BoundingBoxTests
    {
        [Fact]
        public void CreatingAnEmptyBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox();

            // Act

            // Assert
            Assert.True(double.IsPositiveInfinity(aabb.MinPoint.X));
            Assert.True(double.IsPositiveInfinity(aabb.MinPoint.Y));
            Assert.True(double.IsPositiveInfinity(aabb.MinPoint.Z));
            Assert.True(double.IsNegativeInfinity(aabb.MaxPoint.X));
            Assert.True(double.IsNegativeInfinity(aabb.MaxPoint.Y));
            Assert.True(double.IsNegativeInfinity(aabb.MaxPoint.Z));
        }

        [Fact]
        public void CreatingABoundingBoxWithVolume()
        {
            // Arrange
            var minBoundsPoint = new Point(-1, -2, -3);
            var maxBoundsPoint = new Point(3, 2, 1);

            // Act
            var aabb = new BoundingBox(minBoundsPoint, maxBoundsPoint);

            // Assert
            Assert.Equal(minBoundsPoint, aabb.MinPoint);
            Assert.Equal(maxBoundsPoint, aabb.MaxPoint);
        }

        [Fact]
        public void AddPointsToAnEmptyBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox();
            var p1 = new Point(-5, 2, 0);
            var p2 = new Point(7, 0, -3);
            var expectedMin = new Point(-5, 0, -3);
            var expectedMax = new Point(7, 2, 0);

            // Act
            aabb.AddPoint(p1);
            aabb.AddPoint(p2);

            // Assert
            Assert.Equal(expectedMin, aabb.MinPoint);
            Assert.Equal(expectedMax, aabb.MaxPoint);
        }

        [Fact]
        public void AddingOneBoundingBoxToAnother()
        {
            // Arrange
            var aabb1 = new BoundingBox(new Point(-5, -2, 0), new Point(7, 4, 4));
            var aabb2 = new BoundingBox(new Point(8, -7, -2), new Point(14, 2, 8));
            var expectedMin = new Point(-5, -7, -2);
            var expectedMax = new Point(14, 4, 8);

            // Act
            aabb1.AddBox(aabb2);

            // Assert
            Assert.Equal(expectedMin, aabb1.MinPoint);
            Assert.Equal(expectedMax, aabb1.MaxPoint);
        }

        [Fact]
        public void CheckToSeeIfABoundingBoxContainsAGivenPoint()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var points = new Point[] {new Point(5, -2, 0), new Point(11, 4, 7), new Point(8, 1, 3), new Point(3, 0, 3), new Point(8, -4, 3),
                                      new Point(8, 1, -1), new Point(13, 1, 3), new Point(8, 5, 3), new Point(8, 1, 8)};
            var expected = new bool[] {true, true, true, false, false, false, false, false, false};
            var allTestsPassed = true;

            for (var i = 0; i < expected.Length; i++)
            {
                // Act
                var result = aabb.ContainsPoint(points[i]);

                // Assert
                allTestsPassed &= expected[i] == result;
            }

            Assert.True(allTestsPassed);
        }

        [Fact]
        public void CheckToSeeIfABoundingBoxContainsAGivenBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var boxes = new BoundingBox[] {new(new Point(5, -2, 0), new Point(11, 4, 7)),
                                           new(new Point(6, -1, 1), new Point(10, 3, 6)),
                                           new(new Point(4, -3, -1), new Point(10, 3, 6)),
                                           new(new Point(6, -1, 1), new Point(12, 5, 8))};
            var expected = new[] {true, true, false, false};
            var allTestsPassed = true;

            for (var i = 0; i < expected.Length; i++)
            {
                // Act
                var result = aabb.ContainsBox(boxes[i]);

                // Assert
                allTestsPassed &= expected[i] == result;
            }

            Assert.True(allTestsPassed);
        }

        [Fact]
        public void TransformingABoundingBox()
        {
            // Arrange
            var sqrt2 = System.Math.Sqrt(2);
            var onePlusSqrtHalf = 1.0 + 1.0 / sqrt2;
            var aabb = new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
            var transform = Transformations.RotationX(System.Math.PI / 4.0) * Transformations.RotationY(System.Math.PI / 4.0);
            var expectedMin = new Point(-sqrt2, -onePlusSqrtHalf, -onePlusSqrtHalf);
            var expectedMax = new Point(sqrt2, onePlusSqrtHalf, onePlusSqrtHalf);

            // Act
            var result = aabb.TransformBox(transform);

            // Assert
            Assert.Equal(expectedMin, result.MinPoint);
            Assert.Equal(expectedMax, result.MaxPoint);
        }

        [Fact]
        public void IntersectingARayWithABoundingBoxAtTheOrigin()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
            var origins = new Point[] {new(5, 0.5, 0), new(-5, 0.5, 0), new(0.5, 5, 0), new(0.5, -5, 0), new(0.5, 0, 5),
                                       new(0.5, 0, -5), new(0, 0.5, 0), new(-2, 0, 0), new(0, -2, 0), new(0, 0, -2),
                                       new(2, 0, 2), new(0, 2, 2), new(2, 2, 0)};
            var directions = new Vector[] {new(-1, 0, 0), new(1, 0, 0), new(0, -1, 0), new(0, 1, 0), new(0, 0, -1),
                                           new(0, 0, 1), new(0, 0, 1), new(2, 4, 6), new(6, 2, 4), new(4, 6, 2),
                                           new(0, 0, -1), new(0, -1, 0), new(-1, 0, 0)};
            var expected = new[] {true, true, true, true, true, true, true, false, false, false, false, false, false};
            var allTestsPassing = true;

            for (var i = 0; i < expected.Length; i++)
            {
                var ray = new Ray(origins[i], directions[i].Normalise());
    
                // Act
                allTestsPassing &= expected[i] == aabb.Intersects(ray);
            }

            // Assert
            Assert.True(allTestsPassing);
        }

        [Fact]
        public void IntersectingARayWithANonCubicBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var origins = new Point[] {new(15, 1, 2), new(-5, -1, 4), new(7, 6, 5), new(9, -5, 6), new(8, 2, 12),
                                       new(6, 0, -5), new(8, 1, 3.5), new(9, -1, -8), new(8, 3, -4), new(9, -1, -2),
                                       new(4, 0, 9), new(8, 6, -1), new(12, 5, 4)};
            var directions = new Vector[] {new(-1, 0, 0), new(1, 0, 0), new(0, -1, 0), new(0, 1, 0), new(0, 0, -1),
                                           new(0, 0, 1), new(0, 0, 1), new(2, 4, 6), new(6, 2, 4), new(4, 6, 2),
                                           new(0, 0, -1), new(0, -1, 0), new(-1, 0, 0)};
            var expected = new[] {true, true, true, true, true, true, true, false, false, false, false, false, false};
            var allTestsPassing = true;

            for (var i = 0; i < expected.Length; i++)
            {
                var ray = new Ray(origins[i], directions[i].Normalise());
    
                // Act
                allTestsPassing &= expected[i] == aabb.Intersects(ray);
            }

            // Assert
            Assert.True(allTestsPassing);
        }

        [Fact]
        public void SplittingACubicBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(-1, -4, -5), new Point(9, 6, 5));
            var expectedMinLeft = new Point(-1, -4, -5);
            var expectedMaxLeft = new Point(4, 6, 5);
            var expectedMinRight = new Point(4, -4, -5);
            var expectedMaxRight = new Point(9, 6, 5);

            // Act
            aabb.SplitBounds(out var left, out var right);

            // Assert
            Assert.Equal(expectedMinLeft, left.MinPoint);
            Assert.Equal(expectedMaxLeft, left.MaxPoint);
            Assert.Equal(expectedMinRight, right.MinPoint);
            Assert.Equal(expectedMaxRight, right.MaxPoint);
        }

        [Fact]
        public void SplittingAnXWideBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(-1, -2, -3), new Point(9, 5.5, 3));
            var expectedMinLeft = new Point(-1, -2, -3);
            var expectedMaxLeft = new Point(4, 5.5, 3);
            var expectedMinRight = new Point(4, -2, -3);
            var expectedMaxRight = new Point(9, 5.5, 3);

            // Act
            aabb.SplitBounds(out var left, out var right);

            // Assert
            Assert.Equal(expectedMinLeft, left.MinPoint);
            Assert.Equal(expectedMaxLeft, left.MaxPoint);
            Assert.Equal(expectedMinRight, right.MinPoint);
            Assert.Equal(expectedMaxRight, right.MaxPoint);
        }

        [Fact]
        public void SplittingAYWideBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(-1, -2, -3), new Point(5, 8, 3));
            var expectedMinLeft = new Point(-1, -2, -3);
            var expectedMaxLeft = new Point(5, 3, 3);
            var expectedMinRight = new Point(-1, 3, -3);
            var expectedMaxRight = new Point(5, 8, 3);

            // Act
            aabb.SplitBounds(out var left, out var right);

            // Assert
            Assert.Equal(expectedMinLeft, left.MinPoint);
            Assert.Equal(expectedMaxLeft, left.MaxPoint);
            Assert.Equal(expectedMinRight, right.MinPoint);
            Assert.Equal(expectedMaxRight, right.MaxPoint);
        }

        [Fact]
        public void SplittingAZWideBoundingBox()
        {
            // Arrange
            var aabb = new BoundingBox(new Point(-1, -2, -3), new Point(5, 3, 7));
            var expectedMinLeft = new Point(-1, -2, -3);
            var expectedMaxLeft = new Point(5, 3, 2);
            var expectedMinRight = new Point(-1, -2, 2);
            var expectedMaxRight = new Point(5, 3, 7);

            // Act
            aabb.SplitBounds(out var left, out var right);

            // Assert
            Assert.Equal(expectedMinLeft, left.MinPoint);
            Assert.Equal(expectedMaxLeft, left.MaxPoint);
            Assert.Equal(expectedMinRight, right.MinPoint);
            Assert.Equal(expectedMaxRight, right.MaxPoint);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "[Type:RayTracer.BoundingBox\nMinPoint:[X:∞, Y:∞, Z:∞, W:1]\nMaxPoint:[X:-∞, Y:-∞, Z:-∞, W:1]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\n]";
            var orig = new BoundingBox();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
