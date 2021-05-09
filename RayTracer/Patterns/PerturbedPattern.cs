namespace RayTracer
{
    public class PerturbedPattern : Pattern
    {
        private Pattern _patternToPerturb;
        private double _scale;
        private int _octaves;
        private double _persistence;
        private double _lacunarity;

        public Pattern PatternToPerturb
        {
            get { return _patternToPerturb; }
            set { _patternToPerturb = value; }
        }
        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        public int Octaves
        {
            get { return _octaves; }
            set { _octaves = value; }
        }
        public double Persistence
        {
            get { return _persistence; }
            set { _persistence = value; }
        }
        public double Lacunarity
        {
            get { return _lacunarity; }
            set { _lacunarity = value; }
        }

        public PerturbedPattern(Pattern patternToPerturb, double scale = 0.4, int octaves = 1, double persistence = 0.5, double lacunarity = 2.0):
               base(patternToPerturb.ColourA, patternToPerturb.ColourB)
        {
            PatternToPerturb = patternToPerturb;
            Scale = scale;
            Octaves = octaves;
            Persistence = persistence;
            Lacunarity = lacunarity;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var patternPoint = PatternToPerturb.TransformInverse * targetPoint;
            var newX = patternPoint.X + Perlin.Perlin3(patternPoint.X, patternPoint.Y + 1, patternPoint.Z + 1, Octaves, Persistence, Lacunarity) * Scale;
            var newY = patternPoint.Y + Perlin.Perlin3(patternPoint.X + 1, patternPoint.Y, patternPoint.Z + 1, Octaves, Persistence, Lacunarity) * Scale;
            var newZ = patternPoint.Z + Perlin.Perlin3(patternPoint.X + 1, patternPoint.Y + 1, patternPoint.Z, Octaves, Persistence, Lacunarity) * Scale;
            return PatternToPerturb.ColourAtPoint(new Point(newX, newY, newZ));
        }
    }
}
