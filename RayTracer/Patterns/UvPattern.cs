using System;
using System.Collections.Generic;

namespace RayTracer
{
    public class UvPattern: Pattern
    {
        public UvPatternMapType UvPatternMapType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public UvPattern(): base(Colour.BLACK, Colour.WHITE)
        {}

        public UvPattern(Colour colourA, Colour colourB): base(colourA, colourB)
        {}

        public UvPattern(int width, int height, Colour colourA, Colour colourB)
            :this(colourA, colourB)
        {
            Width = width;
            Height = height;
        }

        public UvPattern(int width, int height, Colour colourA, Colour colourB, UvPatternMapType mappingType)
            :this(width, height, colourA, colourB)
        {
            UvPatternMapType = mappingType;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            throw new NotImplementedException();
        }

        protected (double, double) TextureMap(Point p)
        {
            return UvPatternMapType switch
            {
                UvPatternMapType.Spherical => UvPatternMaps.UvSphericalMapping(p),
                UvPatternMapType.Planar => UvPatternMaps.UvPlanarMapping(p),
                UvPatternMapType.Cylindrical => UvPatternMaps.UvCylindricalMapping(p),
                UvPatternMapType.Cubic => UvPatternMaps.UvCubicMapping(p),
                _ => throw new InvalidOperationException($"Unrecognised UV Mapping Type: {UvPatternMapType}")
            };
        }

        public override UvPattern Clone()
        {
            return new UvPattern(Width, Height, ColourA.Clone(), ColourB.Clone(), UvPatternMapType);
        }

        public override string ToString()
        {
            return $"[{base.ToString()}, Width: {Width}, Height: {Height}, MappingType:{UvPatternMapType}]\n";
        }
    }
}
