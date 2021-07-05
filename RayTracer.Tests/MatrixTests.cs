using Xunit;

namespace RayTracer.Tests
{
    public class MatrixTests
    {
        [Fact]
        public void ConstructingAndInspectingA4X4Matrix()
        {
            // Arrange
            var m = new Matrix(1, 2, 3, 4, 5.5, 6.5, 7.5, 8.5, 9, 10, 11, 12, 13.5, 14.5, 15.5, 16.5);

            // Act

            // Assert
            Assert.Equal(1, m.Data[0, 0]);
            Assert.Equal(4, m.Data[0, 3]);
            Assert.Equal(5.5, m.Data[1, 0]);
            Assert.Equal(7.5, m.Data[1, 2]);
            Assert.Equal(11, m.Data[2, 2]);
            Assert.Equal(13.5, m.Data[3, 0]);
            Assert.Equal(15.5, m.Data[3, 2]);
        }

        [Fact]
        public void ConstructingAndInspectingA3X3Matrix()
        {
            // Arrange
            var m = new Matrix(-3, 5, 0, 1, -2, -7, 0, 1, 1);

            // Act

            // Assert
            Assert.Equal(-3, m.Data[0, 0]);
            Assert.Equal(-2, m.Data[1, 1]);
            Assert.Equal(1, m.Data[2, 2]);
        }

        [Fact]
        public void ConstructingAndInspectingA2X2Matrix()
        {
            // Arrange
            var m = new Matrix(-3, 5, 1, -2);

            // Act

            // Assert
            Assert.Equal(-3, m.Data[0, 0]);
            Assert.Equal(5, m.Data[0, 1]);
            Assert.Equal(1, m.Data[1, 0]);
            Assert.Equal(-2, m.Data[1, 1]);
        }

        [Fact]
        public void MatrixEqualityWithIdenticalMatrices()
        {
            // Arrange
            var a = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2);
            var b = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2);

            // Act

