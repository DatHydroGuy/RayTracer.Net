namespace RayTracer.Patterns
{
    public class NestedPattern : Pattern
    {
        public Pattern ParentPattern { get; set; }

        public Pattern PatternA { get; set; }

        public Pattern PatternB { get; set; }

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
                ParentPattern == other?.ParentPattern &&
                PatternA == other?.PatternA &&
                PatternB == other?.PatternB;
        }

        public override int GetHashCode()
        {
            return ParentPattern.GetHashCode() * 7 + PatternA.GetHashCode() * 5 + PatternB.GetHashCode() * 3 + base.GetHashCode() * 2;
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
