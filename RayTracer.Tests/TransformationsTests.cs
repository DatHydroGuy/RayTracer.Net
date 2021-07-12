using System;
using Xunit;

namespace RayTracer.Tests
{
    public class TransformationsTests
    {
        [Fact]
        public void MultiplyingByATranslationMatrix()
        {
            // Arrange
            var t = Transformations.Translation(5, -3, 2);
            var p = new Point(-3, 4, 5);
            var expected = new Point(2, 1, 7);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingByTheInverseOfATranslationMatrix()
        {
            // Arrange
            var t = Transformations.Translation(5, -3, 2);
            var inv = t.Inverse();
            var p = new Point(-3, 4, 5);
            var expected = new Point(-8, 7, 3);

            // Act
            var result = inv * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TranslationDoesNotAffectVectors()
        {
            // Arrange
            var t = Transformations.Translation(5, -3, 2);
            var v = new Vector(-3, 4, 5);

            // Act
            var result = t * v;

            // Assert
            Assert.Equal(v, result);
        }

        [Fact]
        public void AScalingMatrixAppliedToAPoint()
        {
            // Arrange
            var t = Transformations.Scaling(2, 3, 4);
            var p = new Point(-4, 6, 8);
            var expected = new Point(-8, 18, 32);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AScalingMatrixAppliedToAVector()
        {
            // Arrange
            var t = Transformations.Scaling(2, 3, 4);
            var v = new Vector(-4, 6, 8);
            var expected = new Vector(-8, 18, 32);

            // Act
            var result = t * v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheInverseOfAScalingMatrixAppliedToAVector()
        {
            // Arrange
            var t = Transformations.Scaling(2, 3, 4);
            var inv = t.Inverse();
            var v = new Vector(-4, 6, 8);
            var expected = new Vector(-2, 2, 2);

            // Act
            var result = inv * v;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReflectionIsScalingByANegativeValue()
        {
            // Arrange
            var t = Transformations.Scaling(-1, 1, 1);
            var p = new Point(2, 3, 4);
            var expected = new Point(-2, 3, 4);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RotatingAPointAroundTheXAxis()
        {
            // Arrange
            var halfQuarter = Transformations.RotationX(Math.PI / 4.0);
            var fullQuarter = Transformations.RotationX(Math.PI / 2.0);
            var p = new Point(0, 1, 0);
            var expected1 = new Point(0, Math.Sqrt(2) / 2.0, Math.Sqrt(2) / 2.0);
            var expected2 = new Point(0, 0, 1);

            // Act
            var result1 = halfQuarter * p;
            var result2 = fullQuarter * p;

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
        }

        [Fact]
        public void TheInverseOfAnXRotationRotatesInTheOppositeDirection()
        {
            // Arrange
            var halfQuarter = Transformations.RotationX(Math.PI / 4.0);
            var inv = halfQuarter.Inverse();
            var p = new Point(0, 1, 0);
            var expected = new Point(0, Math.Sqrt(2) / 2.0, -Math.Sqrt(2) / 2.0);

            // Act
            var result = inv * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RotatingAPointAroundTheYAxis()
        {
            // Arrange
            var halfQuarter = Transformations.RotationY(Math.PI / 4.0);
            var fullQuarter = Transformations.RotationY(Math.PI / 2.0);
            var p = new Point(0, 0, 1);
            var expected1 = new Point(Math.Sqrt(2) / 2.0, 0, Math.Sqrt(2) / 2.0);
            var expected2 = new Point(1, 0, 0);

            // Act
            var result1 = halfQuarter * p;
            var result2 = fullQuarter * p;

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
        }

        [Fact]
        public void TheInverseOfAYRotationRotatesInTheOppositeDirection()
        {
            // Arrange
            var halfQuarter = Transformations.RotationY(Math.PI / 4.0);
            var inv = halfQuarter.Inverse();
            var p = new Point(0, 0, 1);
            var expected = new Point(-Math.Sqrt(2) / 2.0, 0, Math.Sqrt(2) / 2.0);

            // Act
            var result = inv * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RotatingAPointAroundTheZAxis()
        {
            // Arrange
            var halfQuarter = Transformations.RotationZ(Math.PI / 4.0);
            var fullQuarter = Transformations.RotationZ(Math.PI / 2.0);
            var p = new Point(0, 1, 0);
            var expected1 = new Point(-Math.Sqrt(2) / 2.0, Math.Sqrt(2) / 2.0, 0);
            var expected2 = new Point(-1, 0, 0);

            // Act
            var result1 = halfQuarter * p;
            var result2 = fullQuarter * p;

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
        }

        [Fact]
        public void TheInverseOfAZRotationRotatesInTheOppositeDirection()
        {
            // Arrange
            var halfQuarter = Transformations.RotationZ(Math.PI / 4.0);
            var inv = halfQuarter.Inverse();
            var p = new Point(0, 1, 0);
            var expected = new Point(Math.Sqrt(2) / 2.0, Math.Sqrt(2) / 2.0, 0);

            // Act
            var result = inv * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesXInProportionToY()
        {
            // Arrange
            var t = Transformations.Shearing(1, 0, 0, 0, 0, 0);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X + p.Y, p.Y, p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesXInProportionToZ()
        {
            // Arrange
            var t = Transformations.Shearing(0, 1, 0, 0, 0, 0);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X + p.Z, p.Y, p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesYInProportionToX()
        {
            // Arrange
            var t = Transformations.Shearing(0, 0, 1, 0, 0, 0);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X, p.X + p.Y, p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesYInProportionToZ()
        {
            // Arrange
            var t = Transformations.Shearing(0, 0, 0, 1, 0, 0);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X, p.Y + p.Z, p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesZInProportionToX()
        {
            // Arrange
            var t = Transformations.Shearing(0, 0, 0, 0, 1, 0);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X, p.Y, p.X + p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AShearingTransformationMovesZInProportionToY()
        {
            // Arrange
            var t = Transformations.Shearing(0, 0, 0, 0, 0, 1);
            var p = new Point(2, 3, 4);
            var expected = new Point(p.X, p.Y, p.Y + p.Z);

            // Act
            var result = t * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IndividualTransformationsAreAppliedInSequence()
        {
            // Arrange
            var p = new Point(1, 0, 1);
            var a = Transformations.RotationX(Math.PI / 2.0);
            var b = Transformations.Scaling(5, 5, 5);
            var c = Transformations.Translation(10, 5, 7);
            var expected1 = new Point(1, -1, 0);
            var expected2 = new Point(5, -5, 0);
            var expected3 = new Point(15, 0, 7);

            // Act
            var result1 = a * p;
            var result2 = b * result1;
            var result3 = c * result2;

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
            Assert.Equal(expected3, result3);
        }

        [Fact]
        public void ChainedTransformationsMustBeAppliedInReverseOrder()
        {
            // Arrange
            var p = new Point(1, 0, 1);
            var a = Transformations.RotationX(Math.PI / 2.0);
            var b = Transformations.Scaling(5, 5, 5);
            var c = Transformations.Translation(10, 5, 7);
            var expected = new Point(15, 0, 7);

            // Act
            var result = c * b * a * p;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheTransformationMatrixForTheDefaultOrientation()
        {
            // Arrange
            var from = new Point(0, 0, 0);
            var to = new Point(0, 0, -1);
            var up = new Vector(0, 1, 0);

            // Act
            var result = Transformations.ViewTransform(from, to, up);

            // Assert
            Assert.Equal(Matrix.Identity(4), result);
        }

        [Fact]
        public void TheViewTransformationMatrixLookingInThePositiveZDirection()
        {
            // Arrange
            var from = new Point(0, 0, 0);
            var to = new Point(0, 0, 1);
            var up = new Vector(0, 1, 0);

            // Act
            var result = Transformations.ViewTransform(from, to, up);

            // Assert
            Assert.Equal(Transformations.Scaling(-1, 1, -1), result);
        }

        [Fact]
        public void TheViewTransformationMatrixMovesTheWorldAndNotTheEye()
        {
            // Arrange
            var from = new Point(0, 0, 8);
            var to = new Point(0, 0, 0);
            var up = new Vector(0, 1, 0);

            // Act
            var result = Transformations.ViewTransform(from, to, up);

            // Assert
            Assert.Equal(Transformations.Translation(0, 0, -8), result);
        }

        [Fact]
        public void AnArbitraryViewTransformationMatrix()
        {
            // Arrange
            var from = new Point(1, 3, 2);
            var to = new Point(4, -2, 8);
            var up = new Vector(1, 1, 0);
            var expected = new Matrix(-0.50709, 0.50709, 0.67612, -2.36643, 0.76772, 0.60609, 0.12122, -2.82843, -0.35857, 0.59761, -0.71714, 0.00000, 0.00000, 0.00000, 0.00000, 1.00000);

            // Act
            var result = Transformations.ViewTransform(from, to, up);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CloningATransformation()
        {
            // Arrange
            var orig = Transformations.Translation(1.2, 2.3, 3.4) * Transformations.Scaling(0.9, 0.8, 0.7) * Transformations.Shearing(9.8, 8.7, 7.6, 6.5, 5.4, 4.3) * Transformations.RotationX(-11.3);

            // Act
            var result = orig.Clone();

            // Assert
            var allEqual = true;
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    allEqual &= Utilities.AlmostEqual(orig.Data[y, x], result.Data[y, x]);
                }
            }
            Assert.True(allEqual);
        }
    }
}
