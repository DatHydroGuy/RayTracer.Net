using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter11Image
    {
        public Chapter11Image()
        {
            var floor = new Plane();
            floor.Transform = Transformations.Translation(0, -5.1, 0);
            var floorPattern = new CheckerPattern(Colour.BLACK, Colour.WHITE);
            floor.Material = new Material();
            floor.Material.Ambient = 0.9;
            floor.Material.Pattern = floorPattern;

            var glass = new Sphere();
            glass.Material = new Material();
            glass.Material.Colour = new Colour(0.05, 0.05, 0.1);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;

            var hollow = new Sphere();
            hollow.Transform = Transformations.Scaling(0.5, 0.5, 0.5);
            hollow.Material = new Material();
            hollow.Material.Colour = new Colour(0.1, 0.1, 0.1);
            hollow.Material.Diffuse = 0.1;
            hollow.Material.Shininess = 300;
            hollow.Material.Reflective = 1;
            hollow.Material.Transparency = 1;
            hollow.Material.RefractiveIndex = 1;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, glass, hollow});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(0, 10, 20), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 1000, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 2.5, 0), new Point(0, 0, 0), new Vector(0, 0, 1));

            var canvas = camera.Render(world, 5, 8, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter11Image");
            Console.WriteLine("Chapter11Image Complete");
        }
    }
}
