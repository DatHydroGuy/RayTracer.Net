using System;

namespace RayTracer
{
    public class CheckerPattern: Pattern
    {        
        public CheckerPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            return (Math.Floor(targetPoint.X) + Math.Floor(targetPoint.Y) + Math.Floor(targetPoint.Z)) % 2 == 0 ? ColourA : ColourB;
        }

        public override CheckerPattern Clone()
        {
            return new CheckerPattern(ColourA.Clone(), ColourB.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
