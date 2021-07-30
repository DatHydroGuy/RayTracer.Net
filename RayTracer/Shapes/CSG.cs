using System;
using System.Collections.Generic;

namespace RayTracer
{
    public enum CsgOperation
    {
        Union,
        Intersect,
        Difference,
        None
    }

    public class CSG: Group
    {
        private CsgOperation _operation;
        private Shape _left;
        private Shape _right;

        public CsgOperation Operation
        {
            get { return _operation; }
            set { _operation = value; }
        }
        public Shape Left
        {
            get { return _left; }
            set { _left = value; }
        }
        public Shape Right
        {
            get { return _right; }
            set { _right = value; }
        }
        
        public CSG(CsgOperation operation, Shape shape1, Shape shape2): base()
        {
            Operation = operation is CsgOperation.Union or CsgOperation.Intersect or CsgOperation.Difference ? operation : CsgOperation.None;
            AddChild(shape1);
            Left = shape1.Clone();
            AddChild(shape2);
            Right = shape2.Clone();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CSG;

            return base.Equals(other) &&
                Operation == other.Operation &&
                Left == other.Left &&
                Right == other.Right;
        }

        public override int GetHashCode()
        {
            return (int)(base.GetHashCode() + 2 * Operation.GetHashCode() + 3 * Left.GetHashCode() + 5 * Right.GetHashCode());
        }

        public override Intersection[] LocalIntersects(Ray ray)
        {
            if (AABB == null)
                AABB = GetBoundingBox();

            if (AABB.Intersects(ray))
            {
                var leftIntersect = Left.Intersects(ray);
                var rightIntersect = Right.Intersects(ray);
                var combined = new List<Intersection>(leftIntersect);
                combined.AddRange(rightIntersect);
                combined.Sort(new Comparison<Intersection>((x, y) => x.T.CompareTo(y.T)));
                return FilterIntersections(combined.ToArray());
            }
            else
            {
                return new Intersection[] {};
            }
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            throw new NotImplementedException();
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

        public override CSG Clone()
        {
            return new CSG(Operation, Left, Right)
            {
                Origin = Origin.Clone(),
                Material = Material.Clone(),
                Transform = Transform.Clone(),
                Parent = Parent
            };
        }

        public override string ToString()
        {
            string op = "";
            switch (Operation)
            {
                case CsgOperation.Union:
                    op = "Union";
                    break;
                case CsgOperation.Difference:
                    op = "Difference";
                    break;
                case CsgOperation.Intersect:
                    op = "Intersect";
                    break;
                default:
                    op = "None";
                    break;
            }
            // var left = Left == null ? "null" : Left.ToString();
            // var right = Right == null ? "null" : Right.ToString();
            var baseStr = base.ToString();
            var materialIndex = baseStr.IndexOf("\nMaterial:");
            return $"[{baseStr.Substring(0, materialIndex)}\nOp:{op}{baseStr.Substring(materialIndex)}]";
        }

        public bool IntersectionAllowed(bool isLeftObjectHit, bool isInsideLeftObject, bool isInsideRightObject)
        {
            switch (Operation)
            {
                case CsgOperation.Union:
                    // If an intersect is on the left object, it must not be inside the right object.
                    // If an intersect is on the right object, it must not be inside the left object.
                    return (isLeftObjectHit && !isInsideRightObject) || (!isLeftObjectHit && !isInsideLeftObject);
                case CsgOperation.Intersect:
                    // If an intersect is on the left object, it must be inside the right object.
                    // If an intersect is on the right object, it must be inside the left object.
                    return (isLeftObjectHit && isInsideRightObject) || (!isLeftObjectHit && isInsideLeftObject);
                case CsgOperation.Difference:
                    // If an intersect is on the left object, it must not be inside the right object.
                    // If an intersect is on the right object, it must be inside the left object.
                    return (isLeftObjectHit && !isInsideRightObject) || (!isLeftObjectHit && isInsideLeftObject);
                case CsgOperation.None:
                    return false;
                default:
                    return false;
            }
        }

        public Intersection[] FilterIntersections(Intersection[] intersections)
        {
            // Start outside of both child objects
            bool inLeft = false;
            bool inRight = false;

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
