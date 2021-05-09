namespace RayTracer
{
    public class Vector: Tuple
    {
        public Vector(double x = 0.0, double y = 0.0, double z = 0.0, double w = 0) : base(x, y, z, 0)
        {
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector{
                X = v1.X + v2.X,
                Y = v1.Y + v2.Y,
                Z = v1.Z + v2.Z,
                W = v1.W + v2.W
            };
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector{
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y,
                Z = v1.Z - v2.Z,
                W = v1.W - v2.W
            };
        }

        public static Vector operator -(Vector v)
        {
            return new Vector{
                X = -v.X,
                Y = -v.Y,
                Z = -v.Z
            };
        }

        public static Vector operator *(Vector v, double d)
        {
            return new Vector{
                X = v.X * d,
                Y = v.Y * d,
                Z = v.Z * d
            };
        }

        public static Vector operator *(double d, Vector v)
        {
            return new Vector{
                X = v.X * d,
                Y = v.Y * d,
                Z = v.Z * d
            };
        }

        public static Vector operator /(Vector v, double d)
        {
            return new Vector{
                X = v.X / d,
                Y = v.Y / d,
                Z = v.Z / d
            };
        }

        public Vector Normalise()
        {
            return this / Magnitude();
        }
        
        public Vector Cross(Vector v)
        {
            return new Vector{
                X = Y * v.Z - Z * v.Y,
                Y = Z * v.X - X * v.Z,
                Z = X * v.Y - Y * v.X
            };
        }
        
        public Vector Reflect(Vector normal)
        {
            return this - normal * 2 * Dot(normal);
        }
    }
}
