using System;
using RayTracer;
using RayTracer.Patterns;

namespace RayTracerApp
{
    public class BVHDragon
    {
        public BVHDragon()
        {
            var floor = new Plane();
            floor.Transform = Transformations.RotationY(-Math.PI / 4.0);
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

            Console.WriteLine("BVHDragon Started...");
            Console.WriteLine("Loading model data...");
            var startTime = DateTime.Now;
            var startTime1 = DateTime.Now;

            var parser = new ObjParser();
            parser.ReadFile(@"I:\Programming\DotNetCore\RayTracer\RayTracer\Obj\dragon.obj");
            var dragon = parser.ObjToGroup();

            TimeSpan elapsedTime = DateTime.Now - startTime1;
            var formattedElapsed = elapsedTime.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"Finished loading model data.  Time taken: {formattedElapsed}");

            dragon.Transform = Transformations.Translation(0, 0.121684 / 2.0, 0) * Transformations.Scaling(0.5, 0.5, 0.5);
            var dragonMaterial = new Material();
            dragonMaterial.Colour = new Colour(0.5, 0.4, 1);
            // teapotMaterial.Colour = new Colour(0.05, 0.05, 0.55);
            // dragonMaterial.Diffuse = 0.1;
            // dragonMaterial.Shininess = 1;
            // teapotMaterial.Reflective = 1;
            // teapotMaterial.Transparency = 1;
            // teapotMaterial.RefractiveIndex = 1.52;
            dragon.SetMaterial(dragonMaterial);

            Console.WriteLine("Subdividing model data...");
            startTime1 = DateTime.Now;
            Shape.Divide(dragon, 500);
            elapsedTime = DateTime.Now - startTime1;
            formattedElapsed = elapsedTime.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"Finished subdividing.  Time taken: {formattedElapsed}");

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, dragon});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 8, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\BVHDragonAA");
            elapsedTime = DateTime.Now - startTime;
            formattedElapsed = elapsedTime.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"BVHDragon Complete.  Time taken: {formattedElapsed}");
        }
    }
}
