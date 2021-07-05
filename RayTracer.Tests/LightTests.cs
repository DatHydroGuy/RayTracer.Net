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

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            const string expected = "Light:[Position:[X:9.8, Y:7.6, Z:5.4, W:1]\nIntensity:[R:0.1, G:0.3, B:0.5]\n]\n";
            var orig = new Light(new Point(9.8, 7.6, 5.4), new Colour(0.1, 0.3, 0.5));

            // Act
            var result = orig.ToString();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
