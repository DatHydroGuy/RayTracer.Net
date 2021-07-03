using System;
using RayTracer;
using RayTracer.Shapes;

namespace RayTracerApp
{
    public class Chapter05
    {
        public Chapter05()
        {
            var canvas = new Canvas(1000, 1000);
            var sphereColour = new Colour(0.3, 0, 1);
            var s = new Sphere();
            var rayOrigin = new Point(0, 0, -5);
            var wallZ = 10;
            var wallWidth = 7;
            var wallHeight = 7;
            var pixelWidth = (double)wallWidth / canvas.Width;
            var pixelHeight = (double)wallHeight / canvas.Height;
            var halfWidth = 7 * 0.5;
            var halfHeight = 7 * 0.5;

            for (int y = 0; y < canvas.Height; y++)
            {
                var worldY = halfHeight - pixelHeight * y;
                for (int x = 0; x < canvas.Width; x++)
                {
                    var worldX = -halfWidth + pixelWidth * x;
                    var pointOnWall = new Point(worldX, worldY, wallZ);
                    var rayDirection = pointOnWall - rayOrigin;
                    var r = new Ray(rayOrigin, rayDirection.Normalise());
                    var xs = s.Intersects(r);

                    if (Intersection.Hit(xs) != null)
                    {
                        canvas.WritePixel(x, y, sphereColour);
                    }
                }
            }

            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter05");
            Console.WriteLine("Chapter05 Complete");
        }
    }
}
