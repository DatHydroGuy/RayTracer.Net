using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter15Triangles
    {
        public Chapter15Triangles()
        {
            var floor = new Plane();
            floor.Transform =  Transformations.RotationY(-Math.PI / 4.0);
            var floorPattern = new CheckerPattern(new Colour(0.5, 0.5, 0.5), new Colour(0.8, 0.8, 0.8));
            floorPattern.Transform = Transformations.Scaling(0.5, 0.5, 0.5);
            floor.Material = new Material();
            floor.Material.Specular = 0;
            floor.Material.Pattern = floorPattern;

            var leftWall = new Plane();
            leftWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(-Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            leftWall.Material = new Material();
            leftWall.Material.Specular = 0;
            leftWall.Material.Pattern = floorPattern;

            var rightWall = new Plane();
            rightWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            rightWall.Material = new Material();
            rightWall.Material.Specular = 0;
            rightWall.Material.Pattern = floorPattern;

            var tPoint1 = new Point(-1, 0, -1.5);
            var tPoint2 = new Point(0.5, 2, -1.6);
            var tPoint3 = new Point(2, 0, -1.7);
            var triangle = new Triangle(tPoint1, tPoint2, tPoint3);
            triangle.Material = new Material();
            triangle.Material.Colour = new Colour(0.05, 0.05, 0.55);
            triangle.Material.Diffuse = 0.1;
            triangle.Material.Shininess = 300;
            triangle.Material.Reflective = 0.5;
            triangle.Material.Transparency = 0.5;
            triangle.Material.RefractiveIndex = 1.52;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, triangle});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter15Triangles");
            Console.WriteLine("Chapter15Triangles Complete");
        }
    }
}
