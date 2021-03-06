namespace RayTracer
{
    public class Triangle: Shape
    {
        private Point _vertex1;
        private Point _vertex2;
        private Point _vertex3;
        private Vector _edge1;
        private Vector _edge2;
        private Vector _normal;
        
        public Point Vertex1
        {
            get { return _vertex1; }
            set { _vertex1 = value; }
        }
        public Point Vertex2
        {
            get { return _vertex2; }
            set { _vertex2 = value; }
        }
        public Point Vertex3
        {
            get { return _vertex3; }
            set { _vertex3 = value; }
        }
        public Vector Edge1
        {
            get { return _edge1; }
            set { _edge1 = value; }
        }
        public Vector Edge2
        {
            get { return _edge2; }
            set { _edge2 = value; }
        }
        public Vector Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }
        
        public Triangle(Point p1, Point p2, Point p3): base()
        {
            Update(p1, p2, p3);
        }
        
        public void Update(Point p1, Point p2, Point p3)
        {
            Vertex1 = p1;
            Vertex2 = p2;
            Vertex3 = p3;
            Edge1 = p2 - p1;
            Edge2 = p3 - p1;
            Normal = Edge2.Cross(Edge1).Normalise();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Triangle;

            return base.Equals(other) &&
                Vertex1 == other.Vertex1 &&
                Vertex2 == other.Vertex2 &&
                Vertex3 == other.Vertex3;
        }

        public override int GetHashCode()
        {
            return (int)(base.GetHashCode() + 2 * Vertex1.GetHashCode() + 3 * Vertex2.GetHashCode() + 5 * Vertex3.GetHashCode());
        }

        public override Triangle Clone()
        {
            return new Triangle(Vertex1, Vertex2, Vertex3)
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
            return $"[{baseStr.Substring(0, materialIndex)}\nV1:{Vertex1}V2:{Vertex2}V3:{Vertex3}{baseStr.Substring(materialIndex + 1)}]";
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            var rayCrossEdge2 = ray.Direction.Cross(Edge2);
            var determinant = rayCrossEdge2.Dot(Edge1);
            if (Utilities.AlmostEqual(0, determinant))
            {
                return new Intersection[] {};
            }

            var f = 1.0 / determinant;
            var vertex1ToOrigin = ray.Origin - Vertex1;
            var u = f * vertex1ToOrigin.Dot(rayCrossEdge2);
            if (u < 0 || u > 1)
            {
                return new Intersection[] {};
            }

            var originCrossEdge1 = vertex1ToOrigin.Cross(Edge1);
            var v = f * ray.Direction.Dot(originCrossEdge1);
            if (v < 0 || (u + v) > 1)
            {
                return new Intersection[] {};
            }

            var t = f * Edge2.Dot(originCrossEdge1);
            return new Intersection[] { new Intersection(t, this) };
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            return Normal;
        }

        public override BoundingBox GetBoundingBox()
        {
            var aabb = new BoundingBox();
            aabb.AddPoint(Vertex1);
            aabb.AddPoint(Vertex2);
            aabb.AddPoint(Vertex3);
            return aabb;
        }
    }
}
