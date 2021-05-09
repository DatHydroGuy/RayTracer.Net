using System;

namespace RayTracer
{
    public class BlendedPattern : Pattern
    {
        private Pattern _patternA;
        private Pattern _patternB;

        public Pattern PatternA
        {
            get { return _patternA; }
            set { _patternA = value; }
        }
        public Pattern PatternB
        {
            get { return _patternB; }
            set { _patternB = value; }
        }

        public BlendedPattern(Pattern patternA, Pattern patternB): base(null, null)
        {
            PatternA = patternA;
            PatternB = patternB;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var patternAPoint = PatternA.TransformInverse * targetPoint;
            var patternBPoint = PatternB.TransformInverse * targetPoint;
            return (PatternA.ColourAtPoint(patternAPoint) + PatternB.ColourAtPoint(patternBPoint)) / 2.0;
        }
    }
}
