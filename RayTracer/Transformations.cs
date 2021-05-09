using System;

namespace RayTracer
{
    public static class Transformations
    {
        public static Matrix Translation(double x, double y, double z)
        {
            var result = Matrix.Identity(4);
            result.Data[0, 3] = x;
            result.Data[1, 3] = y;
            result.Data[2, 3] = z;
            return result;
        }

        public static Matrix Scaling(double x, double y, double z)
        {
            var result = Matrix.Identity(4);
            result.Data[0, 0] = x;
            result.Data[1, 1] = y;
            result.Data[2, 2] = z;
            return result;
        }

        public static Matrix RotationX(double radians)
        {
            var result = Matrix.Identity(4);
            result.Data[1, 1] = Math.Cos(radians);
            result.Data[1, 2] = -Math.Sin(radians);
            result.Data[2, 1] = Math.Sin(radians);
            result.Data[2, 2] = Math.Cos(radians);
            return result;
        }

        public static Matrix RotationY(double radians)
        {
            var result = Matrix.Identity(4);
            result.Data[0, 0] = Math.Cos(radians);
            result.Data[0, 2] = Math.Sin(radians);
            result.Data[2, 0] = -Math.Sin(radians);
            result.Data[2, 2] = Math.Cos(radians);
            return result;
        }

        public static Matrix RotationZ(double radians)
        {
            var result = Matrix.Identity(4);
            result.Data[0, 0] = Math.Cos(radians);
            result.Data[0, 1] = -Math.Sin(radians);
            result.Data[1, 0] = Math.Sin(radians);
            result.Data[1, 1] = Math.Cos(radians);
            return result;
        }

        public static Matrix Shearing(double xY, double xZ, double yX, double yZ, double zX, double zY)
        {
            var result = Matrix.Identity(4);
            result.Data[0, 1] = xY;
            result.Data[0, 2] = xZ;
            result.Data[1, 0] = yX;
            result.Data[1, 2] = yZ;
            result.Data[2, 0] = zX;
            result.Data[2, 1] = zY;
            return result;
        }

        public static Matrix ViewTransform(Point from, Point to, Vector up)
        {
            // Get the true up vector from the given one
            var forward = (to - from).Normalise();
            var upNormal = up.Normalise();
            var left = forward.Cross(upNormal);
            var trueUp = left.Cross(forward);

            var orientation = Matrix.Identity(4);
            orientation.Data[0, 0] = left.X;
            orientation.Data[0, 1] = left.Y;
            orientation.Data[0, 2] = left.Z;
            orientation.Data[1, 0] = trueUp.X;
            orientation.Data[1, 1] = trueUp.Y;
            orientation.Data[1, 2] = trueUp.Z;
            orientation.Data[2, 0] = -forward.X;
            orientation.Data[2, 1] = -forward.Y;
            orientation.Data[2, 2] = -forward.Z;

            return orientation * Transformations.Translation(-from.X, -from.Y, -from.Z);
        }
    }
}
