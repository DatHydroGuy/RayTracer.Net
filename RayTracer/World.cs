using System;
using System.Collections.Generic;
using RayTracer.Shapes;

namespace RayTracer
{
    public class World
    {
        private Shape[] _objects;
        private Light[] _lights;

        public Shape[] Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }
        public Light[] Lights
        {
            get { return _lights; }
            set { _lights = value; }
        }

        public World()
        {
            Objects = new Shape[] {};
            Lights = new Light[] {};
        }

        public void AddShapeToWorld(Shape s)
        {
            Objects = Utilities.Append<Shape>(Objects, s);
        }

        public void AddShapesToWorld(Shape[] s)
        {
            Objects = Utilities.AppendMany<Shape>(Objects, s);
        }

        public void AddLightToWorld(Light l)
        {
            Lights = Utilities.Append<Light>(Lights, l);
        }

        public void AddLightsToWorld(Light[] l)
        {
            Lights = Utilities.AppendMany<Light>(Lights, l);
        }

        public Intersection[] IntersectWorld(Ray r)
        {
            var intersections = new List<Intersection>();
            foreach (var obj in Objects)
            {
                intersections.AddRange(obj.Intersects(r));
            }
            intersections.Sort(new Comparison<Intersection>((x, y) => x.T.CompareTo(y.T)));
            return intersections.ToArray();
        }

        public Colour ShadeHit(Comps comps, int remainingReflections = 5)
        {
            var hitColour = new Colour(0, 0, 0);

            foreach (Light light in Lights)
            {
                bool isInShadow = IsShadowed(comps.OverPoint, light);
                hitColour += comps.Obj.Material.Lighting(comps.Obj, light, comps.OverPoint, comps.EyeVector, comps.NormalVector, isInShadow);
                var reflectedColour = ReflectedColour(comps, remainingReflections);    // Add any reflected colour into base colour
                var refractedColour = RefractedColour(comps, remainingReflections);    // Add any refracted colour into base colour

                var material = comps.Obj.Material;
                if(material.Reflective > 0 && material.Transparency > 0)
                {
                    var reflectance = Intersection.Schlick(comps);
                    hitColour += reflectedColour * reflectance;
                    hitColour += refractedColour * (1.0 - reflectance);
                }
                else
                {
                    hitColour += reflectedColour;
                    hitColour += refractedColour;
                }
            }

            return hitColour;
        }

        public Colour ColourAt(Ray r, int remainingReflections = 5)
        {
            var intersections = IntersectWorld(r);
            var hit = Intersection.Hit(intersections);

            if (hit == null)
            {
                return new Colour(0, 0, 0);
            }
            else
            {
                var comps = hit.PrepareComputations(r, intersections);
                return ShadeHit(comps, remainingReflections);
            }
        }

        public Colour ReflectedColour(Comps comps, int remainingReflections = 5)
        {
            if (comps.Obj.Material.Reflective == 0 || remainingReflections == 0)
            {
                return Colour.BLACK;
            }
            
            var reflectRay = new Ray(comps.OverPoint, comps.ReflectVector);
            var reflectColour = ColourAt(reflectRay, remainingReflections - 1);

            return reflectColour * comps.Obj.Material.Reflective;
        }

        public Colour RefractedColour(Comps comps, int remainingReflections = 5)
        {
            if (comps.Obj.Material.Transparency == 0 || remainingReflections == 0)
            {
                return Colour.BLACK;
            }

            // Find the ratio of first index of refraction to the second
            var nRatio = comps.N1 / comps.N2;

            // Cos(thetaI) is the same as the dot product of the two vectors
            var cosThetaI = comps.EyeVector.Dot(comps.NormalVector);

            // Use trig identities to find Sin(thetaT)^2
            var sinThetaTSquared = nRatio * nRatio * (1 - cosThetaI * cosThetaI);

            if (sinThetaTSquared > 1)
            {
                return Colour.BLACK;
            }

            // Find cosThetaT via trig identities
            var cosThetaT = Math.Sqrt(1.0 - sinThetaTSquared);

            // Compute direction of the refracted ray
            var direction = comps.NormalVector * (nRatio * cosThetaI - cosThetaT) - comps.EyeVector * nRatio;

            // Create the refracted ray
            var refractedRay = new Ray(comps.UnderPoint, direction);

            // Find colour of refracted ray.  Make sure you multiply by transparency value
            // to account for any opacity.
            var refractedRayColour = ColourAt(refractedRay, remainingReflections - 1);
            return refractedRayColour * comps.Obj.Material.Transparency;
        }

        public bool IsShadowed(Point point, Light light = null)
        {
            if (light == null)
            {
                light = Lights[0];
            }

            // Calculate shadow vector attributes
            var shadowVector = light.Position - point;
            var shadowVectorDistance = shadowVector.Magnitude();
            var shadowVectorDirection = shadowVector.Normalise();

            // Get all intersections for the shadow vector
            var ray = new Ray(point, shadowVectorDirection);
            var intersections = IntersectWorld(ray);

            // Find hit
            var hit = Intersection.Hit(intersections);

            // If a hit exists and is closer than the light source, then the point is in shadow
            if (hit != null && hit.T < shadowVectorDistance && hit.Obj.Material.CastsShadow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static World DefaultWorld()
        {
            var w = new World();
            w.Lights = new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1))};
            var s1 = new Sphere();
            s1.Material.Colour = new Colour(0.8, 1, 0.6);
            s1.Material.Diffuse = 0.7;
            s1.Material.Specular = 0.2;
            var s2 = new Sphere();
            s2.Transform = Transformations.Scaling(0.5, 0.5, 0.5);
            w.Objects = new Sphere[] {s1, s2};
            return w;
        }
    }
}
