using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter07
    {
        public Chapter07()
        {
            var floor = new Sphere();
            floor.Transform = Transformations.Scaling(10, 0.01, 10);
            floor.Material = new Material();
            floor.Material.Colour = new Colour(1, 0.9, 0.9);
            floor.Material.Specular = 0;

            var leftWall = new Sphere();
            leftWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(-Math.PI / 4.0) *
                                 Transformations.RotationX(Math.PI / 2.0) * Transformations.Scaling(10, 0.01, 10);
            leftWall.Material = floor.Material;

            var rightWall = new Sphere();
            rightWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(Math.PI / 4.0) *
                                 Transformations.RotationX(Math.PI / 2.0) * Transformations.Scaling(10, 0.01, 10);
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
            world.Objects = new Sphere[] {floor, leftWall, rightWall, middleSphere, leftSphere, rightSphere};
            world.Lights = new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1)),
                                        Light.PointLight(new Point(10, 10, -10), new Colour(0.5, 0.5, 1))};

            var camera = new Camera(1000, 500, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 1, 1, true);
            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter07_2lights");
            Console.WriteLine("Chapter07 Complete");
        }
    }
}
