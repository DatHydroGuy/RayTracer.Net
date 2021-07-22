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
            var points = new[] {new Point(0, 0, -1), new Point(1, 0, 0), new Point(0, 0, 1), new Point(-1, 0, 0), new Point(0, 1, 0),
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
            var points = new[] {new Point(0.25, 0, 0.5), new Point(0.25, 0, -0.25), new Point(0.25, 0.5, -0.25), new Point(1.25, 0, 0.5),
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
            var points = new[] {new Point(0, 0, -1), new Point(0, 0.5, -1), new Point(0, 1, -1), new Point(0.70711, 0.5, -0.70711),
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

        [Fact]
        public void IdentifyingTheFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(-1, 0.5, -0.25), new Point(1.1, -0.75, 0.8),
                new Point(0.1, 0.6, 0.9), new Point(-0.7, 0, -2),
                new Point(0.5, 1, 0.9), new Point(-0.2, -1.3, 1.1)};
            var faces = new [] {CubeFace.Left, CubeFace.Right, CubeFace.Front, CubeFace.Back, CubeFace.Upper, CubeFace.Lower};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var result = UvPatternMaps.FaceFromPoint(points[i]);
                allTestsPass &= result == faces[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheFrontFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(-0.5, 0.5, 1), new Point(0.5, -0.5, 1)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeFront(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheBackFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(0.5, 0.5, -1), new Point(-0.5, -0.5, -1)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeBack(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheLeftFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(-1, 0.5, -0.5), new Point(-1, -0.5, 0.5)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeLeft(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheRightFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(1, 0.5, 0.5), new Point(1, -0.5, -0.5)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeRight(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheUpperFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(-0.5, 1, -0.5), new Point(0.5, 1, 0.5)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeUpper(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void UvMappingTheLowerFaceOfACubeFromA3DPoint()
        {
            // Arrange
            var points = new[] {new Point(-0.5, -1, 0.5), new Point(0.5, -1, -0.5)};
            var uValues = new [] {0.25, 0.75};
            var vValues = new [] {0.75, 0.25};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var (resultU, resultV) = UvPatternMaps.UvMapCubeLower(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }
    }
}
