using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter09
    {
        public Chapter09()
        {
            var floor = new Plane();
            floor.Material = new Material();
            floor.Material.Colour = new Colour(1, 0.9, 0.9);
            floor.Material.Specular = 0;

            var leftWall = new Plane();
            leftWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(-Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            leftWall.Material = floor.Material;

            var rightWall = new Plane();
            rightWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            rightWall.Material = floor.Material;

            var middleSphere = new Sphere();
            middleSphere.Transform = Transformations.Translation(-0.5, 1, 0.5);
            middleSphere.Material = new Material();
            middleSphere.Material.Colour = new Colour(0.1, 1, 0.5);
            middleSphere.Material.Diffuse = 0.7;
            middleSphere.Material.Specular = 0.3;

            var rightSphere = new Sphere();
            rightSphere.Transform = Transformations.Translation(1.5, 0.5, -0.5) * Transformations.Scaling(0.5, 0.5, 0.5);
            rightSphere.Material = new Material();
            rightSphere.Material.Colour = new Colour(0.9, 1, 0.8);
            rightSphere.Material.Diffuse = 0.6;
            rightSphere.Material.Shininess = 10;
            rightSphere.Material.Specular = 0.3;

            var leftSphere = new Sphere();
            leftSphere.Transform = Transformations.Translation(-1.5, 0.33, -0.75) * Transformations.Scaling(0.33, 0.33, 0.33);
            leftSphere.Material = new Material();
            leftSphere.Material.Colour = new Colour(1, 0.8, 0.1);
            leftSphere.Material.Diffuse = 0.7;
            leftSphere.Material.Specular = 0.3;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, middleSphere, leftSphere, rightSphere});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1)),
                                                Light.PointLight(new Point(10, 10, -10), new Colour(0.5, 0.5, 1))});

            var camera = new Camera(1000, 500, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 1, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter09_2lights");
            Console.WriteLine("Chapter09 Complete");
        }
    }
}
