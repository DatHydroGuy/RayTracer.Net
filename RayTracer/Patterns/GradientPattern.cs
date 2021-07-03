using System;

namespace RayTracer.Patterns
{
    public class GradientPattern: Pattern
    {        
        public GradientPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var fraction = targetPoint.X - Math.Floor(targetPoint.X);
            return ColourA + (ColourB - ColourA) * fraction;
        }

        public override GradientPattern Clone()
        {
            return new GradientPattern(ColourA.Clone(), ColourB.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
