using System;

namespace RayTracer
{
    public class Cube: Shape
    {
        public Cube(): base()
        {
        }

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
            double xTMin, xTMax, yTMin, yTMax, zTMin, zTMax;
            CheckAxis(ray.Origin.X, ray.Direction.X, out xTMin, out xTMax);
            CheckAxis(ray.Origin.Y, ray.Direction.Y, out yTMin, out yTMax);
            CheckAxis(ray.Origin.Z, ray.Direction.Z, out zTMin, out zTMax);
            var tMin = Math.Max(xTMin, Math.Max(yTMin, zTMin));
            var tMax = Math.Min(xTMax, Math.Min(yTMax, zTMax));
            if (tMin > tMax)
            {
                return new Intersection[] {};
            }
            return new Intersection[2] {new Intersection(tMin, this), new Intersection(tMax, this)};
        }

        private void CheckAxis(double origin, double direction, out double xTMin, out double xTMax)
        {
            double tMin, tMax;
            double tMinNumerator = -1 - origin;
            double tMaxNumerator = 1 - origin;

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
            double maxC = Math.Max(Math.Abs(objectPoint.X), Math.Max(Math.Abs(objectPoint.Y), Math.Abs(objectPoint.Z)));

            if (Utilities.AlmostEqual(maxC, Math.Abs(objectPoint.X)))
            {
                return new Vector(objectPoint.X, 0, 0);
            }
            else if (Utilities.AlmostEqual(maxC, Math.Abs(objectPoint.Y)))
            {
                return new Vector(0, objectPoint.Y, 0);
            }
            else
            {
                return new Vector(0, 0, objectPoint.Z);
            }
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
        }
    }
}
