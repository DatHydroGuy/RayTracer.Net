using System;

namespace RayTracer
{
    public class AlignCheckPattern: UvPattern
    {
        public Colour MainColour { get; set; }
        public Colour UpperLeftColour { get; set; }
        public Colour UpperRightColour { get; set; }
        public Colour LowerLeftColour { get; set; }
        public Colour LowerRightColour { get; set; }

        public AlignCheckPattern(): base(Colour.BLACK, Colour.WHITE)
        {}

        public AlignCheckPattern(Colour mainColour, Colour upperLeftColour, Colour upperRightColour,
            Colour lowerLeftColour, Colour lowerRightColour) : this()
        {
            MainColour = mainColour;
            UpperLeftColour = upperLeftColour;
            UpperRightColour = upperRightColour;
            LowerLeftColour = lowerLeftColour;
            LowerRightColour = lowerRightColour;
            UvPatternMapType = UvPatternMapType.Cubic;
        }

        public Colour ColourAtUv(double u, double v)
        {
            return v switch
            {
                > 0.8 => u < 0.2 ? UpperLeftColour : u > 0.8 ? UpperRightColour : MainColour,
                < 0.2 => u < 0.2 ? LowerLeftColour : u > 0.8 ? LowerRightColour : MainColour,
                _ => MainColour
            };
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var (u, v) = TextureMap(targetPoint);
            return ColourAtUv(u, v);
        }

        public override AlignCheckPattern Clone()
        {
            return new AlignCheckPattern(MainColour.Clone(), UpperLeftColour.Clone(), UpperRightColour.Clone(),
                LowerLeftColour.Clone(), LowerRightColour.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}, MainColour: {MainColour}, UpperLeftColour: {UpperLeftColour}, UpperRightColour: {UpperRightColour}, LowerLeftColour: {LowerLeftColour}, LowerRightColour: {LowerRightColour}, MappingType:{UvPatternMapType}]\n";
        }
    }
}
