namespace RayTracer
{
    public class Point: Tuple
    {
        public Point(double x = 0.0, double y = 0.0, double z = 0.0, double w = 1) : base(x, y, z, 1)
        {
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point{
                X = p1.X + p2.X,
                Y = p1.Y + p2.Y,
                Z = p1.Z + p2.Z,
                W = p1.W + p2.W
            };
        }

        public static Point operator +(Point p1, Vector v1)
        {
            return new Point{
                X = p1.X + v1.X,
                Y = p1.Y + v1.Y,
                Z = p1.Z + v1.Z,
                W = p1.W + v1.W
            };
        }

        public static Point operator -(Point p1, Vector v1)
        {
            return new Point{
                X = p1.X - v1.X,
                Y = p1.Y - v1.Y,
                Z = p1.Z - v1.Z,
                W = p1.W - v1.W
            };
        }

        public static Vector operator -(Point p1, Point p2)
        {
            return new Vector{
                X = p1.X - p2.X,
                Y = p1.Y - p2.Y,
                Z = p1.Z - p2.Z,
                W = p1.W - p2.W
            };
        }

        public static Point operator -(Point p)
        {
            return new Point{
                X = -p.X,
                Y = -p.Y,
                Z = -p.Z
            };
        }

        public static Point operator *(Point p1, double d)
        {
            return new Point{
                X = p1.X * d,
                Y = p1.Y * d,
                Z = p1.Z * d
            };
        }

        public static Point operator *(double d, Point p1)
        {
            return new Point{
                X = p1.X * d,
                Y = p1.Y * d,
                Z = p1.Z * d
            };
        }

        public static Point operator /(Point p1, double d)
        {
            return new Point{
                X = p1.X / d,
                Y = p1.Y / d,
                Z = p1.Z / d
            };
        }
    }
}
