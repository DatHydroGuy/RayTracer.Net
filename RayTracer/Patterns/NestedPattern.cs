using System;

namespace RayTracer
{
    public class NestedPattern : Pattern
    {
        private Pattern _parentPattern;
        private Pattern _patternA;
        private Pattern _patternB;

        public Pattern ParentPattern
        {
            get { return _parentPattern; }
            set { _parentPattern = value; }
        }
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

        public NestedPattern(Pattern parentPattern, Pattern patternA, Pattern patternB): base(null, null)
        {
            ParentPattern = parentPattern;
            PatternA = patternA;
            PatternB = patternB;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var patternPoint = ParentPattern.TransformInverse * targetPoint;
            var parentPatternPoint = ParentPattern.ColourAtPoint(patternPoint);
            var patternAPoint = PatternA.TransformInverse * targetPoint;
            var patternBPoint = PatternB.TransformInverse * targetPoint;
            return parentPatternPoint == ParentPattern.ColourA ? PatternA.ColourAtPoint(patternAPoint) : PatternB.ColourAtPoint(patternBPoint);
        }
    }
}
