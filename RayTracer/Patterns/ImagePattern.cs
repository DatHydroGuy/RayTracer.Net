using System;

namespace RayTracer
{
    public class ImagePattern: UvPattern
    {
        public Canvas Canvas { get; set; }
        
        public ImagePattern(Canvas canvas): base(Colour.BLACK, Colour.WHITE)
        {
            Canvas = canvas;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            var (u, v) = TextureMap(targetPoint);
            return UvPatternAt(u, v);
        }

        public override ImagePattern Clone()
        {
            return new ImagePattern(Canvas);
        }

        public override string ToString()
        {
            return $"[Canvas:{Canvas}]\n";
        }
        
        public Colour UvPatternAt(double uValue, double vValue)
        {
            // Flip v-axis over so that it matches the y-axis with 0 bottom-most
            var v = 1 - vValue;
            
            // Scale by width-1 and height-1 so that we don't overflow when u or v are 1.
            var x = uValue * (Canvas.Width - 1);
            var y = v * (Canvas.Height - 1);
            
            // Round to nearest whole numbers
            return Canvas.PixelAt((int)Math.Round(x), (int)Math.Round(y));
        }
    }
}
