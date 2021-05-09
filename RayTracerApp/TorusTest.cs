using System;
using RayTracer;

namespace RayTracerApp
{
    public class TorusTest
    {
        public TorusTest()
        {
            var floor = new Plane();
            floor.Material = new Material();
            floor.Material.Colour = new Colour(1, 0.9, 0.9);
            floor.Material.Specular = 0;

            var torus = new Torus();
            torus.Transform = Transformations.Translation(0.01, 1.51, 0.01) * Transformations.Scaling(2, 2, 2);
            torus.Material = new Material();
            torus.Material.Colour = new Colour(0.1, 1, 0.5);
            torus.Material.Diffuse = 0.7;
            torus.Material.Specular = 0.3;

            var world = new World();
            world.Objects = new Shape[] {floor, torus};
            world.Lights = new Light[] {Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1))};//,
//                                        Light.PointLight(new Point(10, 10, -10), new Colour(0.5, 0.5, 1))};

            var camera = new Camera(1000, 500, Math.PI / 3.0);
            camera.Transform = Transformations.ViewTransform(new Point(0, 4, -6), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 1, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\torus_test");
            Console.WriteLine("TorusTest Complete");
        }
    }
}
