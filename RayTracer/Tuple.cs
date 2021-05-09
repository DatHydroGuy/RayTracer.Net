using System;

namespace RayTracer
{
    public abstract class Tuple
    {
        private double _x;
        private double _y;
        private double _z;
        private double _w;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }
        public double W
        {
            get { return _w; }
            set { _w = value; }
        }

        public Tuple(double x = 0.0, double y = 0.0, double z = 0.0, double w = 0.0)
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
            if(object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(Tuple t1, Tuple t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(X * 7 + Y * 5 + Z * 3 + W * 2);
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
