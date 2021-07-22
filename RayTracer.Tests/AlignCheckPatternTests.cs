using Xunit;

namespace RayTracer.Tests
{
    public class AlignCheckPatternTests
    {
        [Fact]
        public void LayoutOfTheAlignCheckPattern()
        {
            // Arrange
            var main = new Colour(1, 1, 1);
            var ul = new Colour(1, 0, 0);
            var ur = new Colour(1, 1, 0);
            var ll = new Colour(0, 1, 0);
            var lr = new Colour(0, 1, 1);
            var pattern = new AlignCheckPattern(main, ul, ur, ll, lr);
            var uValues = new [] {0.5, 0.1, 0.9, 0.1, 0.9};
            var vValues = new [] {0.5, 0.9, 0.9, 0.1, 0.1};
            var expecteds = new [] {main, ul, ur, ll, lr};
            var allTestsPass = true;

            for (var i = 0; i < expecteds.Length; i++)
            {
                // Act
                var result = pattern.ColourAtUv(uValues[i], vValues[i]);
                allTestsPass &= result == expecteds[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }
    }
}
