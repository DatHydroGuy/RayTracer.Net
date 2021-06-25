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

        public override bool Equals(object obj)
        {
            var other = obj as BlendedPattern;

            return base.Equals(other) &&
                PatternA == other.PatternA &&
                PatternB == other.PatternB;
        }

        public override int GetHashCode()
        {
            return (int)(PatternA.GetHashCode() * 5 + PatternB.GetHashCode() * 3 + base.GetHashCode() * 2);
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var patternAPoint = PatternA.TransformInverse * targetPoint;
            var patternBPoint = PatternB.TransformInverse * targetPoint;
            return (PatternA.ColourAtPoint(patternAPoint) + PatternB.ColourAtPoint(patternBPoint)) / 2.0;
        }

        public override BlendedPattern Clone()
        {
            return new BlendedPattern(PatternA.Clone(), PatternB.Clone())
            {
                ColourA = ColourA == null ? null : ColourA.Clone(),
                ColourB = ColourB == null ? null : ColourB.Clone()
            };
        }

        public override string ToString()
        {
            var patA = PatternA == null ? "null\n" : PatternA.ToString();
            var patB = PatternB == null ? "null\n" : PatternB.ToString();
            return $"[{base.ToString()}PatternA:{patA}PatternB:{patB}]\n";
        }
    }
}
