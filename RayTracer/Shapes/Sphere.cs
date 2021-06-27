using System;

namespace RayTracer
{
    public class Sphere: Shape
    {
        private double _radius;

        public double Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public Sphere(): base()
        {
            Radius = 1.0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Sphere;

            return base.Equals(other) &&
                Radius == other.Radius;
        }

        public override int GetHashCode()
        {
            return (int)(Radius * 7 + base.GetHashCode());
        }

        public override Sphere Clone()
        {
            return new Sphere
            {
                Origin = Origin.Clone(),
                Material = Material.Clone(),
                Transform = Transform.Clone(),
                Parent = Parent,
                Radius = Radius
            };
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            var materialIndex = baseStr.IndexOf("\nMaterial:");
            return $"[{baseStr.Substring(0, materialIndex)}\nRadius:{Radius}\n{baseStr.Substring(materialIndex + 1)}]";
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            var sphereToRay = ray.Origin - Origin;   // origin should always be Point(0, 0, 0)

            // Calculate the discriminant between sphere (this) and the passed in ray
            var a = ray.Direction.Dot(ray.Direction);
            var b = 2 * ray.Direction.Dot(sphereToRay);
            var c = sphereToRay.Dot(sphereToRay) - 1;
            var discriminant = b * b - 4 * a * c;

            // If discriminant < 0, then the ray misses the sphere altogether
            if ( discriminant < 0 )
                return new Intersection[] {};
            
            var t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            return new Intersection[2] {new Intersection(t1, this), new Intersection(t2, this)};
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            return (objectPoint - new Point(0, 0, 0)).Normalise();
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
        }

        public static Sphere GlassSphere()
        {
            var s = new Sphere();
            s.Material.Transparency = 1.0;
            s.Material.RefractiveIndex = 1.5;
            return s;
        }
    }
}
