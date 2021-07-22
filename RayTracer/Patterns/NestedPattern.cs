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

        public override bool Equals(object obj)
        {
            var other = obj as NestedPattern;

            return base.Equals(other) &&
                ParentPattern == other.ParentPattern &&
                PatternA == other.PatternA &&
                PatternB == other.PatternB;
        }

        public override int GetHashCode()
        {
            return (int)(ParentPattern.GetHashCode() * 7 + PatternA.GetHashCode() * 5 + PatternB.GetHashCode() * 3 + base.GetHashCode() * 2);
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var patternPoint = ParentPattern.TransformInverse * targetPoint;
            var parentPatternPoint = ParentPattern.ColourAtPoint(patternPoint);
            var patternAPoint = PatternA.TransformInverse * targetPoint;
            var patternBPoint = PatternB.TransformInverse * targetPoint;
            return parentPatternPoint == ParentPattern.ColourA ? PatternA.ColourAtPoint(patternAPoint) : PatternB.ColourAtPoint(patternBPoint);
        }

        public override NestedPattern Clone()
        {
            return new NestedPattern(ParentPattern.Clone(), PatternA.Clone(), PatternB.Clone())
            {
                ParentPattern = ParentPattern == null ? null : ParentPattern.Clone(),
                ColourA = ColourA == null ? null : ColourA.Clone(),
                ColourB = ColourB == null ? null : ColourB.Clone()
            };
        }

        public override string ToString()
        {
            var parent = ParentPattern == null ? "null\n" : ParentPattern.ToString();
            var patA = PatternA == null ? "null\n" : PatternA.ToString();
            var patB = PatternB == null ? "null\n" : PatternB.ToString();
            return $"[{base.ToString()}ParentPattern:{parent}PatternA:{patA}PatternB:{patB}]\n";
        }
    }
}
