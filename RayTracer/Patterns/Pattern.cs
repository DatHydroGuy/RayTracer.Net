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

        public override bool Equals(object obj)
        {
            var other = obj as Pattern;

            return other != null &&
                ColourA == other.ColourA &&
                ColourB == other.ColourB;
        }

        public static bool operator==(Pattern t1, Pattern t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            if(object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(Pattern t1, Pattern t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(ColourA.GetHashCode() * 3 + ColourB.GetHashCode() * 2);
        }

        public abstract Colour ColourAtPoint(Point targetPoint);

        public abstract Pattern Clone();

        public Colour ColourAtShape(Shape obj, Point targetPoint)
        {
            // var objectPoint = obj.TransformInverse * targetPoint;
            var objectPoint = obj.WorldToObject(targetPoint);
            var patternPoint = TransformInverse * objectPoint;
            return ColourAtPoint(patternPoint);
        }
    }
}
