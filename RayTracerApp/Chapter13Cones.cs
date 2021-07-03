using System;
using RayTracer;
using RayTracer.Patterns;

namespace RayTracerApp
{
    public class Chapter13Cones
    {
        public Chapter13Cones()
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

            var glass = new Cone();
            glass.Minimum = -1;
            glass.Maximum = 0;
            glass.Closed = true;
            glass.Transform = Transformations.Translation(1.6, 2.01, 0.01) * Transformations.Scaling(1, 2, 1);
            glass.Material = new Material();
            glass.Material.Colour = new Colour(0.05, 0.05, 0.55);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;

            var hollow = new Cone();
            hollow.Minimum = -1;
            hollow.Maximum = 0;
            hollow.Closed = true;
            hollow.Transform = Transformations.Translation(1.6, 1.71, 0.01) * Transformations.Scaling(0.8, 1.6, 0.8);
            hollow.Material = new Material();
            hollow.Material.Colour = new Colour(0.05, 0.05, 0.05);
            hollow.Material.Diffuse = 0.1;
            hollow.Material.Shininess = 300;
            hollow.Material.Reflective = 1;
            hollow.Material.Transparency = 1;
            hollow.Material.RefractiveIndex = 1;

            var solid = new Cone();
            solid.Minimum = 0;
            solid.Maximum = 1;
            solid.Closed = true;
            solid.Transform = Transformations.Translation(-1.6, 0.01, -0.25) * Transformations.Scaling(1, 2.5, 1);
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
            camera.Transform = Transformations.ViewTransform(new Point(0, 2.1, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter13Cones");
            Console.WriteLine("Chapter13Cones Complete");
        }
    }
}
