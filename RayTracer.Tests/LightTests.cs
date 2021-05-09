using Xunit;

namespace RayTracer.Tests
{
    public class LightTests
    {
        [Fact]
        public void APointLightHasAPositionAndIntensity()
        {
            // Arrange
            var position = new Point(0, 0, 0);
            var intensity = new Colour(1, 1, 1);

            // Act
            var light = Light.PointLight(position, intensity);

            // Assert
            Assert.Equal(position, light.Position);
            Assert.Equal(intensity, light.Intensity);
        }
    }
}
