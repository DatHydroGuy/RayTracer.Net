using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter06
    {
        public Chapter06()
        {
            var canvas = new Canvas(1000, 1000);
            var s = new Sphere();
            s.Material = new Material();
            s.Material.Colour = new Colour(0.3, 0, 1);
            var light = Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1));
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

                    if (xs.Length > 0)
                    {
                        var rayHit = Intersection.Hit(xs);
                        var hitPoint = r.Position(rayHit.T);
                        var hitNormal = rayHit.Obj.NormalAt(hitPoint);
                        var eyeVector = -r.Direction;
                        var hitColour = rayHit.Obj.Material.Lighting(s, light, hitPoint, eyeVector, hitNormal);
                        canvas.WritePixel(x, y, hitColour);
                    }
                }
            }

            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter06");
            Console.WriteLine("Chapter06 Complete");
        }
    }
}
