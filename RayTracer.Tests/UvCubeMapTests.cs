using Xunit;

namespace RayTracer.Tests
{
    public class UvCubeMapTests
    {
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
                var result = UvCubeMap.FaceFromPoint(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeFront(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeBack(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeLeft(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeRight(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeUpper(points[i]);
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
                var (resultU, resultV) = UvCubeMap.UvMapCubeLower(points[i]);
                allTestsPass &= Utilities.AlmostEqual(resultU, uValues[i]);
                allTestsPass &= Utilities.AlmostEqual(resultV, vValues[i]);
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void FindingTheColoursOnAMappedCube()
        {
            // Arrange
            var red = Colour.RED;
            var yellow = new Colour(1, 1, 0);
            var brown = new Colour(1, 0.5, 0);
            var green = Colour.GREEN;
            var cyan = new Colour(0, 1, 1);
            var blue = Colour.BLUE;
            var purple = new Colour(1, 0, 1);
            var white = Colour.WHITE;

            var left = new AlignCheckPattern(yellow, cyan, red, blue, brown);
            var front = new AlignCheckPattern(cyan, red, yellow, brown, green);
            var right = new AlignCheckPattern(red, yellow, purple, green, white);
            var back = new AlignCheckPattern(green, purple, cyan, white, blue);
            var upper = new AlignCheckPattern(brown, cyan, purple, red, yellow);
            var lower = new AlignCheckPattern(purple, brown, green, blue, white);

            var pattern = new UvCubeMap(left, front, right, back, upper, lower);
            
            var points = new[] {new Point(-1, 0, 0), new Point(-1, 0.9, -0.9), new Point(-1, 0.9, 0.9), 
                new Point(-1, -0.9, -0.9), new Point(-1, -0.9, 0.9), new Point(0, 0, 1), 
                new Point(-0.9, 0.9, 1), new Point(0.9, 0.9, 1), new Point(-0.9, -0.9, 1), 
                new Point(0.9, -0.9, 1), new Point(1, 0, 0), new Point(1, 0.9, 0.9), 
                new Point(1, 0.9, -0.9), new Point(1, -0.9, 0.9), new Point(1, -0.9, -0.9), 
                new Point(0, 0, -1), new Point(0.9, 0.9, -1), new Point(-0.9, 0.9, -1), 
                new Point(0.9, -0.9, -1), new Point(-0.9, -0.9, -1), new Point(0, 1, 0), 
                new Point(-0.9, 1, -0.9), new Point(0.9, 1, -0.9), new Point(-0.9, 1, 0.9), 
                new Point(0.9, 1, 0.9), new Point(0, -1, 0), new Point(-0.9, -1, 0.9), 
                new Point(0.9, -1, 0.9), new Point(-0.9, -1, -0.9), new Point(0.9, -1, -0.9)};
            var expected = new [] {yellow, cyan, red, blue, brown, cyan, red, yellow, brown, green, red, 
                yellow, purple, green, white, green, purple, cyan, white, blue, brown, cyan, purple, red, yellow, 
                purple, brown, green, blue, white};
            var allTestsPass = true;

            for (var i = 0; i < points.Length; i++)
            {
                // Act
                var result = pattern.ColourAtPoint(points[i]);
                allTestsPass &= result == expected[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }
    }
}