            // Assert
            Assert.Equal(a, b);
        }

        [Fact]
        public void MatrixInequalityWithDifferentMatrices()
        {
            // Arrange
            var a = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2);
            var b = new Matrix(2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2, 1);

            // Act

            // Assert
            Assert.NotEqual(a, b);
        }

        [Fact]
        public void MultiplyingTwoMatrices()
        {
            // Arrange
            var a = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2);
            var b = new Matrix(-2, 1, 2, 3, 3, 2, 1, -1, 4, 3, 6, 5, 1, 2, 7, 8);
            var expected = new Matrix(20, 22, 50, 48, 44, 54, 114, 108, 40, 58, 110, 102, 16, 26, 46, 42);

            // Act
            var result = a * b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAMatrixByAPoint()
        {
            // Arrange
            var a = new Matrix(1, 2, 3, 4, 2, 4, 4, 2, 8, 6, 4, 1, 0, 0, 0, 1);
            var b = new Point(1, 2, 3);
            var expected = new Point(18, 24, 33);

            // Act
            var result = a * b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAMatrixByAVector()
        {
            // Arrange
            var a = new Matrix(1, 2, 3, 4, 2, 4, 4, 2, 8, 6, 4, 1, 0, 0, 0, 1);
            var b = new Vector(1, 2, 3);
            var expected = new Vector(14, 22, 32);

            // Act
            var result = a * b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreatingA4X4IdentityMatrix()
        {
            // Arrange
            var expected = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            // Act
            var result = Matrix.Identity(4);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreatingA3X3IdentityMatrix()
        {
            // Arrange
            var expected = new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1);

            // Act
            var result = Matrix.Identity(3);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreatingA2X2IdentityMatrix()
        {
            // Arrange
            var expected = new Matrix(1, 0, 0, 1);

            // Act
            var result = Matrix.Identity(2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAMatrixByTheIdentityMatrix()
        {
            // Arrange
            var a = new Matrix(0, 1, 2, 4, 1, 2, 4, 8, 2, 4, 8, 16, 4, 8, 16, 32);

            // Act
            var result = a * Matrix.Identity(4);

            // Assert
            Assert.Equal(a, result);
        }

        [Fact]
        public void MultiplyingTheIdentityMatrixByATuple()
        {
            // Arrange
            var a = new Point(1, 2, 3)
            {
                W = 4
            };

            // Act
            var result = Matrix.Identity(4) * a;

            // Assert
            Assert.Equal(a, result);
        }

        [Fact]
        public void TransposingASquareMatrix()
        {
            // Arrange
            var a = new Matrix(0, 9, 3, 0, 9, 8, 0, 8, 1, 8, 5, 3, 0, 0, 5, 8);
            var expected = new Matrix(0, 9, 1, 0, 9, 8, 8, 0, 3, 0, 5, 5, 0, 8, 3, 8);

            // Act
            var result = a.Transpose();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TransposingTheIdentityMatrix()
        {
            // Arrange
            var a = Matrix.Identity(4);

            // Act
            var result = a.Transpose();

            // Assert
            Assert.Equal(a, result);
        }

        [Fact]
        public void CalculatingTheDeterminantOfA2X2Matrix()
        {
            // Arrange
            var a = new Matrix(1, 5, -3, 2);
            const int expected = 17;

            // Act
            var result = a.Determinant();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ASubmatrixOfA3X3MatrixIsA2X2Matrix()
        {
            // Arrange
            var a = new Matrix(1, 5, 0, -3, 2, 7, 0, 6, -3);
            var expected = new Matrix(-3, 2, 0, 6);

            // Act
            var result = a.Submatrix(0, 2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ASubmatrixOfA4X4MatrixIsA3X3Matrix()
        {
            // Arrange
            var a = new Matrix(-6, 1, 1, 6, -8, 5, 8, 6, -1, 0, 8, 2, -7, 1, -1, 1);
            var expected = new Matrix(-6, 1, 6, -8, 8, 6, -7, -1, 1);

            // Act
            var result = a.Submatrix(2, 1);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateTheMinorOfA3X3Matrix()
        {
            // Arrange
            var a = new Matrix(3, 5, 0, 2, -1, -7, 6, -1, 5);
            const int expected = 25;

            // Act
            var result = a.Minor(1, 0);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateTheCofactorOfA3X3Matrix()
        {
            // Arrange
            var a = new Matrix(3, 5, 0, 2, -1, -7, 6, -1, 5);
            const int minor1 = -12;
            const int cofactor1 = -12;
            const int minor2 = 25;
            const int cofactor2 = -25;

            // Act
            var result1 = a.Minor(0, 0);
            var result2 = a.Cofactor(0, 0);
            var result3 = a.Minor(1, 0);
            var result4 = a.Cofactor(1, 0);

            // Assert
            Assert.Equal(minor1, result1);
            Assert.Equal(cofactor1, result2);
            Assert.Equal(minor2, result3);
            Assert.Equal(cofactor2, result4);
        }

        [Fact]
        public void CalculateTheDeterminantOfA3X3Matrix()
        {
            // Arrange
            var a = new Matrix(1, 2, 6, -5, 8, -4, 2, 6, 4);
            const int cofactor1 = 56;
            const int cofactor2 = 12;
            const int cofactor3 = -46;
            const int determinant = -196;

            // Act
            var result1 = a.Cofactor(0, 0);
            var result2 = a.Cofactor(0, 1);
            var result3 = a.Cofactor(0, 2);
            var result4 = a.Determinant();

            // Assert
            Assert.Equal(cofactor1, result1);
            Assert.Equal(cofactor2, result2);
            Assert.Equal(cofactor3, result3);
            Assert.Equal(determinant, result4);
        }

        [Fact]
        public void CalculateTheDeterminantOfA4X4Matrix()
        {
            // Arrange
            var a = new Matrix(-2, -8, 3, 5, -3, 1, 7, 3, 1, 2, -9, 6, -6, 7, 7, -9);
            const int cofactor1 = 690;
            const int cofactor2 = 447;
            const int cofactor3 = 210;
            const int cofactor4 = 51;
            const int determinant = -4071;

            // Act
            var result1 = a.Cofactor(0, 0);
            var result2 = a.Cofactor(0, 1);
            var result3 = a.Cofactor(0, 2);
            var result4 = a.Cofactor(0, 3);
            var result5 = a.Determinant();

            // Assert
            Assert.Equal(cofactor1, result1);
            Assert.Equal(cofactor2, result2);
            Assert.Equal(cofactor3, result3);
            Assert.Equal(cofactor4, result4);
            Assert.Equal(determinant, result5);
        }

        [Fact]
        public void TestingAnInvertibleMatrixForInvertibility()
        {
            // Arrange
            var a = new Matrix(6, 4, 4, 4, 5, 5, 7, 6, 4, -9, 3, -7, 9, 1, 7, -6);
            const int expDet = -2120;

            // Act
            var resDet = a.Determinant();
            var resInv = a.Inverse();

            // Assert
            Assert.Equal(expDet, resDet);
            Assert.NotNull(resInv);
        }

        [Fact]
        public void TestingANoninvertibleMatrixForInvertibility()
        {
            // Arrange
            var a = new Matrix(-4, 2, -2, -3, 9, 6, 2, 6, 0, -5, 1, -5, 0, 0, 0, 0);
            const int expDet = 0;

            // Act
            var resDet = a.Determinant();
            var resInv = a.Inverse();

            // Assert
            Assert.Equal(expDet, resDet);
            Assert.Null(resInv);
        }

        [Fact]
        public void CalculateTheInverseOfAnInvertibleMatrix()
        {
            // Arrange
            var a = new Matrix(-5, 2, 6, -8, 1, -5, 1, 8, 7, 7, -6, -7, 1, -3, 7, 4);
            const int expected1 = 532;
            const int expected2 = -160;
            const double expected3 = -160.0 / 532.0;
            const int expected4 = 105;
            const double expected5 = 105.0 / 532.0;
            var expected6 = new Matrix(0.21805, 0.45113, 0.24060, -0.04511, -0.80827, -1.45677, -0.44361, 0.52068, -0.07895, -0.22368, -0.05263, 0.19737, -0.52256, -0.81391, -0.30075, 0.30639);

            // Act
            var b = a.Inverse();
            var result1 = a.Determinant();
            var result2 = a.Cofactor(2, 3);
            var result3 = b.Data[3, 2];
            var result4 = a.Cofactor(3, 2);
            var result5 = b.Data[2, 3];

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
            Assert.Equal(expected3, result3);
            Assert.Equal(expected4, result4);
            Assert.Equal(expected5, result5);
            Assert.Equal(expected6, b);
        }

        [Fact]
        public void CalculateTheInverseOfASecondMatrix()
        {
            // Arrange
            var a = new Matrix(8, -5, 9, 2, 7, 5, 6, 1, -6, 0, 9, 6, -3, 0, -9, -4);
            var expected = new Matrix(-0.15385, -0.15385, -0.28205, -0.53846, -0.07692, 0.12308, 0.02564, 0.03077, 0.35897, 0.35897, 0.43590, 0.92308, -0.69231, -0.69231, -0.76923, -1.92308);

            // Act
            var result = a.Inverse();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateTheInverseOfAThirdMatrix()
        {
            // Arrange
            var a = new Matrix(9, 3, 0, 9, -5, -2, -6, -3, -4, 9, 6, 4, -7, 6, 6, 2);
            var expected = new Matrix(-0.04074, -0.07778, 0.14444, -0.22222, -0.07778, 0.03333, 0.36667, -0.33333, -0.02901, -0.14630, -0.10926, 0.12963, 0.17778, 0.06667, -0.26667, 0.33333);

            // Act
            var result = a.Inverse();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAMatrixByItsInverse()
        {
            // Arrange
            var a = new Matrix(9, 3, 0, 9, -5, -2, -6, -3, -4, 9, 6, 4, -7, 6, 6, 2);
            var expected = Matrix.Identity(4);

            // Act
            var result = a * a.Inverse();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MultiplyingAMatrixProductByTheInverseOfTheSecondMatrixRestoresTheFirstMatrix()
        {
            // Arrange
            var a = new Matrix(3, -9, 7, 3, 3, -8, 2, -9, -4, 4, 4, 1, -6, 5, -1, 1);
            var b = new Matrix(8, 2, 2, 2, 3, -1, 7, 0, 7, 0, 5, 4, 6, -2, 0, 5);

            // Act
            var c = a * b;
            var result = c * b.Inverse();

            // Assert
            Assert.Equal(a, result);
        }

        [Fact]
        public void InvertingTheIdentityMatrix()
        {
            // Arrange
            var a = Matrix.Identity(4);
            var expected = Matrix.Identity(4);

            // Act
            var result = a.Inverse();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void InverseOfTheTransposeEqualsTheTransposeOfTheInverse()
        {
            // Arrange
            var a = new Matrix(3, -9, 7, 3, 3, -8, 2, -9, -4, 4, 4, 1, -6, 5, -1, 1);
            var b = a.Inverse().Transpose();
            var c = a.Transpose().Inverse();

            // Act

            // Assert
            Assert.Equal(b, c);
        }
    }
}
