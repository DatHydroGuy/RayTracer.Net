namespace RayTracer
{
    public class Light
    {
        public Point Position { get; }

        public Colour Intensity { get; }

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
            return t1?.Equals(t2) ?? ReferenceEquals(t2, null);
        }

        public static bool operator!=(Light t1, Light t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() * 3 + Intensity.GetHashCode() * 2;
        }

        public override string ToString()
        {
            return $"Light:[Position:{Position}Intensity:{Intensity}]\n";
        }

        public static Light PointLight(Point position, Colour intensity)
        {
            return new Light(position, intensity);
        }
    }
}
