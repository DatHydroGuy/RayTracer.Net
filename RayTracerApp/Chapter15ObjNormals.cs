using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter15ObjNormals
    {
        public Chapter15ObjNormals()
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

            var parser = new ObjParser();
            parser.ReadFile(@"I:\Programming\DotNetCore\RayTracer\RayTracer\Obj\utah_teapot.obj");
            var teapot = parser.ObjToGroup();
            teapot.Transform = Transformations.Scaling(0.15, 0.15, 0.15) * Transformations.RotationX(-Math.PI / 2.0);
            var teapotMaterial = new Material();
            // teapotMaterial.Colour = new Colour(0.5, 0.4, 0);
            teapotMaterial.Colour = new Colour(0.05, 0.05, 0.55);
            teapotMaterial.Diffuse = 0.1;
            teapotMaterial.Shininess = 300;
            teapotMaterial.Reflective = 1;
            teapotMaterial.Transparency = 1;
            teapotMaterial.RefractiveIndex = 1.52;
            teapot.SetMaterial(teapotMaterial);

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, teapot});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter15ObjNormals");
            Console.WriteLine("Chapter15ObjNormals Complete");
        }
    }
}
