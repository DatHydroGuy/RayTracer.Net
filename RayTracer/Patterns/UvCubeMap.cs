using System;

namespace RayTracer
{
    public class UvCubeMap: UvPattern
    {
        public UvPattern LeftFace { get; set; }
        public UvPattern RightFace { get; set; }
        public UvPattern FrontFace { get; set; }
        public UvPattern BackFace { get; set; }
        public UvPattern UpperFace { get; set; }
        public UvPattern LowerFace { get; set; }
        
        public UvCubeMap(): base(Colour.BLACK, Colour.WHITE)
        {}

        public UvCubeMap(UvPattern leftFace, UvPattern frontFace, UvPattern rightFace,
            UvPattern backFace, UvPattern upperFace, UvPattern lowerFace) : this()
        {
            LeftFace = leftFace;
            RightFace = rightFace;
            FrontFace = frontFace;
            BackFace = backFace;
            UpperFace = upperFace;
            LowerFace = lowerFace;
        }

        public override Colour ColourAtPoint(Point targetPoint)
        {
            double u;
            double v;
            
            var face = FaceFromPoint(targetPoint);

            switch (face)
            {
                case CubeFace.Left:
                    (u, v) = UvMapCubeLeft(targetPoint);
                    return LeftFace.ColourAtUv(u, v);
                case CubeFace.Right:
                    (u, v) = UvMapCubeRight(targetPoint);
                    return RightFace.ColourAtUv(u, v);
                case CubeFace.Front:
                    (u, v) = UvMapCubeFront(targetPoint);
                    return FrontFace.ColourAtUv(u, v);
                case CubeFace.Back:
                    (u, v) = UvMapCubeBack(targetPoint);
                    return BackFace.ColourAtUv(u, v);
                case CubeFace.Upper:
                    (u, v) = UvMapCubeUpper(targetPoint);
                    return UpperFace.ColourAtUv(u, v);
                case CubeFace.Lower:
                    (u, v) = UvMapCubeLower(targetPoint);
                    return LowerFace.ColourAtUv(u, v);
                default:
                    throw new SystemException($"Unknown cube face: {face}");
            }
        }

        public override UvCubeMap Clone()
        {
            return new UvCubeMap(LeftFace.Clone(), FrontFace.Clone(), RightFace.Clone(), BackFace.Clone(), UpperFace.Clone(), LowerFace.Clone());
        }

        public override string ToString()
        {
            return $"[{base.ToString()}, LeftFace: {LeftFace}, RightFace: {RightFace}, FrontFace: {FrontFace}, BackFace: {BackFace}, UpperFace: {UpperFace}, LowerFace: {LowerFace}]\n";
        }

        public static CubeFace FaceFromPoint(Point point)
        {
            var absX = Math.Abs(point.X);
            var absY = Math.Abs(point.Y);
            var absZ = Math.Abs(point.Z);
            var coord = Math.Max(absX, Math.Max(absY, absZ));

            if (Utilities.AlmostEqual(coord, point.X))
                return CubeFace.Right;
            if (Utilities.AlmostEqual(coord, -point.X))
                return CubeFace.Left;
            if (Utilities.AlmostEqual(coord, point.Y))
                return CubeFace.Upper;
            if (Utilities.AlmostEqual(coord, -point.Y))
                return CubeFace.Lower;
            return Utilities.AlmostEqual(coord, point.Z) ? CubeFace.Front : CubeFace.Back;
        }

        public static (double, double) UvMapCubeFront(Point point)
        {
            var u = (point.X + 1) % 2 * 0.5;
            var v = (point.Y + 1) % 2 * 0.5;
            return (u, v);
        }

        public static (double, double) UvMapCubeBack(Point point)
        {
            var u = (1 - point.X) % 2 * 0.5;
            var v = (point.Y + 1) % 2 * 0.5;
            return (u, v);
        }

        public static (double, double) UvMapCubeLeft(Point point)
        {
            var u = (point.Z + 1) % 2 * 0.5;
            var v = (point.Y + 1) % 2 * 0.5;
            return (u, v);
        }

        public static (double, double) UvMapCubeRight(Point point)
        {
            var u = (1 - point.Z) % 2 * 0.5;
            var v = (point.Y + 1) % 2 * 0.5;
            return (u, v);
        }

        public static (double, double) UvMapCubeUpper(Point point)
        {
            var u = (point.X + 1) % 2 * 0.5;
            var v = (1 - point.Z) % 2 * 0.5;
            return (u, v);
        }

        public static (double, double) UvMapCubeLower(Point point)
        {
            var u = (point.X + 1) % 2 * 0.5;
            var v = (point.Z + 1) % 2 * 0.5;
            return (u, v);
        }
    }
}
