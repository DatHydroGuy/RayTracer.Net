using System;
using System.Collections.Generic;

namespace RayTracer.Shapes
{
    public enum CsgOperation
    {
        Union,
        Intersect,
        Difference,
        None
    }

    public class Csg: Group
    {
        public CsgOperation Operation { get; }

        public Shape Left { get; set; }

        public Shape Right { get; set; }

        public Csg(CsgOperation operation, Shape shape1, Shape shape2)
        {
            Operation = operation is CsgOperation.Union or CsgOperation.Intersect or CsgOperation.Difference ? operation : CsgOperation.None;
            AddChild(shape1);
            Left = shape1.Clone();
            AddChild(shape2);
            Right = shape2.Clone();
        }

        public override bool Equals(object obj)
        {
            return obj is Csg other && base.Equals(other) && Operation == other.Operation && Left == other.Left && Right == other.Right;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 2 * Operation.GetHashCode() + 3 * Left.GetHashCode() + 5 * Right.GetHashCode();
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            Aabb ??= GetBoundingBox();

            if (Aabb.Intersects(ray))
            {
                var leftIntersect = Left.Intersects(ray);
                var rightIntersect = Right.Intersects(ray);
                var combined = new List<Intersection>(leftIntersect);
                combined.AddRange(rightIntersect);
                combined.Sort((x, y) => x.T.CompareTo(y.T));
                return FilterIntersections(combined.ToArray());
            }
            else
            {
                return Array.Empty<Intersection>();
            }
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            throw new NotImplementedException();    // Yes, this should throw an error if execution gets here!
        }

        public override BoundingBox GetBoundingBox()
        {
            var asGroup = this as Group;
            var bounds = new BoundingBox();
            foreach (var shape in asGroup.Shapes)
            {
                bounds.AddBox(shape.GetParentSpaceBoundingBox());
            }
            return bounds;
        }

        public override Csg Clone()
        {
            return new Csg(Operation, Left, Right)
            {
                Origin = Origin.Clone(),
                Material = Material.Clone(),
                Transform = Transform.Clone(),
                Parent = Parent
            };
        }

        public override string ToString()
        {
            var op = Operation switch
            {
                CsgOperation.Union => "Union",
                CsgOperation.Difference => "Difference",
                CsgOperation.Intersect => "Intersect",
                _ => "None"
            };
            var baseStr = base.ToString();
            var materialIndex = baseStr.IndexOf("\nMaterial:", StringComparison.Ordinal);
            return $"[{baseStr[..materialIndex]}\nOp:{op}{baseStr[materialIndex..]}]";
        }

        public bool IntersectionAllowed(bool isLeftObjectHit, bool isInsideLeftObject, bool isInsideRightObject)
        {
            return Operation switch
            {
                CsgOperation.Union =>
                    // If an intersect is on the left object, it must not be inside the right object.
                    // If an intersect is on the right object, it must not be inside the left object.
                    isLeftObjectHit && !isInsideRightObject || !isLeftObjectHit && !isInsideLeftObject,
                CsgOperation.Intersect =>
                    // If an intersect is on the left object, it must be inside the right object.
                    // If an intersect is on the right object, it must be inside the left object.
                    isLeftObjectHit && isInsideRightObject || !isLeftObjectHit && isInsideLeftObject,
                CsgOperation.Difference =>
                    // If an intersect is on the left object, it must not be inside the right object.
                    // If an intersect is on the right object, it must be inside the left object.
                    isLeftObjectHit && !isInsideRightObject || !isLeftObjectHit && isInsideLeftObject,
                CsgOperation.None => false,
                _ => false
            };
        }

        public Intersection[] FilterIntersections(Intersection[] intersections)
        {
            // Start outside of both child objects
            var inLeft = false;
            var inRight = false;

            var result = new List<Intersection>();

            foreach (var i in intersections)
            {
                // If i.Obj is part of the Left object, then set inLeft to true
                var isLeftObjHit = Left.Contains(i.Obj);

                if (IntersectionAllowed(isLeftObjHit, inLeft, inRight))
                {
                    result.Add(i);
                }

                // Depending on the object which was hit, toggle inLeft or inRight
                if (isLeftObjHit)
                {
                    inLeft = !inLeft;
                }
                else
                {
                    inRight = !inRight;
                }
            }

            return result.ToArray();
        }
    }
}
