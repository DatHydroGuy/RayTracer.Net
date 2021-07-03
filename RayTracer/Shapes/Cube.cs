using System;

namespace RayTracer.Shapes
{
    public class Cube: Shape
    {
        public override bool Equals(object obj)
        {
            var other = obj as Cube;

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Cube Clone()
        {
            return new Cube
            {
                Origin = Origin.Clone(),
                Material = Material.Clone(),
                Transform = Transform.Clone(),
                Parent = Parent
            };
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            return $"[{baseStr}]";
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            CheckAxis(ray.Origin.X, ray.Direction.X, out var xTMin, out var xTMax);
            CheckAxis(ray.Origin.Y, ray.Direction.Y, out var yTMin, out var yTMax);
            CheckAxis(ray.Origin.Z, ray.Direction.Z, out var zTMin, out var zTMax);
            var tMin = Math.Max(xTMin, Math.Max(yTMin, zTMin));
            var tMax = Math.Min(xTMax, Math.Min(yTMax, zTMax));
            return tMin > tMax ? Array.Empty<Intersection>() : new Intersection[] {new(tMin, this), new(tMax, this)};
        }

        private static void CheckAxis(double origin, double direction, out double xTMin, out double xTMax)
        {
            double tMin, tMax;
            var tMinNumerator = -1 - origin;
            var tMaxNumerator = 1 - origin;

            if (Math.Abs(direction) >= Utilities.EPSILON)
            {
                tMin = tMinNumerator / direction;
                tMax = tMaxNumerator / direction;
            }
            else
            {
                tMin = tMinNumerator * double.PositiveInfinity;
                tMax = tMaxNumerator * double.PositiveInfinity;
            }

            if (tMin > tMax)
            {
                xTMin = tMax;
                xTMax = tMin;
            }
            else
            {
                xTMin = tMin;
                xTMax = tMax;
            }
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            var maxC = Math.Max(Math.Abs(objectPoint.X), Math.Max(Math.Abs(objectPoint.Y), Math.Abs(objectPoint.Z)));

            if (Utilities.AlmostEqual(maxC, Math.Abs(objectPoint.X)))
            {
                return new Vector(objectPoint.X, 0, 0);
            }

            return Utilities.AlmostEqual(maxC, Math.Abs(objectPoint.Y)) ? new Vector(0, objectPoint.Y, 0) : new Vector(0, 0, objectPoint.Z);
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
        }
    }
}
