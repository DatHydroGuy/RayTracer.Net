using System;

namespace RayTracer
{
    public enum UvPatternMapType
    {
        Spherical,
        Cylindrical,
        Planar,
        Cubic
    }

    public enum CubeFace
    {
        Left,
        Right,
        Front,
        Back,
        Lower,
        Upper
    }
    
    public static class UvPatternMaps
    {
        public static (double, double) UvSphericalMapping(Point point)
        {
            // Azimuthal angle -π < theta <= π increases clockwise when viewed from above.
            // This is opposite to what we want, but will be fixed in a subsequent calculation.
            var theta = Math.Atan2(point.X, point.Z);

            var vec = new Vector(point.X, point.Y, point.Z);
            var radius = vec.Magnitude();

            // Polar angle 0 < phi <= π.  This also has a direction issue to fix later.
            var phi = Math.Acos(point.Y / radius);
            
            // Re-map theta into new variable in the rage -0.5 < rawU <= 0.5
            var rawU = theta / (2 * Math.PI);
            
            // We want to define u such that: 0 <= u < 1 and to fix the direction issue from the first calculation
            var u = 1 - (rawU + 0.5);
            
            // Want to define v such that 0 <= v <= 1 where 0 at "South pole" and 1 at "North pole".
            var v = 1 - phi / Math.PI;

            return (u, v);
        }

        public static (double, double) UvPlanarMapping(Point point)
        {
            var u = point.X % 1;
            var v = point.Z % 1;
            return (u < 0 ? u + 1 : u, v < 0 ? v + 1 : v);
        }

        public static (double, double) UvCylindricalMapping(Point point)
        {
            var theta = Math.Atan2(point.X, point.Z);
            var rawU = theta / (2 * Math.PI);
            var u = 1 - (rawU + 0.5);

            var v = point.Y % 1;
            return (u, v < 0 ? v + 1 : v);
        }
    }
}
