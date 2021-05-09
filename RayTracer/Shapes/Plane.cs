using System;

namespace RayTracer
{
    public class Plane: Shape
    {
        public Plane(): base()
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as Plane;

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            if (Math.Abs(ray.Direction.Y) < Utilities.EPSILON)
            {
                return new Intersection[] {};
            }
            else
            {
                var t = -ray.Origin.Y / ray.Direction.Y;    // Calculate slope of ray
                return new Intersection[] {new Intersection(t, this)};
            }
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            return new Vector(0, 1, 0);
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity),
                                   new Point(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity));
        }
    }
}
