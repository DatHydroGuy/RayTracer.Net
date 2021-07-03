using System;
using System.Collections.Generic;
using RayTracer;
using RayTracer.Patterns;

namespace RayTracerApp
{
    public class Chapter16CSGHexagon
    {
        public Chapter16CSGHexagon()
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

            var hex = Hexagon();
            hex.Transform = Transformations.Translation(0, 0.5, 0) * Transformations.Scaling(2, 2, 2);

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, sky, hex});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(0, 2.5, -5), new Point(0, 0, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter16CSGHexagon");
            Console.WriteLine("Chapter16CSGHexagon Complete");
        }

        public Shape Hexagon()
        {
            var material = new Material();
            material.Colour = new Colour(0.05, 0.05, 0.05);
            material.Diffuse = 0.1;
            material.Shininess = 300;
            material.Reflective = 1;
            material.Transparency = 1;
            material.RefractiveIndex = 1.52;

            var ballTransform = Transformations.Translation(0, 0, -1) * Transformations.Scaling(0.25, 0.25, 0.25);
            var barTransform = Transformations.Translation(0, 0, -1) * Transformations.RotationY(-Math.PI / 6.0) * Transformations.RotationZ(-Math.PI / 2.0) * Transformations.Scaling(0.25, 1, 0.25);

            var balls = new List<Shape>();
            var bars = new List<Shape>();
            for (int i = 0; i < 6; i++)
            {
                var sphere = new Sphere();
                sphere.Transform = Transformations.RotationY(0.1 + i * Math.PI / 3.0) * ballTransform;
                sphere.Material = material;
                balls.Add(sphere);
                var bar = new Cylinder();
                bar.Minimum = 0;
                bar.Maximum = 1;
                bar.Transform = Transformations.RotationY(0.1 + i * Math.PI / 3.0) * barTransform;
                bar.Material = material;
                bars.Add(bar);

            }
            var hex = new CSG(CsgOperation.Union, balls[0], balls[1]);
            hex = new CSG(CsgOperation.Union, hex, balls[2]);
            hex = new CSG(CsgOperation.Union, hex, balls[3]);
            hex = new CSG(CsgOperation.Union, hex, balls[4]);
            hex = new CSG(CsgOperation.Union, hex, balls[5]);
            hex = new CSG(CsgOperation.Union, hex, bars[0]);
            hex = new CSG(CsgOperation.Union, hex, bars[1]);
            hex = new CSG(CsgOperation.Union, hex, bars[2]);
            hex = new CSG(CsgOperation.Union, hex, bars[3]);
            hex = new CSG(CsgOperation.Union, hex, bars[4]);
            hex = new CSG(CsgOperation.Union, hex, bars[5]);
            return hex;
        }
    }
}
