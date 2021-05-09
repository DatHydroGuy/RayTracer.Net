using System;
using Xunit;

namespace RayTracer.Tests
{
    public class MaterialTests
    {
        [Fact]
        public void TheDefaultMaterial()
        {
            // Arrange
            var m = new Material();

            // Act

            // Assert
            Assert.Equal(new Colour(1, 1, 1), m.Colour);
            Assert.Equal(0.1, m.Ambient);
            Assert.Equal(0.9, m.Diffuse);
            Assert.Equal(0.9, m.Specular);
            Assert.Equal(200, m.Shininess);
        }

        [Fact]
        public void LightingWithTheEyeBetweenTheLightAndTheSurface()
        {
            // Arrange
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, 0, -1);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 0, -10), new Colour(1, 1, 1));

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector);

            // Assert
            Assert.Equal(new Colour(1.9, 1.9, 1.9), result);
        }

        [Fact]
        public void LightingWithTheEyeBetweenTheLightAndTheSurfaceButOffsetAt45Degrees()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) * 0.5;
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, halfSqrt2, -halfSqrt2);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 0, -10), new Colour(1, 1, 1));

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector);

            // Assert
            Assert.Equal(new Colour(1, 1, 1), result);
        }

        [Fact]
        public void LightingWithTheEyeOppositeTheSurfaceWithLightOffsetAt45Degrees()
        {
            // Arrange
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, 0, -1);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 10, -10), new Colour(1, 1, 1));

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector);

            // Assert
            Assert.Equal(new Colour(0.7364, 0.7364, 0.7364), result);
        }

        [Fact]
        public void LightingWithTheEyeInThePathOfTheLightReflectionVector()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) * 0.5;
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, -halfSqrt2, -halfSqrt2);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 10, -10), new Colour(1, 1, 1));

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector);

            // Assert
            Assert.Equal(new Colour(1.6364, 1.6364, 1.6364), result);
        }

        [Fact]
        public void LightingWithTheLightBehindTheSurface()
        {
            // Arrange
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, 0, -1);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 0, 10), new Colour(1, 1, 1));

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector);

            // Assert
            Assert.Equal(new Colour(0.1, 0.1, 0.1), result);
        }

        [Fact]
        public void LightingWithTheSurfaceInShadow()
        {
            // Arrange
            var m = new Material();
            var targetPoint = new Point(0, 0, 0);
            var eyeVector = new Vector(0, 0, -1);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 0, -10), new Colour(1, 1, 1));
            bool isInShadow = true;

            // Act
            var result = m.Lighting(new Sphere(), light, targetPoint, eyeVector, normalVector, isInShadow);

            // Assert
            Assert.Equal(new Colour(0.1, 0.1, 0.1), result);
        }

        [Fact]
        public void LightingWithAStripedPatternApplied()
        {
            // Arrange
            var m = new Material();
            m.Pattern = new StripePattern(Colour.WHITE, Colour.BLACK);
            m.Ambient = 1;
            m.Diffuse = 0;
            m.Specular = 0;
            var eyeVector = new Vector(0, 0, -1);
            var normalVector = new Vector(0, 0, -1);
            var light = Light.PointLight(new Point(0, 0, -10), new Colour(1, 1, 1));

            // Act
            var result1 = m.Lighting(new Sphere(), light, new Point(0.9, 0, 0), eyeVector, normalVector, false);
            var result2 = m.Lighting(new Sphere(), light, new Point(1.1, 0, 0), eyeVector, normalVector, false);

            // Assert
            Assert.Equal(Colour.WHITE, result1);
            Assert.Equal(Colour.BLACK, result2);
        }

        [Fact]
        public void DefaultMaterialHasAReflectivity()
        {
            // Arrange
            var m = new Material();

            // Act

            // Assert
            Assert.Equal(0.0, m.Reflective);
        }

        [Fact]
        public void DefaultMaterialHasATransparency()
        {
            // Arrange
            var m = new Material();

            // Act

            // Assert
            Assert.Equal(0.0, m.Transparency);
        }

        [Fact]
        public void DefaultMaterialHasARefractiveIndex()
        {
            // Arrange
            var m = new Material();

            // Act

            // Assert
            Assert.Equal(1.0, m.RefractiveIndex);
        }
    }
}
