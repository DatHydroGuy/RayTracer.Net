using System;

namespace RayTracer.Shapes
{
    public class SmoothTriangle: Shape
    {
        public Point Vertex1 { get; set; }

        public Point Vertex2 { get; set; }

        public Point Vertex3 { get; set; }

        public Vector Normal1 { get; set; }

        public Vector Normal2 { get; set; }

        public Vector Normal3 { get; set; }

        public Vector Edge1 { get; set; }

        public Vector Edge2 { get; set; }

        public SmoothTriangle(Point p1, Point p2, Point p3, Vector n1, Vector n2, Vector n3)
        {
            UpdatePoints(p1, p2, p3);
            UpdateNormals(n1, n2, n3);
        }

        public void UpdatePoints(Point p1, Point p2, Point p3)
        {
            Vertex1 = p1;
            Vertex2 = p2;
            Vertex3 = p3;
            Edge1 = p2 - p1;
            Edge2 = p3 - p1;
        }

        public void UpdateNormals(Vector n1, Vector n2, Vector n3)
        {
            Normal1 = n1;
            Normal2 = n2;
            Normal3 = n3;
        }

        public override bool Equals(object obj)
        {
            return obj is SmoothTriangle other && base.Equals(other) && Vertex1 == other.Vertex1 && Vertex2 == other.Vertex2 && Vertex3 == other.Vertex3 && Normal1 == other.Normal1 && Normal2 == other.Normal2 && Normal3 == other.Normal3;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 2 * Vertex1.GetHashCode() + 3 * Vertex2.GetHashCode() + 5 * Vertex3.GetHashCode() +
                   7 * Normal1.GetHashCode() + 11 * Normal2.GetHashCode() + 13 * Normal3.GetHashCode();
        }

        public override SmoothTriangle Clone()
        {
            return new SmoothTriangle(Vertex1, Vertex2, Vertex3, Normal1, Normal2, Normal3)
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
            var materialIndex = baseStr.IndexOf("\nMaterial:", StringComparison.Ordinal);
            return $"[{baseStr[..materialIndex]}\nV1:{Vertex1}V2:{Vertex2}V3:{Vertex3}N1:{Normal1}N2:{Normal2}N3:{Normal3}{baseStr[(materialIndex + 1)..]}]";
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            var rayCrossEdge2 = ray.Direction.Cross(Edge2);
            var determinant = rayCrossEdge2.Dot(Edge1);
            if (Utilities.AlmostEqual(0, determinant))
            {
                return Array.Empty<Intersection>();
            }

            var f = 1.0 / determinant;
            var vertex1ToOrigin = ray.Origin - Vertex1;
            var u = f * vertex1ToOrigin.Dot(rayCrossEdge2);
            if (u is < 0 or > 1)
            {
                return Array.Empty<Intersection>();
            }

            var originCrossEdge1 = vertex1ToOrigin.Cross(Edge1);
            var v = f * ray.Direction.Dot(originCrossEdge1);
            if (v < 0 || u + v > 1)
            {
                return Array.Empty<Intersection>();
            }

            var t = f * Edge2.Dot(originCrossEdge1);
            return new[] {Intersection.IntersectionWithUV(t, this, u, v)};
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            if (intersect != null)
                return Normal2 * intersect.U + Normal3 * intersect.V + Normal1 * (1 - intersect.U - intersect.V);

            throw new ArgumentNullException(nameof(intersect), "the intersection cannot be null");
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
