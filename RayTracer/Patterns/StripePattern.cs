using System;

namespace RayTracer
{
    public class StripePattern: Pattern
    {        
        public StripePattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            return Math.Floor(targetPoint.X) % 2 == 0 ? ColourA : ColourB;
        }

        public override StripePattern Clone()
        {
            return new StripePattern(ColourA.Clone(), ColourB.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
