using Xunit;

namespace RayTracer.Tests
{
    public class ColourTests
    {
        [Fact]
        public void AColourHasRedGreenBlueElements()
        {
            // Arrange
            var a = new Colour(-0.5, 0.4, 1.7);

            // Act

            // Assert
            Assert.Equal(-0.5, a.Red);
            Assert.Equal(0.4, a.Green);
            Assert.Equal(1.7, a.Blue);
        }

        [Fact]
        public void AddingTwoColours()
        {
            // Arrange
            var c1 = new Colour(0.9, 0.6, 0.75);
            var c2 = new Colour(0.7, 0.1, 0.25);
            var expected = new Colour(1.6, 0.7, 1.0);

            // Act
            var result = c1 + c2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SubtractingTwoColours()
        {
            // Arrange
            var c1 = new Colour(0.9, 0.6, 0.75);
            var c2 = new Colour(0.7, 0.1, 0.25);
            var expected = new Colour(0.2, 0.5, 0.5);

            // Act
            var result = c1 - c2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NegatingAColour()
        {
            // Arrange
            var v = new Colour(1, -2, 3);
            var expected = new Colour(-1, 2, -3);

            // Act
            var result = -v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingTwoColours()
        {
            // Arrange
            var c1 = new Colour(1, 0.2, 0.4);
            var c2 = new Colour(0.9, 1, 0.1);
            var expected = new Colour(0.9, 0.2, 0.04);

            // Act
            var result = c1 * c2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAColourByAScalar()
        {
            // Arrange
            var v = new Colour(1, -2, 3);
            var expected = new Colour(3.5, -7, 10.5);

            // Act
            var result = v * 3.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAColourByAFraction()
        {
            // Arrange
            var v = new Colour(1, -2, 3);
            var expected = new Colour(0.5, -1, 1.5);

            // Act
            var result = v * 0.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DividingAColourByAScalar()
        {
            // Arrange
            var v = new Colour(1, -2, 3);
            var expected = new Colour(0.5, -1, 1.5);

            // Act
            var result = v / 2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CloningAColour()
        {
            // Arrange
            var orig = new Colour(0.2, 0.4, 0.6);

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Red, clone.Red);
            Assert.Equal(orig.Green, clone.Green);
            Assert.Equal(orig.Blue, clone.Blue);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "[R:0.1, G:0.3, B:0.5]\n";
            var orig = new Colour(0.1, 0.3, 0.5);

            // Act
            var result = orig.ToString();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
