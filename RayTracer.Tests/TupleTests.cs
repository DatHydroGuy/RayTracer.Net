using System;
using Xunit;

namespace RayTracer.Tests
{
    public class TupleTests
    {
        [Fact]
        public void APointIsATupleWithWEqualTo1()
        {
            // Arrange
            var a = new Point(4.3, -4.2, 3.1);

            // Act

            // Assert
            Assert.IsAssignableFrom<Tuple>(a);
            Assert.Equal(4.3, a.X);
            Assert.Equal(-4.2, a.Y);
            Assert.Equal(3.1, a.Z);
            Assert.Equal(1.0, a.W);
        }

        [Fact]
        public void AVectorIsATupleWithWEqualTo0()
        {
            // Arrange
            var a = new Vector(4.3, -4.2, 3.1);

            // Act

            // Assert
            Assert.IsAssignableFrom<Tuple>(a);
            Assert.Equal(4.3, a.X);
            Assert.Equal(-4.2, a.Y);
            Assert.Equal(3.1, a.Z);
            Assert.Equal(0.0, a.W);
        }

        [Fact]
        public void AddingAVectorAndAPoint()
        {
            // Arrange
            var p = new Point(3, -2, 5);
            var v = new Vector(-2, 3, 1);
            var expected = new Point(1, 1, 6);

            // Act
            var result = p + v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SubtractingTwoPoints()
        {
            // Arrange
            var p1 = new Point(3, 2, 1);
            var p2 = new Point(5, 6, 7);
            var expected = new Vector(-2, -4, -6);

            // Act
            var result = p1 - p2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SubtractingAVectorFromAPoint()
        {
            // Arrange
            var p = new Point(3, 2, 1);
            var v = new Vector(5, 6, 7);
            var expected = new Point(-2, -4, -6);

            // Act
            var result = p - v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SubtractingTwoVectors()
        {
            // Arrange
            var v1 = new Vector(3, 2, 1);
            var v2 = new Vector(5, 6, 7);
            var expected = new Vector(-2, -4, -6);

            // Act
            var result = v1 - v2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SubtractingAVectorFromTheZeroVector()
        {
            // Arrange
            var v1 = new Vector(0, 0, 0);
            var v2 = new Vector(1, -2, 3);
            var expected = new Vector(-1, 2, -3);

            // Act
            var result = v1 - v2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NegatingAVector()
        {
            // Arrange
            var v = new Vector(1, -2, 3);
            var expected = new Vector(-1, 2, -3);

            // Act
            var result = -v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NegatingAPoint()
        {
            // Arrange
            var p = new Point(1, -2, 3);
            var expected = new Point(-1, 2, -3);

            // Act
            var result = -p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAVectorByAScalar()
        {
            // Arrange
            var v = new Vector(1, -2, 3);
            var expected = new Vector(3.5, -7, 10.5);

            // Act
            var result = v * 3.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAPointByAScalar()
        {
            // Arrange
            var p = new Point(1, -2, 3);
            var expected = new Point(3.5, -7, 10.5);

            // Act
            var result = p * 3.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAVectorByAFraction()
        {
            // Arrange
            var v = new Vector(1, -2, 3);
            var expected = new Vector(0.5, -1, 1.5);

            // Act
            var result = v * 0.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAPointByAFraction()
        {
            // Arrange
            var p = new Point(1, -2, 3);
            var expected = new Point(0.5, -1, 1.5);

            // Act
            var result = p * 0.5;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DividingAVectorByAScalar()
        {
            // Arrange
            var v = new Vector(1, -2, 3);
            var expected = new Vector(0.5, -1, 1.5);

            // Act
            var result = v / 2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DividingAPointByAScalar()
        {
            // Arrange
            var p = new Point(1, -2, 3);
            var expected = new Point(0.5, -1, 1.5);

            // Act
            var result = p / 2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 0, 0, 1)]
        [InlineData(0, 1, 0, 1)]
        [InlineData(0, 0, 1, 1)]
        [InlineData(1, 2, 3, 14)]
        [InlineData(-1, -2, -3, 14)]
        public void ComputingTheMagnitudeOfAVector(double vx, double vy, double vz, double expectedSquared)
        {
            // Arrange
            var v = new Vector(vx, vy, vz);
            double expected = Math.Sqrt(expectedSquared);

            // Act
            var result = v.Magnitude();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NormalisingAnAxisAlignedVector()
        {
            // Arrange
            var v = new Vector(4, 0, 0);
            var expected = new Vector(1, 0, 0);

            // Act
            var result = v.Normalise();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NormalisingAGeneralVector()
        {
            // Arrange
            var v = new Vector(1, 2, 3);
            double sqrt14 = Math.Sqrt(14);
            var expected = new Vector(1 / sqrt14, 2 / sqrt14, 3 / sqrt14);

            // Act
            var result = v.Normalise();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MagnitudeOfANormalisedVector()
        {
            // Arrange
            var v = new Vector(1, 2, 3);
            var expected = 1;

            // Act
            var result = v.Normalise().Magnitude();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DotProductOfTwoTuples()
        {
            // Arrange
            var v1 = new Vector(1, 2, 3);
            var v2 = new Vector(2, 3, 4);
            var expected = 20;

            // Act
            var result = v1.Dot(v2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CrossProductOfTwoVectors()
        {
            // Arrange
            var v1 = new Vector(1, 2, 3);
            var v2 = new Vector(2, 3, 4);
            var v1CrossV2 = new Vector(-1, 2, -1);
            var v2CrossV1 = new Vector(1, -2, 1);

            // Act
            var result1 = v1.Cross(v2);
            var result2 = v2.Cross(v1);

            // Assert
            Assert.Equal(v1CrossV2, result1);
            Assert.Equal(v2CrossV1, result2);
        }

        [Fact]
        public void ReflectingAVectorApproachingAt45Degrees()
        {
            // Arrange
            var v = new Vector(1, -1, 0);
            var n = new Vector(0, 1, 0);
            var expected = new Vector(1, 1, 0);

            // Act
            var result = v.Reflect(n);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReflectingAVectorOffASlantedSurface()
        {
            // Arrange
            var halfSqrtTwo = Math.Sqrt(2) / 2.0;
            var v = new Vector(0, -1, 0);
            var n = new Vector(halfSqrtTwo, halfSqrtTwo, 0);
            var expected = new Vector(1, 0, 0);

            // Act
            var result = v.Reflect(n);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CloningAPoint()
        {
            // Arrange
            var orig = new Point(1.2, 2.3, 3.4);

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.X, clone.X);
            Assert.Equal(orig.Y, clone.Y);
            Assert.Equal(orig.Z, clone.Z);
            Assert.Equal(orig.W, clone.W);
        }

        [Fact]
        public void CloningAVector()
        {
            // Arrange
            var orig = new Vector(1.2, 2.3, 3.4);

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.X, clone.X);
            Assert.Equal(orig.Y, clone.Y);
            Assert.Equal(orig.Z, clone.Z);
            Assert.Equal(orig.W, clone.W);
        }
    }
}
