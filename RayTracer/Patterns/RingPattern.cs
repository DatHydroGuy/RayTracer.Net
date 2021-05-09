using System;

namespace RayTracer
{
    public class RingPattern: Pattern
    {        
        public RingPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            return Math.Floor(Math.Sqrt(targetPoint.X * targetPoint.X + targetPoint.Z * targetPoint.Z)) % 2 == 0 ? ColourA : ColourB;
        }
    }
}
