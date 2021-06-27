using System;

namespace RayTracer
{
    public class BoundingBox
    {
        private Point _minPoint;
        private Point _maxPoint;
        private Matrix _transform;
        private Matrix _transformInverse;

        public Point MinPoint
        {
            get { return _minPoint; }
            set { _minPoint = value; }
        }
        public Point MaxPoint
        {
            get { return _maxPoint; }
            set { _maxPoint = value; }
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

        public override string ToString()
        {
            var transform = Transform == null ? "null\n" : Transform.ToString();
            return $"[Type:{GetType()}\nMinPoint:{MinPoint}MaxPoint:{MaxPoint}Transform:{transform}]";
        }

        public BoundingBox(Point minPoint = null, Point maxPoint = null)
        {
            MinPoint = minPoint == null ? new Point(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity) : minPoint;
            MaxPoint = maxPoint == null ? new Point(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity) : maxPoint;
            Transform = Matrix.Identity(4);
        }

        public void AddPoint(Point newPoint)
        {
            MinPoint.X = newPoint.X < MinPoint.X ? newPoint.X : MinPoint.X;
            MinPoint.Y = newPoint.Y < MinPoint.Y ? newPoint.Y : MinPoint.Y;
            MinPoint.Z = newPoint.Z < MinPoint.Z ? newPoint.Z : MinPoint.Z;
            MaxPoint.X = newPoint.X > MaxPoint.X ? newPoint.X : MaxPoint.X;
            MaxPoint.Y = newPoint.Y > MaxPoint.Y ? newPoint.Y : MaxPoint.Y;
            MaxPoint.Z = newPoint.Z > MaxPoint.Z ? newPoint.Z : MaxPoint.Z;
        }

        public void AddBox(BoundingBox newBox)
        {
            AddPoint(newBox.MinPoint);
            AddPoint(newBox.MaxPoint);
        }

        public bool ContainsPoint(Point testPoint)
        {
            return MinPoint.X <= testPoint.X && testPoint.X <= MaxPoint.X &&
                   MinPoint.Y <= testPoint.Y && testPoint.Y <= MaxPoint.Y &&
                   MinPoint.Z <= testPoint.Z && testPoint.Z <= MaxPoint.Z;
        }

        public bool ContainsBox(BoundingBox testBox)
        {
            return MinPoint.X <= testBox.MinPoint.X && testBox.MaxPoint.X <= MaxPoint.X &&
                   MinPoint.Y <= testBox.MinPoint.Y && testBox.MaxPoint.Y <= MaxPoint.Y &&
                   MinPoint.Z <= testBox.MinPoint.Z && testBox.MaxPoint.Z <= MaxPoint.Z;
        }

        public BoundingBox TransformBox(Matrix transform)
        {
            var points = new Point[] {MinPoint,
                                      new Point(MinPoint.X, MinPoint.Y, MaxPoint.Z),
                                      new Point(MinPoint.X, MaxPoint.Y, MinPoint.Z),
                                      new Point(MinPoint.X, MaxPoint.Y, MaxPoint.Z),
                                      new Point(MaxPoint.X, MinPoint.Y, MinPoint.Z),
                                      new Point(MaxPoint.X, MinPoint.Y, MaxPoint.Z),
                                      new Point(MaxPoint.X, MaxPoint.Y, MinPoint.Z),
                                      MaxPoint};

            var newBox = new BoundingBox();
            
            foreach (var pt in points)
            {
                newBox.AddPoint(transform * pt);
            }

            return newBox;
        }

        public bool Intersects(Ray ray)
        {
            double xTMin, xTMax, yTMin, yTMax, zTMin, zTMax;
            CheckAxis(ray.Origin.X, ray.Direction.X, MinPoint.X, MaxPoint.X, out xTMin, out xTMax);
            CheckAxis(ray.Origin.Y, ray.Direction.Y, MinPoint.Y, MaxPoint.Y, out yTMin, out yTMax);
            CheckAxis(ray.Origin.Z, ray.Direction.Z, MinPoint.Z, MaxPoint.Z, out zTMin, out zTMax);
            var tMin = Math.Max(xTMin, Math.Max(yTMin, zTMin));
            var tMax = Math.Min(xTMax, Math.Min(yTMax, zTMax));
            if (tMin > tMax)
            {
                return false;
                // return new Intersection[] {};
            }
            return true;
            // return new Intersection[2] {new Intersection(tMin, this), new Intersection(tMax, this)};
        }

        public void SplitBounds(out BoundingBox left, out BoundingBox right)
        {
            var xDim = MaxPoint.X - MinPoint.X;
            var yDim = MaxPoint.Y - MinPoint.Y;
            var zDim = MaxPoint.Z - MinPoint.Z;
            var bigDim = Math.Max(xDim, Math.Max(yDim, zDim));
            Point midMin;
            Point midMax;

            if (bigDim == xDim)
            {
                midMin = new Point(MinPoint.X + xDim / 2.0, MinPoint.Y, MinPoint.Z);
                midMax = new Point(MinPoint.X + xDim / 2.0, MaxPoint.Y, MaxPoint.Z);
            }
            else if (bigDim == yDim)
            {
                midMin = new Point(MinPoint.X, MinPoint.Y + yDim / 2.0, MinPoint.Z);
                midMax = new Point(MaxPoint.X, MinPoint.Y + yDim / 2.0, MaxPoint.Z);
            }
            else
            {
                midMin = new Point(MinPoint.X, MinPoint.Y, MinPoint.Z + zDim / 2.0);
                midMax = new Point(MaxPoint.X, MaxPoint.Y, MinPoint.Z + zDim / 2.0);
            }

            left = new BoundingBox(MinPoint, midMax);
            right = new BoundingBox(midMin, MaxPoint);
        }

        private void CheckAxis(double origin, double direction, double axisMin, double axisMax, out double foundMin, out double foundMax)
        {
            double tMin, tMax;
            double tMinNumerator = axisMin - origin;
            double tMaxNumerator = axisMax - origin;

            if (Math.Abs(direction) >= Utilities.EPSILON)
            {
                tMin = tMinNumerator / direction;
                tMax = tMaxNumerator / direction;
            }
            else
            {
                tMin = tMinNumerator * double.PositiveInfinity;
                tMax = tMaxNumerator * double.PositiveInfinity;
            }

            if (tMin > tMax)
            {
                foundMin = tMax;
                foundMax = tMin;
            }
            else
            {
                foundMin = tMin;
                foundMax = tMax;
            }
        }
    }
}
