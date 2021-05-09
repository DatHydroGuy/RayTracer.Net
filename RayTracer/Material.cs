using System;

namespace RayTracer
{
    public class Material
    {
        private Colour _colour;
        private double _ambient;
        private double _diffuse;
        private double _specular;
        private double _shininess;
        private double _reflective;
        private double _transparency;
        private double _refractiveIndex;
        private bool _castsShadow;
        private Pattern _pattern;
        
        public Colour Colour
        {
            get { return _colour; }
            set { _colour = value; }
        }
        public double Ambient
        {
            get { return _ambient; }
            set { _ambient = value; }
        }
        public double Diffuse
        {
            get { return _diffuse; }
            set { _diffuse = value; }
        }
        public double Specular
        {
            get { return _specular; }
            set { _specular = value; }
        }
        public double Shininess
        {
            get { return _shininess; }
            set { _shininess = value; }
        }
        public double Reflective
        {
            get { return _reflective; }
            set { _reflective = value; }
        }
        public double Transparency
        {
            get { return _transparency; }
            set { _transparency = value; }
        }
        public double RefractiveIndex
        {
            get { return _refractiveIndex; }
            set { _refractiveIndex = value; }
        }
        public bool CastsShadow
        {
            get { return _castsShadow; }
            set { _castsShadow = value; }
        }
        public Pattern Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

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
                CastsShadow == other.CastsShadow;
        }

        public static bool operator==(Material m1, Material m2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            if(object.ReferenceEquals(m1, null))
            {
                return object.ReferenceEquals(m2, null);
            }

            return m1.Equals(m2);
        }

        public static bool operator!=(Material m1, Material m2)
        {
            return !(m1 == m2);
        }

        public override int GetHashCode()
        {
            int shadowContrib = CastsShadow ? 1 : -1;
            return (int)(Ambient * 2 + Diffuse * 3 + Specular * 5 + Shininess * 7 + Reflective * 11 + Transparency * 13 + RefractiveIndex * 17 + shadowContrib * 19);
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

            // If dot product of light and normal vectors is negative, light is on other side of surface
            var lightDotNormal = lightVector.Dot(normalVector);
            if (lightDotNormal >= 0 && !isInShadow)     // TODO: If isInShadow = true, calculate new light ray from targetPoint for transparent materials
            {
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
            }

            // Add the three contributions together to get final shading
            return ambient + diffuse + specular;
        }
    }
}
