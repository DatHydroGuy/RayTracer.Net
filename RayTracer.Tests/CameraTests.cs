using System;
using Xunit;

namespace RayTracer.Tests
{
    public class CameraTests
    {
        [Fact]
        public void CreatingACamera()
        {
            // Arrange
            const int hSize = 160;
            const int vSize = 120;
            const double fieldOfView = Math.PI / 2.0;

            // Act
            var c = new Camera(hSize, vSize, fieldOfView);

            // Assert
            Assert.Equal(hSize, c.HSize);
            Assert.Equal(vSize, c.VSize);
            Assert.Equal(fieldOfView, c.FieldOfView);
            Assert.Equal(Matrix.Identity(4), c.Transform);
        }

        [Fact]
        public void ThePixelSizeForAHorizontalCanvas()
        {
            // Arrange
            const int hSize = 200;
            const int vSize = 125;
            const double fieldOfView = Math.PI / 2.0;

            // Act
            var c = new Camera(hSize, vSize, fieldOfView);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.01, c.PixelSize));
        }

        [Fact]
        public void ThePixelSizeForAVerticalCanvas()
        {
            // Arrange
            const int hSize = 125;
            const int vSize = 200;
            const double fieldOfView = Math.PI / 2.0;

            // Act
            var c = new Camera(hSize, vSize, fieldOfView);

            // Assert
            Assert.True(Utilities.AlmostEqual(0.01, c.PixelSize));
        }

        [Fact]
        public void ConstructingARayThroughTheCenterOfTheCanvas()
        {
            // Arrange
            var c = new Camera(201, 101, Math.PI / 2.0);

            // Act
            var r = c.RayForPixel(100, 50);

            // Assert
            Assert.Equal(new Point(0, 0, 0), r.Origin);
            Assert.Equal(new Vector(0, 0, -1), r.Direction);
        }

        [Fact]
        public void ConstructingARayThroughTheCornerOfTheCanvas()
        {
            // Arrange
            var c = new Camera(201, 101, Math.PI / 2.0);

            // Act
            var r = c.RayForPixel(0, 0);

            // Assert
            Assert.Equal(new Point(0, 0, 0), r.Origin);
            Assert.Equal(new Vector(0.66519, 0.33259, -0.66851), r.Direction);
        }

        [Fact]
        public void ConstructingARayWhenTheCameraHasBeenTransformed()
        {
            // Arrange
            var c = new Camera(201, 101, Math.PI / 2.0)
            {
                Transform = Transformations.RotationY(Math.PI / 4.0) * Transformations.Translation(0, -2, 5)
            };

            // Act
            var r = c.RayForPixel(100, 50);

            // Assert
            Assert.Equal(new Point(0, 2, -5), r.Origin);
            Assert.Equal(new Vector(0.5 * Math.Sqrt(2), 0, -0.5 * Math.Sqrt(2)), r.Direction);
        }

        [Fact]
        public void RenderingAWorldWithACamera()
        {
            // Arrange
            var w = World.DefaultWorld();
            var c = new Camera(11, 11, Math.PI / 2.0);
            var from = new Point(0, 0, -5);
            var to = new Point(0, 0, 0);
            var up = new Vector(0, 1, 0);
            c.Transform = Transformations.ViewTransform(from, to, up);

            // Act
            var image = c.Render(w);

            // Assert
            Assert.Equal(new Colour(0.38066, 0.47583, 0.2855), image.PixelAt(5, 5));
        }
    }
}
