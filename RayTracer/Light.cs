namespace RayTracer
{
    public class Light
    {
        private Point _position;
        private Colour _intensity;

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Colour Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public Light(Point position, Colour intensity)
        {
            Position = position;
            Intensity = intensity;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Light;

            return other != null &&
                Position == other.Position &&
                Intensity == other.Intensity;
        }

        public static bool operator==(Light t1, Light t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            if(object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(Light t1, Light t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(Position.GetHashCode() * 3 + Intensity.GetHashCode() * 2);
        }

        public static Light PointLight(Point position, Colour intensity)
        {
            return new Light(position, intensity);
        }
    }
}
