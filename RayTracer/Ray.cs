namespace RayTracer
{
    public class Ray
    {
        public Point Origin { get; }

        public Vector Direction { get; }

        public Ray(Point origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Point Position(double t)
        {
            return Origin + t * Direction;
        }

        public Ray Transform(Matrix m)
        {
            return new Ray(m * Origin, m * Direction);
        }
    }
}
