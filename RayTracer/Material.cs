using System;
using RayTracer.Patterns;
using RayTracer.Shapes;

namespace RayTracer
{
    public class Material
    {
        public Colour Colour { get; set; }

        public double Ambient { get; set; }

        public double Diffuse { get; set; }

        public double Specular { get; set; }

        public double Shininess { get; set; }

        public double Reflective { get; set; }

        public double Transparency { get; set; }

        public double RefractiveIndex { get; set; }

        public bool CastsShadow { get; set; }

        public Pattern Pattern { get; set; }

        public Material()
        {
            Colour = new Colour(1, 1, 1);
            Ambient = 0.1;
            Diffuse = 0.9;
            Specular = 0.9;
            Shininess = 200.0;
            Reflective = 0.0;
            Transparency = 0.0;
            RefractiveIndex = 1.0;
            CastsShadow = true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Material;

            return other != null &&
                Colour == other.Colour &&
                Utilities.AlmostEqual(Ambient, other.Ambient) &&
                Utilities.AlmostEqual(Diffuse, other.Diffuse) &&
                Utilities.AlmostEqual(Specular, other.Specular) &&
                Utilities.AlmostEqual(Shininess, other.Shininess) &&
                Utilities.AlmostEqual(Reflective, other.Reflective) &&
                Utilities.AlmostEqual(Transparency, other.Transparency) &&
                Utilities.AlmostEqual(RefractiveIndex, other.RefractiveIndex) &&
                CastsShadow == other.CastsShadow &&
                Pattern == other.Pattern;
        }

        public static bool operator==(Material m1, Material m2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            return m1?.Equals(m2) ?? ReferenceEquals(m2, null);
        }

        public static bool operator!=(Material m1, Material m2)
        {
            return !(m1 == m2);
        }

        public override int GetHashCode()
        {
            var shadowContrib = CastsShadow ? 1 : -1;
            return (int)(Ambient * 2 + Diffuse * 3 + Specular * 5 + Shininess * 7 + Reflective * 11 + Transparency * 13 + RefractiveIndex * 17 + shadowContrib * 19);
        }

        public override string ToString()
        {
            var pattern = Pattern == null ? "null\n" : Pattern.ToString();
            return $"[Colour:{Colour}Amb:{Ambient},Dif:{Diffuse},Spec:{Specular},Shin:{Shininess},Refl:{Reflective},Tran:{Transparency},Refr:{RefractiveIndex},Shad:{CastsShadow},\nPattern:{pattern}]\n";
        }

        public Material Clone()
        {
            return new Material
            {
                Colour = Colour.Clone(),
                Ambient = Ambient,
                Diffuse = Diffuse,
                Specular = Specular,
                Shininess = Shininess,
                Reflective = Reflective,
                Transparency = Transparency,
                RefractiveIndex = RefractiveIndex,
                CastsShadow = CastsShadow,
                Pattern = Pattern == null ? null : Pattern.Clone()
            };
        }

        public Colour Lighting(Shape obj, Light light, Point targetPoint, Vector eyeVector, Vector normalVector, bool isInShadow = false)
        {
            // If material has a pattern applied, get the pattern colour at the given point, otherwise use the material colour
            var effectiveColour = Pattern == null ? Colour : Pattern.ColourAtShape(obj, targetPoint);

            // Combine material colour with the light colour
            effectiveColour *= light.Intensity;

            // Compute ambient contribution
            var ambient = effectiveColour * Ambient;

            // Find direction to light source
            var lightVector = (light.Position - targetPoint).Normalise();

            var diffuse = new Colour(0, 0, 0);
            var specular = new Colour(0, 0, 0);
            
            // TODO: If isInShadow = true, calculate new light ray from targetPoint for transparent materials
            // If dot product of light and normal vectors is negative, light is on other side of surface
            var lightDotNormal = lightVector.Dot(normalVector);
            if (!(lightDotNormal >= 0) || isInShadow)
                return ambient + diffuse + specular;

            // Calculate diffuse contribution
            diffuse = effectiveColour * Diffuse * lightDotNormal;

            // If dot product of reflection vector and eye vector is negative, light reflects away from the eye
            var reflectionVector = -lightVector.Reflect(normalVector);
            var reflectDotEye = reflectionVector.Dot(eyeVector);

            if (reflectDotEye < 0)
            {
                specular = new Colour(0, 0, 0);
            }
            else
            {
                // Calculate specular contribution
                var factor = Math.Pow(reflectDotEye, Shininess);
                specular = light.Intensity * Specular * factor;
            }

            // Add the three contributions together to get final shading
            return ambient + diffuse + specular;
        }
    }
}
