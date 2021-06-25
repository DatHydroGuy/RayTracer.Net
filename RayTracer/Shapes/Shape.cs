using System;
using System.Threading.Tasks;

namespace RayTracer
{
    public abstract class Shape
    {
        protected long _id;
        private Point _origin;
        private Matrix _transform;
        private Matrix _transformInverse;
        private Material _material;
        private Group _parent;
        
        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        public Matrix Transform
        {
            get { return _transform; }
            set { 
                _transform = value;
                _transformInverse = value.Inverse();
                }
        }
        public Matrix TransformInverse
        {
            get { return _transformInverse; }
            set { 
                _transformInverse = value;
                _transform = value.Inverse();
                }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public Group Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Shape()
        {
            _id = DateTime.Now.Ticks;
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
            if(object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(Shape t1, Shape t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return (int)(Origin.GetHashCode() * 5 + Transform.GetHashCode() * 3 + Material.GetHashCode() * 2);
        }

        public override string ToString()
        {
            var parent = Parent == null ? "null\n" : Parent.ToString();
            var material = Material == null ? "null\n" : Material.ToString();
            var transform = Transform == null ? "null\n" : Transform.ToString();
            return $"Type:{GetType()}\nId:{_id}\nOrigin:{Origin}Parent:{parent}Material:{material}Transform:{transform}";
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
            var asCSG = this as CSG;

            if(asCSG != null)
            {
                return (asCSG.Left.Contains(child) || asCSG.Right.Contains(child));
            }
            else if (asGroup != null)
            {
                return asGroup.Contains(child);
            }
            else
            {
                return this == child;
            }
        }

        public static void Divide(Shape shape, int threshold)
        {
            var asGroup = shape as Group;
            var asCSG = shape as CSG;

            if(asCSG != null)
            {
                var leftGroup = new Group();
                leftGroup.AddChildren(new Shape[] {asCSG.Left});
                asCSG.Left = leftGroup;

                var rightGroup = new Group();
                rightGroup.AddChildren(new Shape[] {asCSG.Right});
                asCSG.Right = rightGroup;

                Shape.Divide(asCSG.Left, threshold);
                Shape.Divide(asCSG.Right, threshold);
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
                    Shape.Divide(childGroup, threshold);
                });
            }
        }
    }
}
