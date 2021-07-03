using System;

namespace RayTracer.Patterns
{
    public class DoubleGradientRingPattern: Pattern
    {        
        public DoubleGradientRingPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var distance = ColourB - ColourA;
            var pointDistance = Math.Sqrt(targetPoint.X * targetPoint.X + targetPoint.Z * targetPoint.Z);
            var fraction = 1 - Math.Abs((pointDistance - Math.Floor(pointDistance) - 0.5) * 2);
            return ColourA + distance * fraction;
        }

        public override DoubleGradientRingPattern Clone()
        {
            return new DoubleGradientRingPattern(ColourA.Clone(), ColourB.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
