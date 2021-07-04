namespace RayTracer
{
    public class Colour
    {
        public static readonly Colour BLACK = new(0, 0, 0);
        public static readonly Colour WHITE = new(1, 1, 1);
        public static readonly Colour RED = new(1, 0, 0);
        public static readonly Colour GREEN = new(0, 1, 0);
        public static readonly Colour BLUE = new(0, 0, 1);

        public double Red { get; private init; }

        public double Green { get; private init; }

        public double Blue { get; private init; }

        public Colour(double red = 0.0, double green = 0.0, double blue = 0.0)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Colour;

            return other != null &&
                Utilities.AlmostEqual(Red, other.Red) &&
                Utilities.AlmostEqual(Green, other.Green) &&
                Utilities.AlmostEqual(Blue, other.Blue);
        }

        public static bool operator==(Colour t1, Colour t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            return t1?.Equals(t2) ?? ReferenceEquals(t2, null);
        }

        public static bool operator!=(Colour t1, Colour t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(Red * 5 + Green * 3 + Blue * 2);
        }

        public static Colour operator +(Colour p1, Colour p2)
        {
            return new Colour{
                Red = p1.Red + p2.Red,
                Green = p1.Green + p2.Green,
                Blue = p1.Blue + p2.Blue
            };
        }

        public static Colour operator -(Colour p1, Colour p2)
        {
            return new Colour{
                Red = p1.Red - p2.Red,
                Green = p1.Green - p2.Green,
                Blue = p1.Blue - p2.Blue
            };
        }

        public static Colour operator -(Colour p)
        {
            return new Colour{
                Red = -p.Red,
                Green = -p.Green,
                Blue = -p.Blue
            };
        }

        public static Colour operator *(Colour p1, Colour p2)
        {
            return new Colour{
                Red = p1.Red * p2.Red,
                Green = p1.Green * p2.Green,
                Blue = p1.Blue * p2.Blue
            };
        }

        public static Colour operator *(Colour p1, double d)
        {
            return new Colour{
                Red = p1.Red * d,
                Green = p1.Green * d,
                Blue = p1.Blue * d
            };
        }

        public static Colour operator *(double d, Colour p1)
        {
            return new Colour{
                Red = p1.Red * d,
                Green = p1.Green * d,
                Blue = p1.Blue * d
            };
        }

        public static Colour operator /(Colour p1, double d)
        {
            return new Colour{
                Red = p1.Red / d,
                Green = p1.Green / d,
                Blue = p1.Blue / d
            };
        }

        public override string ToString()
        {
            return $"[R:{Red}, G:{Green}, B:{Blue}]\n";
        }

        public Colour Clone()
        {
            return new Colour(Red, Green, Blue);
        }
    }
}
