namespace RayTracer
{
    public class Comps
    {
        private double _t;
        private Shape _obj;
        private Point _point;
        private Point _overPoint;
        private Point _underPoint;
        private Vector _eyeVector;
        private Vector _normalVector;
        private Vector _reflectVector;
        private bool _inside;
        private double _n1;
        private double _n2;

        public double T
        {
            get { return _t; }
            set { _t = value; }
        }
        public Shape Obj
        {
            get { return _obj; }
            set { _obj = value; }
        }
        public Point TargetPoint
        {
            get { return _point; }
            set { _point = value; }
        }
        public Point OverPoint
        {
            get { return _overPoint; }
            set { _overPoint = value; }
        }
        public Point UnderPoint
        {
            get { return _underPoint; }
            set { _underPoint = value; }
        }
        public Vector EyeVector
        {
            get { return _eyeVector; }
            set { _eyeVector = value; }
        }
        public Vector NormalVector
        {
            get { return _normalVector; }
            set { _normalVector = value; }
        }
        public Vector ReflectVector
        {
            get { return _reflectVector; }
            set { _reflectVector = value; }
        }
        public bool Inside
        {
            get { return _inside; }
            set { _inside = value; }
        }
        public double N1
        {
            get { return _n1; }
            set { _n1 = value; }
        }
        public double N2
        {
            get { return _n2; }
            set { _n2 = value; }
        }

        public Comps() {}
    }
}
