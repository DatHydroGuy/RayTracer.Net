using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter12
    {
        public Chapter12()
        {
            var floor = new Plane();
            var floorPattern = new CheckerPattern(new Colour(1, 0.3, 0.7), new Colour(0.3, 1, 0.7));
            floorPattern.Transform = Transformations.Translation(0.01, 0.01, 0.01);
            floor.Material = new Material();
            floor.Material.Specular = 0;
            floor.Material.Pattern = floorPattern;

            var sky = new Plane();
            sky.Transform = Transformations.Translation(0, 5, 0);
            var skyPattern = new DoubleGradientPattern(new Colour(0.8, 0.8, 1), new Colour(0.1, 0.1, 1));
            skyPattern.Transform = Transformations.Scaling(100000, 100000, 100000);
            sky.Material = new Material();
            sky.Material.Specular = 0;
            sky.Material.Ambient = 0.9;
            sky.Material.Pattern = skyPattern;

            var glass = new Cube();
            glass.Transform = Transformations.Translation(1.61, 1.0001, 0.01) * Transformations.RotationY(Math.PI / 4.5);
            glass.Material = new Material();
            glass.Material.Colour = new Colour(0.05, 0.05, 0.55);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;

            var hollow = new Cube();
            hollow.Transform = Transformations.Translation(1.61, 1, 0.01) * Transformations.Scaling(0.75, 0.75, 0.75) * Transformations.RotationY(Math.PI / 4.5);
            hollow.Material = new Material();
            hollow.Material.Colour = new Colour(0.05, 0.05, 0.05);
            hollow.Material.Diffuse = 0.1;
            hollow.Material.Shininess = 300;
            hollow.Material.Reflective = 1;
            hollow.Material.Transparency = 1;
            hollow.Material.RefractiveIndex = 1;

            var solid = new Cube();
            solid.Transform = Transformations.Translation(-1.61, 1, 0.01) * Transformations.RotationY(-Math.PI / 4.5);
            solid.Material = new Material();
            solid.Material.Colour = new Colour(0.05, 0.05, 0.05);
            solid.Material.Diffuse = 0.1;
            solid.Material.Shininess = 300;
            solid.Material.Reflective = 1;
            solid.Material.Transparency = 0;
            solid.Material.RefractiveIndex = 1.52;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, sky, glass, hollow, solid});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter12");
            Console.WriteLine("Chapter12 Complete");
        }
    }
}
