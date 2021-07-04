using System;

namespace RayTracer
{
    public abstract class Tuple
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double W { get; set; }

        protected Tuple(double x = 0.0, double y = 0.0, double z = 0.0, double w = 0.0)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Tuple;

            return other != null &&
                Utilities.AlmostEqual(X, other.X) &&
                Utilities.AlmostEqual(Y, other.Y) &&
                Utilities.AlmostEqual(Z, other.Z) &&
                Utilities.AlmostEqual(W, other.W);
        }

        public static bool operator==(Tuple t1, Tuple t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            return t1?.Equals(t2) ?? ReferenceEquals(t2, null);
        }

        public static bool operator!=(Tuple t1, Tuple t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(X * 7 + Y * 5 + Z * 3 + W * 2);
        }

        public override string ToString()
        {
            return $"[X:{X}, Y:{Y}, Z:{Z}, W:{W}]\n";
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public double Dot(Tuple t)
        {
            return X * t.X + Y * t.Y + Z * t.Z + W * t.W;
        }
    }
}
