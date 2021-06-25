using System;

namespace RayTracer
{
    public class DoubleGradientPattern: Pattern
    {        
        public DoubleGradientPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var fraction = 1 - Math.Abs((targetPoint.X - Math.Floor(targetPoint.X) - 0.5) * 2);
            return ColourA + (ColourB - ColourA) * fraction;
        }

        public override DoubleGradientPattern Clone()
        {
            return new DoubleGradientPattern(ColourA.Clone(), ColourB.Clone());
        }
    }
}
