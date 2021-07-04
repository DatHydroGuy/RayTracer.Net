using RayTracer.Shapes;

namespace RayTracer
{
    public class Comps
    {
        public double T { get; set; }

        public Shape Obj { get; set; }

        public Point TargetPoint { get; set; }

        public Point OverPoint { get; set; }

        public Point UnderPoint { get; set; }

        public Vector EyeVector { get; set; }

        public Vector NormalVector { get; set; }

        public Vector ReflectVector { get; set; }

        public bool Inside { get; set; }

        public double N1 { get; set; }

        public double N2 { get; set; }
    }
}
