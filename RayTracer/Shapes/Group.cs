using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RayTracer
{
    public class Group: Shape
    {
        private List<Shape> _shapes;
        private BoundingBox _aabb;
        
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }
        public BoundingBox AABB
        {
            get { return _aabb; }
            set { _aabb = value; }
        }
        
        public Group(): base()
        {
            Shapes = new List<Shape>();
            AABB = null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Group;

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Group Clone()
        {
            var groupClone = new Group
            {
                Origin = Origin.Clone(),
                Parent = Parent,
                Transform = Transform.Clone()
            };
            groupClone.AddChildren(Shapes.Select(shape => shape.Clone()));
            groupClone.SetMaterial(Material.Clone());
            return groupClone;
        }

        public override string ToString()
        {
            var parent = Parent == null ? "null" : Parent._id.ToString();
            var currGroup = $"Type:{GetType()}, Origin:{Origin}Parent:{parent}\nMaterial:{Material}Transform:{Transform}Children:\n";
            var children = "";
            foreach (var shape in Shapes)
            {
                children += shape.ToString() + "\n";
            }

            return currGroup + children;
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            if (AABB == null)
                AABB = GetBoundingBox();

            if (AABB.Intersects(ray))
            {
                var result = new List<Intersection>();

                foreach (var shape in Shapes)
                {
                    result.AddRange(shape.Intersects(ray));
                }

                result.Sort(new System.Comparison<Intersection>((x, y) => x.T.CompareTo(y.T)));
                return result.ToArray();
            }
            else
            {
                return new Intersection[] {};
            }
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            throw new System.Exception("Cannot call LocalNormalAt() on an abstract Group object");
        }

        public override BoundingBox GetBoundingBox()
        {
            var box = new BoundingBox();

            Parallel.ForEach(Shapes, shape =>
            {
                var childBox = shape.GetParentSpaceBoundingBox();
                box.AddBox(childBox);
            });

            return box;
        }

        public void AddChildren(IEnumerable<Shape> s)
        {
            Shapes.AddRange(s);
            // foreach (var child in s)
            // {
            //     child.Parent = this;
            //     Shapes.Add(child);
            // }
            Parallel.ForEach(Shapes, child =>
            {
                child.Parent = this;
            });
            AABB = null;
        }

        public void AddChild(Shape s)
        {
            s.Parent = this;
            Shapes.Add(s);
            AABB = null;
        }

        public void SetMaterial(Material newMaterial)
        {
            Material = newMaterial;
            Parallel.ForEach(Shapes, child =>
            {
                if (child.GetType() == typeof(Group))
                {
                    ((Group)child).SetMaterial(newMaterial);
                }
                else
                {
                    child.Material = newMaterial;
                }
            });
        }

        public void PartitionChildObjects(out Group left, out Group right)
        {
            var aabb = GetBoundingBox();
            BoundingBox leftBB;
            BoundingBox rightBB;
            aabb.SplitBounds(out leftBB, out rightBB);

            left = new Group();
            var leftShapes = Shapes.AsParallel().Where(x => leftBB.ContainsBox(x.GetParentSpaceBoundingBox())).ToList();
            Shapes.RemoveAll(x => leftShapes.Contains(x));
            left.AddChildren(leftShapes);

            right = new Group();
            var rightShapes = Shapes.AsParallel().Where(x => rightBB.ContainsBox(x.GetParentSpaceBoundingBox())).ToList();
            Shapes.RemoveAll(x => rightShapes.Contains(x));
            right.AddChildren(rightShapes);
        }

        public void CreateSubgroup(IEnumerable<Shape> shapes)
        {
            var subGroup = new Group();
            subGroup.AddChildren(shapes);
            Shapes.RemoveAll(x => shapes.Contains(x));
            Shapes.Add(subGroup);
        }
    }
}
