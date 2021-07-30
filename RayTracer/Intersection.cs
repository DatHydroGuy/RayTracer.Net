using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer
{
    public class Intersection
    {
        private double _t;
        private double _u;
        private double _v;
        private Shape _obj;
        
        public double T
        {
            get { return _t; }
            set { _t = value; }
        }
        public double U
        {
            get { return _u; }
            set { _u = value; }
        }
        public double V
        {
            get { return _v; }
            set { _v = value; }
        }
        public Shape Obj
        {
            get { return _obj; }
            set { _obj = value; }
        }

        public Intersection(double t, Shape obj)
        {
            T = t;
            U = double.NaN;
            V = double.NaN;
            Obj = obj;
        }

        public Comps PrepareComputations(Ray r, Intersection[] allIntersections = null)
        {
            var comps = new Comps();
            comps.T = T;
            comps.Obj = Obj;
            comps.TargetPoint = r.Position(comps.T);
            comps.EyeVector = -r.Direction;
            comps.NormalVector = comps.Obj.NormalAt(comps.TargetPoint, this);

            if (comps.NormalVector.Dot(comps.EyeVector) < 0)
            {
                comps.Inside = true;
                comps.NormalVector = -comps.NormalVector;
            }
            else
            {
                comps.Inside = false;
            }

            // Compute the reflection vector of the ray
            comps.ReflectVector = r.Direction.Reflect(comps.NormalVector);

            // Initialise the OverPoint so that shadow casting avoids "acne" effects
            comps.OverPoint = comps.TargetPoint + comps.NormalVector * Utilities.EPSILON;

            // Initialise the UnderPoint so that reflection and refraction rays can be cast
            comps.UnderPoint = comps.TargetPoint - comps.NormalVector * Utilities.EPSILON;

            if (allIntersections == null)
            {
                allIntersections = new Intersection[] {this};
            }
            double n1;
            double n2;
            CalculateRefractiveNormals(allIntersections, out n1, out n2);
            comps.N1 = n1;
            comps.N2 = n2;

            return comps;
        }

        private void CalculateRefractiveNormals(Intersection[] allIntersections, out double n1, out double n2)
        {
            var containers = new List<Shape>();
            var n1Out = 1.0;
            var n2Out = 1.0;

            foreach (var intersection in allIntersections)
            {
                if(Utilities.AlmostEqual(intersection.T, T))
                {
                    if(containers.Count > 0)
                    {
                        n1Out = containers.Last().Material.RefractiveIndex;
                    }
                }

                if(containers.Contains(intersection.Obj))
                {
                    containers.Remove(intersection.Obj);
                }
                else
                {
                    containers.Add(intersection.Obj);
                }

                if(Utilities.AlmostEqual(intersection.T, T))
                {
                    if(containers.Count > 0)
                    {
                        n2Out = containers.Last().Material.RefractiveIndex;
                    }
                    break;
                }
            }

            n1 = n1Out;
            n2 = n2Out;
        }

        public static Intersection IntersectionWithUV(double time, Shape shape, double u, double v)
        {
            var intersection = new Intersection(time, shape);
            intersection.U = u;
            intersection.V = v;
            return intersection;
        }

        public static Intersection[] Intersections(params Intersection[] intersections)
        {
            return intersections;
        }

        public static Intersection Hit(Intersection[] i)
        {
            Array.Sort(i, new Comparison<Intersection>((x, y) => x.T.CompareTo(y.T)));
            return Array.Find(i, x => x.T >= 0);
        }

        public static double Schlick(Comps comps)
        {
            // Find cosine of angle between eye and normal vectors using dot product
            var cosThetaI = comps.EyeVector.Dot(comps.NormalVector);

            // Can only have total internal reflection if n1 > n2
            if (comps.N1 > comps.N2)
            {
                var n = comps.N1 / comps.N2;
                var sinThetaTSquared = n * n * (1.0 - cosThetaI * cosThetaI);
                if (sinThetaTSquared > 1.0)
                {
                    return 1.0;
                }

                // Compute cosine of thetaT using trig identities
                var cosThetaT = Math.Sqrt(1.0 - sinThetaTSquared);

                // When n1 > n2, use cosThetaT instead of cosThetaI
                cosThetaI = cosThetaT;
            }

            var r0 = ((comps.N1 - comps.N2) / (comps.N1 + comps.N2)) * ((comps.N1 - comps.N2) / (comps.N1 + comps.N2));
            return r0 + (1.0 - r0) * Math.Pow(1.0 - cosThetaI, 5);
        }
    }
}
