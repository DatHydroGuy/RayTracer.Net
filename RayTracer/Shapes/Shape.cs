using System;
using System.Threading.Tasks;

namespace RayTracer.Shapes
{
    public abstract class Shape
    {
        protected readonly long Id;
        private Matrix _transform;
        private Matrix _transformInverse;

        public Point Origin { get; set; }

        public Matrix Transform
        {
            get => _transform;
            set { 
                _transform = value;
                _transformInverse = value.Inverse();
                }
        }

        public Matrix TransformInverse
        {
            get => _transformInverse;
            set { 
                _transformInverse = value;
                _transform = value.Inverse();
                }
        }

        public Material Material { get; set; }

        public Group Parent { get; set; }

        protected Shape()
        {
            Id = DateTime.Now.Ticks;
            Origin = new Point();
            Transform = Matrix.Identity(4);
            Material = new Material();
            Parent = null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Shape;

            return other != null &&
                Origin == other.Origin &&
                Transform == other.Transform &&
                Material == other.Material;
        }

        public abstract Shape Clone();

        public static bool operator==(Shape t1, Shape t2)
        {
            // If any nulls are passed in, then both arguments must be null for equality
            return t1?.Equals(t2) ?? ReferenceEquals(t2, null);
        }

        public static bool operator!=(Shape t1, Shape t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return Origin.GetHashCode() * 5 + Transform.GetHashCode() * 3 + Material.GetHashCode() * 2;
        }

        public override string ToString()
        {
            var parent = Parent == null ? "null" : Parent.Id.ToString();
            var material = Material == null ? "null\n" : Material.ToString();
            var transform = Transform == null ? "null\n" : Transform.ToString();
            return $"Type:{GetType()}\nId:{Id}\nOrigin:{Origin}Parent:{parent}\nMaterial:{material}Transform:{transform}";
        }

        public Intersection[] Intersects(Ray ray)
        {
            var localRay = ray.Transform(TransformInverse);
            return LocalIntersects(localRay);
        }

        public abstract Intersection[] LocalIntersects(Ray ray);

        public Vector NormalAt(Point worldPoint, Intersection intersect = null)
        {
            var objectPoint = WorldToObject(worldPoint);
            var objectNormal = LocalNormalAt(objectPoint, intersect);
            return NormalToWorld(objectNormal);
        }

        public abstract Vector LocalNormalAt(Point objectPoint, Intersection intersect = null);

        public abstract BoundingBox GetBoundingBox();

        public BoundingBox GetParentSpaceBoundingBox()
        {
            return GetBoundingBox().TransformBox(Transform);
        }

        public Point WorldToObject(Point point)
        {
            if (Parent != null)
            {
                point = Parent.WorldToObject(point);
            }

            return TransformInverse * point;
        }

        public Vector NormalToWorld(Vector normal)
        {
            // Convert local normal to a normal vector in the parent's object space
            normal = TransformInverse.Transpose() * normal;
            normal.W = 0;
            normal = normal.Normalise();

            if (Parent != null)
            {
                normal = Parent.NormalToWorld(normal);
            }

            return normal;
        }

        public bool Contains(Shape child)
        {
            var asGroup = this as Group;
            var asCsg = this as Csg;

            if(asCsg != null)
            {
                return (asCsg.Left.Contains(child) || asCsg.Right.Contains(child));
            }

            if (asGroup != null)
            {
                return asGroup.Contains(child);
            }

            return this == child;
        }

        public static void Divide(Shape shape, int threshold)
        {
            var asGroup = shape as Group;
            var asCsg = shape as Csg;

            if(asCsg != null)
            {
                var leftGroup = new Group();
                leftGroup.AddChildren(new[] {asCsg.Left});
                asCsg.Left = leftGroup;

                var rightGroup = new Group();
                rightGroup.AddChildren(new[] {asCsg.Right});
                asCsg.Right = rightGroup;

                Divide(asCsg.Left, threshold);
                Divide(asCsg.Right, threshold);
            }
            else if (asGroup != null)
            {
                if (threshold <= asGroup.Shapes.Count)
                {
                    Group left;
                    Group right;
                    asGroup.PartitionChildObjects(out left, out right);

                    if (left != null && left.Shapes.Count > 0)
                        asGroup.CreateSubgroup(left.Shapes);

                    if (right != null && right.Shapes.Count > 0)
                        asGroup.CreateSubgroup(right.Shapes);
                }

                Parallel.ForEach(asGroup.Shapes, childGroup =>
                {
                    Divide(childGroup, threshold);
                });
            }
        }
    }
}
