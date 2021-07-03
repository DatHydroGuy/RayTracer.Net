using System;
using RayTracer;
using RayTracer.Patterns;

namespace RayTracerApp
{
    public class PerturbedPatternTest
    {
        public PerturbedPatternTest()
        {
            var floorScale = 0.6;
            var floorOctaves = 3;
            var floorPersistence = 10;
            var floorLacunarity = 2;

            var floor = new Plane();
            floor.Transform = Transformations.RotationY(-Math.PI * 0.15);
            var basePattern1 = new StripePattern(new Colour(1, 0, 0), new Colour(1, 1, 1));
            var basePattern2 = new StripePattern(new Colour(1, 0, 0), new Colour(1, 1, 1));
            basePattern2.Transform = Transformations.RotationY(Math.PI / 2.0);
            var floorPattern1 = new PerturbedPattern(basePattern1, floorScale, floorOctaves, floorPersistence, floorLacunarity);
            var floorPattern2 = new PerturbedPattern(basePattern2, floorScale, floorOctaves, floorPersistence, floorLacunarity);
            var blendedPattern = new BlendedPattern(floorPattern1, floorPattern2);
            floor.Material = new Material();
            floor.Material.Colour = new Colour(1, 0, 0);
            floor.Material.Specular = 0;
            floor.Material.Pattern = blendedPattern;

            var sphereScale = 0.9;
            var sphereOctaves = 4;
            var spherePersistence = 10;
            var sphereLacunarity = 1.7;

            var middleSphere = new Sphere();
            middleSphere.Transform = Transformations.Translation(-0.5, 1, 0.5) * Transformations.RotationY(-1.7) *
                                     Transformations.RotationX(-2.35) * Transformations.RotationZ(1.1);

            var basePattern3 = new StripePattern(new Colour(0, 0.8, 0.4), new Colour(0.3, 1, 0.6));
            basePattern3.Transform = Transformations.Translation(0.35, 0, 0) * Transformations.Scaling(0.4, 0.4, 0.4);

            var spherePattern = new PerturbedPattern(basePattern3, sphereScale, sphereOctaves, spherePersistence, sphereLacunarity);

            middleSphere.Material = new Material();
            middleSphere.Material.Colour = new Colour(1, 1, 1);
            middleSphere.Material.Diffuse = 0.7;
            middleSphere.Material.Specular = 0.3;
            middleSphere.Material.Pattern = spherePattern;

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, middleSphere});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1600, 1000, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 1.5, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 1, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\PerturbedPatternTest");
            Console.WriteLine("PerturbedPatternTest Complete");
        }
    }
}
