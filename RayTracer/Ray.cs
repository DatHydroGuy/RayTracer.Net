namespace RayTracer
{
    public class Ray
    {
        private Point _origin;
        private Vector _direction;

        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

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
