using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracer.Shapes
{
    public class Group: Shape
    {
        public List<Shape> Shapes { get; set; }

        public BoundingBox Aabb { get; set; }

        public Group()
        {
            Shapes = new List<Shape>();
            Aabb = null;
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
            var parent = Parent == null ? "null" : Parent.Id.ToString();
            var currGroup = $"Type:{GetType()}, Origin:{Origin}Parent:{parent}\nMaterial:{Material}Transform:{Transform}Children:\n";
            var children = "";
            foreach (var shape in Shapes)
            {
                children += shape + "\n";
            }

            return currGroup + children;
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            Aabb ??= GetBoundingBox();

            if (Aabb.Intersects(ray))
            {
                var result = new List<Intersection>();

                foreach (var shape in Shapes)
                {
                    result.AddRange(shape.Intersects(ray));
                }

                result.Sort((x, y) => x.T.CompareTo(y.T));
                return result.ToArray();
            }
            else
            {
                return System.Array.Empty<Intersection>();
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
            Parallel.ForEach(Shapes, child =>
            {
                child.Parent = this;
            });
            Aabb = null;
        }

        public void AddChild(Shape s)
        {
            s.Parent = this;
            Shapes.Add(s);
            Aabb = null;
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
            aabb.SplitBounds(out var leftBb, out var rightBb);

            left = new Group();
            var leftShapes = Shapes.AsParallel().Where(x => leftBb.ContainsBox(x.GetParentSpaceBoundingBox())).ToList();
            Shapes.RemoveAll(x => leftShapes.Contains(x));
            left.AddChildren(leftShapes);

            right = new Group();
            var rightShapes = Shapes.AsParallel().Where(x => rightBb.ContainsBox(x.GetParentSpaceBoundingBox())).ToList();
            Shapes.RemoveAll(x => rightShapes.Contains(x));
            right.AddChildren(rightShapes);
        }

        public void CreateSubgroup(IEnumerable<Shape> shapes)
        {
            var subGroup = new Group();
            IEnumerable<Shape> shapesArray = shapes as Shape[] ?? shapes.ToArray();
            subGroup.AddChildren(shapesArray);
            Shapes.RemoveAll(x => shapesArray.Contains(x));
            Shapes.Add(subGroup);
        }
    }
}
