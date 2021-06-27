using System;
using System.Collections.Generic;

namespace RayTracer
{
    /// Source: http://blog.marcinchwedczuk.pl/ray-tracing-torus
    public class Torus: Shape
    {
        private double _majorRadius;
        private double _minorRadius;

        public double MajorRadius
        {
            get { return _majorRadius; }
            set { _majorRadius = value; }
        }
        public double MinorRadius
        {
            get { return _minorRadius; }
            set { _minorRadius = value; }
        }

        /* For best results, keep both MajorRadius and MinorRadius <1 and use scaling transformation */
        public Torus(double majorRadius = 0, double minorRadius = 0): base()
        {
            MajorRadius = majorRadius == 0 ? 0.75 : majorRadius;
            MinorRadius = minorRadius == 0 ? 0.25 : minorRadius;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Torus;

            return base.Equals(other) &&
                MajorRadius == other.MajorRadius &&
                MinorRadius == other.MinorRadius;
        }

        public override int GetHashCode()
        {
            return (int)(MajorRadius * 11 + MinorRadius * 7 + base.GetHashCode());
        }

        public override Torus Clone()
        {
            return new Torus(MajorRadius, MinorRadius)
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
            var materialIndex = baseStr.IndexOf("\nMaterial:");
            return $"[{baseStr.Substring(0, materialIndex)}\nMinorRadius:{MinorRadius},MajorRadius:{MajorRadius}\n{baseStr.Substring(materialIndex + 1)}]";
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            // var torusToRay = ray.Origin - Origin;   // origin should always be Point(0, 0, 0)       // centerToRayOrigin 

            // var radiusSquareds = MinorRadius * MinorRadius + MajorRadius * MajorRadius;
            // var dSquareds = (ray.Direction.X * ray.Direction.X + ray.Direction.Y * ray.Direction.Y + ray.Direction.Z * ray.Direction.Z);
            // var oSquareds = (ray.Origin.X * ray.Origin.X + ray.Origin.Y * ray.Origin.Y + ray.Origin.Z * ray.Origin.Z);
            // var originDotDirection = (ray.Origin.X * ray.Direction.X + ray.Origin.Y * ray.Direction.Y + ray.Origin.Z * ray.Direction.Z);
            // var quarticA = dSquareds * dSquareds;
            // var quarticB = 4.0 * dSquareds * originDotDirection;
            // var quarticC = 2.0 * dSquareds * (oSquareds - radiusSquareds) + 4.0 * originDotDirection * originDotDirection + 4.0 * MajorRadius * MajorRadius * ray.Direction.Y * ray.Direction.Y;
            // var quarticD = 4.0 * (oSquareds - radiusSquareds) * originDotDirection + 8.0 * MajorRadius * MajorRadius * ray.Origin.Y * ray.Direction.Y;
            // var quarticE = (oSquareds - radiusSquareds) * (oSquareds - radiusSquareds) - 4.0 * MajorRadius * MajorRadius * (MinorRadius * MinorRadius - ray.Origin.Y * ray.Origin.Y);


            // var torusToRayDotDirection = torusToRay.Dot(ray.Direction);
            // var torusToRayDotDirectionSquared = torusToRay.Dot(torusToRay);
            // var minorRadiusSquared = MinorRadius * MinorRadius;
            // var majorRadiusSquared = MajorRadius * MajorRadius;

            // var yAxisDotTorusToRay = new Vector(0, 1, 0).Dot(torusToRay);
            // var yAxisDotRayDirection = new Vector(0, 1, 0).Dot(ray.Direction);
            // var a = 1 - yAxisDotRayDirection * yAxisDotRayDirection;
            // var b = 2 * (torusToRay.Dot(ray.Direction) - yAxisDotTorusToRay * yAxisDotRayDirection);
            // var c = torusToRayDotDirectionSquared - yAxisDotTorusToRay * yAxisDotTorusToRay;
            // var d = torusToRayDotDirectionSquared + majorRadiusSquared - minorRadiusSquared;

            // // Set up quartic equation
            // var quarticA = 1;
            // var quarticB = 4 * torusToRayDotDirection;
            // var quarticC = 2 * d + quarticB * quarticB * 0.25 - 4 * majorRadiusSquared * a;
            // var quarticD = quarticB * d - 4 * majorRadiusSquared * b;
            // var quarticE = d * d - 4 * majorRadiusSquared * c;


            var oX = ray.Origin.X;
            var oY = ray.Origin.Y;
            var oZ = ray.Origin.Z;
            var dX = ray.Direction.X;
            var dY = ray.Direction.Y;
            var dZ = ray.Direction.Z;

            var sumDSquared = dX * dX + dY * dY + dZ * dZ;      // By definition of a normalised ray vector, this is always 1
            var sumOSquared = oX * oX + oY * oY + oZ * oZ;
            var e = sumOSquared - MajorRadius * MajorRadius - MinorRadius * MinorRadius;
            var f = oX * dX + oY * dY + oZ * dZ;
            var fourASquared = 4.0 * MajorRadius * MajorRadius;

            var quarticA = sumDSquared * sumDSquared;
            var quarticB = 4.0 * sumDSquared * f;
            var quarticC = 2.0 * sumDSquared * e + 4.0 * f * f + fourASquared * dY * dY;
            var quarticD = 4.0 * f * e + 2.0 * fourASquared * oY * dY;
            var quarticE = e * e - fourASquared * (MinorRadius * MinorRadius - oY * oY);

            var results = Utilities.Solve4(quarticA, quarticB, quarticC, quarticD, quarticE);

            // var sumOSquared = oX * oX + oY * oY + oZ * oZ;
            // var e = sumOSquared - MajorRadius * MajorRadius - MinorRadius * MinorRadius;
            // var f = oX * dX + oY * dY + oZ * dZ;
            // var fourASquared = 4.0 * MajorRadius * MajorRadius;

            // var quarticA = 1.0;
            // var quarticB = 4.0 * f;
            // var quarticC = 2.0 * e + 4.0 * f * f + fourASquared * dY * dY;
            // var quarticD = 4.0 * f * e + 2.0 * fourASquared * oY * dY;
            // var quarticE = e * e - fourASquared * (MinorRadius * MinorRadius - oY * oY);

            // var results = Utilities.Solve4(quarticA, quarticB, quarticC, quarticD, quarticE);

            var intersections = new List<Intersection>();
            foreach (var root in results)
            {
                intersections.Add(new Intersection(root, this));
            }
            intersections.Sort(new Comparison<Intersection>((x, y) => x.T.CompareTo(y.T)));
            return intersections.ToArray();
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            var angle = Math.Atan2(objectPoint.Z, objectPoint.X);
            var tubePoint = new Point(MajorRadius * Math.Cos(angle), 0, MajorRadius * Math.Sin(angle));
            return (objectPoint - tubePoint).Normalise();

            // var radiiSquared = MajorRadius * MajorRadius + MinorRadius * MinorRadius;
            // var sumSquared = objectPoint.X * objectPoint.X + objectPoint.Y * objectPoint.Y + objectPoint.Z * objectPoint.Z;
            // var norm = new Vector(4.0 * objectPoint.X * (sumSquared - radiiSquared),
            //                       4.0 * objectPoint.Y * (sumSquared - radiiSquared + 2.0 * MajorRadius * MajorRadius),
            //                       4.0 * objectPoint.Z * (sumSquared - radiiSquared));

            // return norm.Normalise();
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, -0.5, -1), new Point(1, 0.5, 1));
        }
    }
}
