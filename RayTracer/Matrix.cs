namespace RayTracer
{
    public class Matrix
    {
        private int _width;
        private int _height;
        private double[,] _data;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public double[,] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Matrix(int size = 4): this(size, size)
        {}

        public Matrix(int width = 4, int height = 4)
        {
            Width = width;
            Height = height;
            Data = new double[height, width];
        }

        public Matrix(double m00, double m01, double m10, double m11): this(2)
        {
            Data[0, 0] = m00;
            Data[0, 1] = m01;
            Data[1, 0] = m10;
            Data[1, 1] = m11;
        }

        public Matrix(double m00, double m01, double m02,
                      double m10, double m11, double m12,
                      double m20, double m21, double m22): this(3)
        {
            Data[0, 0] = m00;
            Data[0, 1] = m01;
            Data[0, 2] = m02;
            Data[1, 0] = m10;
            Data[1, 1] = m11;
            Data[1, 2] = m12;
            Data[2, 0] = m20;
            Data[2, 1] = m21;
            Data[2, 2] = m22;
        }

        public Matrix(double m00, double m01, double m02, double m03,
                      double m10, double m11, double m12, double m13,
                      double m20, double m21, double m22, double m23,
                      double m30, double m31, double m32, double m33): this(4)
        {
            Data[0, 0] = m00;
            Data[0, 1] = m01;
            Data[0, 2] = m02;
            Data[0, 3] = m03;
            Data[1, 0] = m10;
            Data[1, 1] = m11;
            Data[1, 2] = m12;
            Data[1, 3] = m13;
            Data[2, 0] = m20;
            Data[2, 1] = m21;
            Data[2, 2] = m22;
            Data[2, 3] = m23;
            Data[3, 0] = m30;
            Data[3, 1] = m31;
            Data[3, 2] = m32;
            Data[3, 3] = m33;
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as Matrix;

            bool areEqual = other.Data != null;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    areEqual &= Utilities.AlmostEqual(Data[y, x], other.Data[y, x]);
                }
            }

            return areEqual;
        }

        public static bool operator==(Matrix t1, Matrix t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            if(ReferenceEquals(t1, null))
            {
                return ReferenceEquals(t2, null);
            }

            if(ReferenceEquals(t2, null))
            {
                return false;
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(Matrix t1, Matrix t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            var hashValue = 0.0;
            var sixteenPrimes = new int[] {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53};

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var primeIndex = y * Height + x;
                    hashValue += Data[y, x] * sixteenPrimes[primeIndex];
                }
            }

            return (int)hashValue;
        }

        public override string ToString()
        {
            return $"[[{Data[0, 0]}, {Data[0, 1]}, {Data[0, 2]}, {Data[0, 3]},\n" +
                   $"{Data[1, 0]}, {Data[1, 1]}, {Data[1, 2]}, {Data[1, 3]},\n" +
                   $"{Data[2, 0]}, {Data[2, 1]}, {Data[2, 2]}, {Data[2, 3]},\n" +
                   $"{Data[3, 0]}, {Data[3, 1]}, {Data[3, 2]}, {Data[3, 3]}]]\n";
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Width != m2.Height)
            {
                return null;
            }

            var result = new Matrix(m2.Width, m1.Height);
            for (int y = 0; y < m1.Height; y++)
            {
                for (int x = 0; x < m2.Width; x++)
                {
                    for (int idx = 0; idx < m1.Width; idx++)
                    {
                        result.Data[y, x] += m1.Data[y, idx] * m2.Data[idx, x];
                    }
                }
            }

            return result;
        }

        public static Point operator *(Matrix m, Point p)
        {
            if (m.Width != 4)
            {
                return null;
            }

            return new Point()
            {
                X = m.Data[0, 0] * p.X + m.Data[0, 1] * p.Y + m.Data[0, 2] * p.Z + m.Data[0, 3] * p.W,
                Y = m.Data[1, 0] * p.X + m.Data[1, 1] * p.Y + m.Data[1, 2] * p.Z + m.Data[1, 3] * p.W,
                Z = m.Data[2, 0] * p.X + m.Data[2, 1] * p.Y + m.Data[2, 2] * p.Z + m.Data[2, 3] * p.W,
                W = m.Data[3, 0] * p.X + m.Data[3, 1] * p.Y + m.Data[3, 2] * p.Z + m.Data[3, 3] * p.W
            };
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            if (m.Width != 4)
            {
                return null;
            }

            return new Vector()
            {
                X = m.Data[0, 0] * v.X + m.Data[0, 1] * v.Y + m.Data[0, 2] * v.Z + m.Data[0, 3] * v.W,
                Y = m.Data[1, 0] * v.X + m.Data[1, 1] * v.Y + m.Data[1, 2] * v.Z + m.Data[1, 3] * v.W,
                Z = m.Data[2, 0] * v.X + m.Data[2, 1] * v.Y + m.Data[2, 2] * v.Z + m.Data[2, 3] * v.W,
                W = m.Data[3, 0] * v.X + m.Data[3, 1] * v.Y + m.Data[3, 2] * v.Z + m.Data[3, 3] * v.W
            };
        }

        public static Matrix Identity(int size)
        {
            var m = new Matrix(size);
            for (int i = 0; i < size; i++)
            {
                m.Data[i, i] = 1;
            }
            return m;
        }

        public Matrix Clone()
        {
            return new Matrix(
                Data[0, 0], Data[0, 1], Data[0, 2], Data[0, 3],
                Data[1, 0], Data[1, 1], Data[1, 2], Data[1, 3],
                Data[2, 0], Data[2, 1], Data[2, 2], Data[2, 3],
                Data[3, 0], Data[3, 1], Data[3, 2], Data[3, 3]
            );
        }

        public Matrix Transpose()
        {
            var m = new Matrix(Height, Width);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    m.Data[x, y] = Data[y, x];
                }
            }
            return m;
        }

        public double Determinant()
        {
            if (Width != Height)
            {
                return 0;   // No solution
            }

            if (Width == 2)
            {
                return Data[0, 0] * Data[1, 1] - Data[0, 1] * Data[1, 0];
            }
            else
            {
                var result = 0.0;
                for (int i = 0; i < Width; i++)
                {
                    result += Data[0, i] * Cofactor(0, i);
                }
                return result;
            }
        }

        public Matrix Submatrix(int row, int column)
        {
            var result = new Matrix(Height - 1, Width - 1);
            for (int y = 0; y < Height; y++)
            {
                if (y != row)
                {
                    var targetY = y < row ? y : y - 1;
                    for (int x = 0; x < Width; x++)
                    {
                        if (x != column)
                        {
                            var targetX = x <= column ? x : x - 1;
                            result.Data[targetY, targetX] = Data[y, x];
                        }
                    }
                }
            }

            return result;
        }

        public double Minor(int row, int column)
        {
            return Submatrix(row, column).Determinant();
        }

        public double Cofactor(int row, int column)
        {
            var minor = Minor(row, column);
            var result = (row + column) % 2 == 1 ? -minor : minor;
            return result;
        }

        public Matrix Inverse()
        {
            var determinant = Determinant();

            if (Utilities.AlmostEqual(determinant, 0))
            {
                return null;
            }

            var result = new Matrix(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var c = Cofactor(y, x);
                    result.Data[x, y] = c / determinant;
                }
            }

            return result;
        }
    }
}
