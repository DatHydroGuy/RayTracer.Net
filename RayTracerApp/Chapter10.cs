using System;
using RayTracer;
using RayTracer.Patterns;

namespace RayTracerApp
{
    public class Chapter10
    {
        public Chapter10()
        {
            var floor = new Plane();
            var floorPattern = new StripePattern(new Colour(1, 1, 0), new Colour(0, 0, 1));
            floorPattern.Transform = Transformations.Scaling(0.5, 0.5, 0.5);
            floor.Material = new Material();
            floor.Material.Colour = new Colour(1, 0.9, 0.9);
            floor.Material.Specular = 0;
            floor.Material.Pattern = floorPattern;

            var leftWall = new Plane();
            var leftWallPattern = new RingPattern(new Colour(0, 1, 0), new Colour(1, 0.5, 0));
            leftWallPattern.Transform = Transformations.Scaling(0.5, 0.5, 0.5);
            leftWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(-Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            leftWall.Material = new Material();
            leftWall.Material.Colour = new Colour(1, 0.9, 0.9);
            leftWall.Material.Specular = 0;
            leftWall.Material.Pattern = leftWallPattern;

            var rightWall = new Plane();
            var rightWallPattern = new CheckerPattern(new Colour(1, 0, 0), new Colour(0, 1, 1));
            rightWallPattern.Transform = Transformations.Scaling(0.75, 0.5, 0.5);
            rightWall.Transform = Transformations.Translation(0, 0, 5) * Transformations.RotationY(Math.PI / 4.0) * Transformations.RotationX(Math.PI / 2.0);
            rightWall.Material = new Material();
            rightWall.Material.Colour = new Colour(1, 0.9, 0.9);
            rightWall.Material.Specular = 0;
            rightWall.Material.Pattern = rightWallPattern;

            var middleSphere = new Sphere();
            middleSphere.Transform = Transformations.Translation(-0.5, 1, 0.5) * Transformations.RotationX(-2 * Math.PI / 3.0) * Transformations.RotationZ(Math.PI / 3.0);
            middleSphere.Material = new Material();
            middleSphere.Material.Colour = new Colour(0.1, 1, 0.5);
            middleSphere.Material.Diffuse = 0.7;
            middleSphere.Material.Specular = 0.3;
            var middleSpherePattern = new GradientPattern(new Colour(1, 0, 0), new Colour(0, 1, 1));
            middleSpherePattern.Transform = Transformations.Translation(-1, 0, 0) * Transformations.Scaling(2, 2, 2) * Transformations.RotationX(Math.PI / 3.0);
            middleSphere.Material.Pattern = middleSpherePattern;

            var rightSphere = new Sphere();
            rightSphere.Transform = Transformations.Translation(1.5, 0.5, -0.5) * Transformations.Scaling(0.5, 0.5, 0.5) * Transformations.RotationX(2 * Math.PI / 3.0) * Transformations.RotationZ(-Math.PI / 4.0);
            rightSphere.Material = new Material();
            rightSphere.Material.Colour = new Colour(0.9, 1, 0.8);
            rightSphere.Material.Diffuse = 0.6;
            rightSphere.Material.Shininess = 10;
            rightSphere.Material.Specular = 0.3;
            var rightSpherePattern = new StripePattern(new Colour(1, 1, 0), new Colour(0.4, 0, 1));
            rightSpherePattern.Transform = Transformations.Scaling(0.1, 0.1, 0.1);
            rightSphere.Material.Pattern = rightSpherePattern;

            var leftSphere = new Sphere();
            leftSphere.Transform = Transformations.Translation(-1.5, 0.33, -0.75) * Transformations.Scaling(0.33, 0.33, 0.33) * Transformations.RotationX(Math.PI / 3.0) * Transformations.RotationZ(Math.PI / 4.0);
            leftSphere.Material = new Material();
            leftSphere.Material.Colour = new Colour(1, 0.8, 0.1);
            leftSphere.Material.Diffuse = 0.7;
            leftSphere.Material.Specular = 0.3;
            var leftSpherePattern = new CheckerPattern(new Colour(1, 0.8, 0.8), new Colour(0.2, 0.2, 0.4));
            leftSpherePattern.Transform = Transformations.Scaling(0.25, 0.5, 0.75);
            leftSphere.Material.Pattern = leftSpherePattern;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, leftWall, rightWall, middleSphere, leftSphere, rightSphere});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(0.5, 0.5, 0)),
                                                Light.PointLight(new Point(0, 10, -10), new Colour(0, 0.5, 0.5)),
                                                Light.PointLight(new Point(10, 10, -10), new Colour(0.5, 0, 0.5))});

            var camera = new Camera(1000, 500, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 1, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter10");
            Console.WriteLine("Chapter10 Complete");
        }
    }
}
