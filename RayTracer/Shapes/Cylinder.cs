using System;
using System.Collections.Generic;

namespace RayTracer
{
    public class Cylinder: Shape
    {
        private double _radius;
        private double _minimum;
        private double _maximum;
        private bool _closed;

        public double Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public double Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }
        public double Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }
        public bool Closed
        {
            get { return _closed; }
            set { _closed = value; }
        }
        
        public Cylinder(): base()
        {
            Radius = 1.0;
            Minimum = double.NegativeInfinity;
            Maximum = double.PositiveInfinity;
            Closed = false;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Cylinder;

            return base.Equals(other) &&
                Radius == other.Radius &&
                Minimum == other.Minimum &&
                Maximum == other.Maximum;
        }

        public override int GetHashCode()
        {
            int closedHash = Closed ? 1 : -1;
            return (int)(base.GetHashCode() + Radius * 11 + closedHash * 13);     // Not including min and max because the infinities might break the hashing!
        }

        public override Cylinder Clone()
        {
            return new Cylinder
            {
                Origin = Origin.Clone(),
                Material = Material,
                Transform = Transform.Clone(),
                Parent = Parent,
                Radius = Radius,
                Minimum = Minimum,
                Maximum = Maximum,
                Closed = Closed
            };
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            var result = new List<Intersection>();
            var a = ray.Direction.X * ray.Direction.X + ray.Direction.Z * ray.Direction.Z;

            // If ray isn't parallel to the y-axis, need to check for intersections
            if (!Utilities.AlmostEqual(0, a))
            {
                // Calculate the discriminant between cylinder and the passed in ray
                var b = 2 * ray.Origin.X * ray.Direction.X + 2 * ray.Origin.Z * ray.Direction.Z;
                var c = ray.Origin.X * ray.Origin.X + ray.Origin.Z * ray.Origin.Z - 1;
                var discriminant = b * b - 4 * a * c;

                // If discriminant < 0, then the ray misses the cylinder altogether
                if ( discriminant < 0 )
                    return new Intersection[] {};
                
                var t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                var t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);

                if (t1 > t2)
                {
                    var temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                var y1 = ray.Origin.Y + t1 * ray.Direction.Y;
                if (Minimum < y1 && y1 < Maximum)
                {
                    result.Add(new Intersection(t1, this));
                }

                var y2 = ray.Origin.Y + t2 * ray.Direction.Y;
                if (Minimum < y2 && y2 < Maximum)
                {
                    result.Add(new Intersection(t2, this));
                }
            }

            IntersectCaps(ray, ref result);

            return result.ToArray();
        }

        private void IntersectCaps(Ray ray, ref List<Intersection> intersections)
        {
            // Caps only matter if cylinder is closed and the ray isn't parallel to the y-axis
            if (!Closed || Utilities.AlmostEqual(0, ray.Direction.Y))
                return;
            
            // Check for intersection with lower end cap by intersecting with plane at Minimum
            var t = (Minimum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t))
            {
                intersections.Add(new Intersection(t, this));
            }
            
            // Check for intersection with upper end cap by intersecting with plane at Maximum
            t = (Maximum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t))
            {
                intersections.Add(new Intersection(t, this));
            }
        }

        /// Helper function checks to see if intersection at t is within a radius of 1 from the y-axis
        private bool CheckCap(Ray ray, double t)
        {
            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;

            return x * x + z * z <= 1;
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            // Compute the square of the distance from the y-axis
            var dist = objectPoint.X * objectPoint.X + objectPoint.Z * objectPoint.Z;

            if (dist < 1 && objectPoint.Y >= Maximum - Utilities.EPSILON)
                return new Vector(0, 1, 0);
            else if(dist < 1 && objectPoint.Y <= Minimum + Utilities.EPSILON)
                return new Vector(0, -1, 0);
            else
                return new Vector(objectPoint.X, 0, objectPoint.Z);
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, Minimum, -1),
                                   new Point(1, Maximum, 1));
        }
    }
}
