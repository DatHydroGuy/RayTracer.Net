using System;

namespace RayTracer
{
    public class GradientRingPattern: Pattern
    {        
        public GradientRingPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var distance = ColourB - ColourA;
            var pointDistance = Math.Sqrt(targetPoint.X * targetPoint.X + targetPoint.Z * targetPoint.Z);
            var fraction = pointDistance - Math.Floor(pointDistance);
            return ColourA + distance * fraction;
        }

        public override GradientRingPattern Clone()
        {
            return new GradientRingPattern(ColourA.Clone(), ColourB.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
