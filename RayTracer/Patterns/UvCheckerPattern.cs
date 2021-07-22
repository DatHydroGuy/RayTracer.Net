using System;

namespace RayTracer
{
    public class UvCheckerPattern: UvPattern
    {
        public UvCheckerPattern(int width, int height, Colour colourA, Colour colourB)
            :base(width, height, colourA, colourB)
        {}
        public UvCheckerPattern(int width, int height, Colour colourA, Colour colourB, UvPatternMapType mappingType)
            :base(width, height, colourA, colourB, mappingType)
        {}

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var (u, v) = TextureMap(targetPoint);
            return ColourAtUv(u, v);
        }

        public Colour ColourAtUv(double uValue, double vValue)
        {
            var u = Math.Floor(uValue * Width);
            var v = Math.Floor(vValue * Height);

            return (u + v) % 2 == 0 ? ColourA : ColourB;
        }

        public override UvCheckerPattern Clone()
        {
            return new UvCheckerPattern(Width, Height, ColourA.Clone(), ColourB.Clone(), UvPatternMapType);
        }

        public override string ToString()
        {
            return $"[{base.ToString()}]\n";
        }
    }
}
