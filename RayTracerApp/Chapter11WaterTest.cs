using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter11WaterTest
    {
        public Chapter11WaterTest()
        {
            var floor = new Plane();
            var floorPattern = new CheckerPattern(new Colour(1, 0.3, 0.7), new Colour(0.3, 1, 0.7));
            floor.Material = new Material();
            floor.Material.Specular = 0;
            floor.Material.Pattern = floorPattern;

            var leftWall = new Plane();
            leftWall.Transform = Transformations.Translation(-4, 0, 0) * Transformations.RotationZ(Math.PI / 2.0);
            var leftWallPattern = new CheckerPattern(Colour.BLACK, Colour.WHITE);
            leftWallPattern.Transform = Transformations.RotationY(Math.PI / 6.0);
            leftWall.Material = new Material();
            leftWall.Material.Specular = 0;
            leftWall.Material.Pattern = leftWallPattern;

            var rightWall = new Plane();
            rightWall.Transform = Transformations.Translation(0, 0, 3) * Transformations.RotationX(Math.PI / 2.0);
            var rightWallPattern = new CheckerPattern(Colour.BLACK, Colour.WHITE);
            rightWallPattern.Transform = Transformations.RotationY(Math.PI / 6.0);
            rightWall.Material = new Material();
            rightWall.Material.Specular = 0;
            rightWall.Material.Pattern = rightWallPattern;

            var water = new Plane();
            water.Transform = Transformations.Translation(0, 1.75, 0);
            water.Material = new Material();
            water.Material.Colour = new Colour(0, 0.1, 0.2);
            water.Material.Ambient = 0.1;
            water.Material.Diffuse = 0.1;
            water.Material.Specular = 0.5;
            water.Material.Shininess = 300;
            water.Material.Reflective = 1;
            water.Material.Transparency = 0.5;
            water.Material.RefractiveIndex = 1.33;
            water.Material.CastsShadow = false;

            var glass = new Sphere();
            glass.Transform = Transformations.Translation(1.5, 1, 0);
            glass.Material = new Material();
            glass.Material.Colour = new Colour(0.05, 0.05, 0.05);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;

            var hollow = new Sphere();
            hollow.Transform = Transformations.Translation(1.5, 1, 0) * Transformations.Scaling(0.5, 0.5, 0.5);
            hollow.Material = new Material();
            hollow.Material.Colour = new Colour(0.05, 0.05, 0.05);
            hollow.Material.Diffuse = 0.1;
            hollow.Material.Shininess = 300;
            hollow.Material.Reflective = 1;
            hollow.Material.Transparency = 1;
            hollow.Material.RefractiveIndex = 1;

            var solid = new Sphere();
            solid.Transform = Transformations.Translation(-1.5, 1, 0);
            solid.Material = new Material();
            solid.Material.Colour = new Colour(0.05, 0.05, 0.05);
            solid.Material.Diffuse = 0.1;
            solid.Material.Shininess = 300;
            solid.Material.Reflective = 1;
            solid.Material.Transparency = 0;
            solid.Material.RefractiveIndex = 1.52;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, water, glass, hollow, solid});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(10, 10, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(2, 4, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter11WaterTest");
            Console.WriteLine("Chapter11WaterTest Complete");
        }
    }
}
