using System;

namespace RayTracer
{
    public abstract class Pattern
    {
        private Colour _colourA;
        private Colour _colourB;
        private Matrix _transform;
        private Matrix _transformInverse;

        public Matrix Transform
        {
            get { return _transform; }
            set { 
                _transform = value;
                _transformInverse = value.Inverse();
                }
        }
        public Matrix TransformInverse
        {
            get { return _transformInverse; }
            set { 
                _transformInverse = value;
                _transform = value.Inverse();
                }
        }
        public Colour ColourA
        {
            get { return _colourA; }
            set { _colourA = value; }
        }
        public Colour ColourB
        {
            get { return _colourB; }
            set { _colourB = value; }
        }
        
        public Pattern(Colour colourA, Colour colourB)
        {
            ColourA = colourA;
            ColourB = colourB;
            Transform = Matrix.Identity(4);
        }

        public abstract Colour ColourAtPoint(Point targetPoint);

        public Colour ColourAtShape(Shape obj, Point targetPoint)
        {
            // var objectPoint = obj.TransformInverse * targetPoint;
            var objectPoint = obj.WorldToObject(targetPoint);
            var patternPoint = TransformInverse * objectPoint;
            return ColourAtPoint(patternPoint);
        }
    }
}
