using System;
using Xunit;

namespace RayTracer.Tests
{
    public class UvPatternMapTests
    {
        [Fact]
        public void UsingASphericalMappingOnA3DPoint()
        {
            // Arrange
            var points = new[] { new Point(0, 0, -1), new Point(1, 0, 0), new Point(0, 0, 1), new Point(-1, 0, 0), new Point(0, 1, 0),
                new Point(0, -1, 0), new Point(Math.Sqrt(2) * 0.5, Math.Sqrt(2) * 0.5, 0)};
            var uValues = new [] {0.0, 0.25, 0.5, 0.75, 0.5, 0.5, 0.25};
            var vValues = new [] {0.5, 0.5, 0.5, 0.5, 1.0, 0.0, 0.75};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvSphericalMapping(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UsingAPlanarMappingOnA3DPoint()
        {
            // Arrange
            var points = new[] { new Point(0.25, 0, 0.5), new Point(0.25, 0, -0.25), new Point(0.25, 0.5, -0.25), new Point(1.25, 0, 0.5),
                new Point(0.25, 0, -1.75), new Point(1, 0, -1), new Point(0, 0, 0)};
            var uValues = new [] {0.25, 0.25, 0.25, 0.25, 0.25, 0.0, 0.0};
            var vValues = new [] {0.5, 0.75, 0.75, 0.5, 0.25, 0.0, 0.0};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvPlanarMapping(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UsingACylindricalMappingOnA3DPoint()
        {
            // Arrange
            var points = new[] { new Point(0, 0, -1), new Point(0, 0.5, -1), new Point(0, 1, -1), new Point(0.70711, 0.5, -0.70711),
                new Point(1, 0.5, 0), new Point(0.70711, 0.5, 0.70711), new Point(0,-0.25, 1), new Point(-0.70711, 0.5, 0.70711),
                new Point(-1, 1.25, 0), new Point(-0.70711, 0.5, -0.70711)};
            var uValues = new [] {0.0, 0.0, 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875};
            var vValues = new [] {0.0, 0.5, 0.0, 0.5, 0.5, 0.5, 0.75, 0.5, 0.25, 0.5};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvCylindricalMapping(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }
    }
}