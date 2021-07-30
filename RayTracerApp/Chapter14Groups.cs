using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter14Groups
    {
        public Chapter14Groups()
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
            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter14Groups");
            Console.WriteLine("Chapter14Groups Complete");
        }

        public Shape HexCorner()
        {
            var glass = new Sphere();
            glass.Transform = Transformations.Translation(0, 0, -1) * Transformations.Scaling(0.25, 0.25, 0.25);
            glass.Material = new Material();
            // glass.Material.Colour = new Colour(1, 1, 1);
            glass.Material.Colour = new Colour(0.05, 0.05, 0.05);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;
            return glass;
        }

        public Shape HexEdge()
        {
            var glass = new Cylinder();
            glass.Minimum = 0;
            glass.Maximum = 1;
            glass.Transform = Transformations.Translation(0, 0, -1) * Transformations.RotationY(-Math.PI / 6.0) * Transformations.RotationZ(-Math.PI / 2.0) * Transformations.Scaling(0.25, 1, 0.25);
            glass.Material = new Material();
            // glass.Material.Colour = new Colour(1, 1, 1);
            glass.Material.Colour = new Colour(0.05, 0.05, 0.05);
            glass.Material.Diffuse = 0.1;
            glass.Material.Shininess = 300;
            glass.Material.Reflective = 1;
            glass.Material.Transparency = 1;
            glass.Material.RefractiveIndex = 1.52;
            return glass;
        }

        public Shape HexSide()
        {
            var side = new Group();
            side.AddChild(HexCorner());
            side.AddChild(HexEdge());
            return side;
        }

        public Shape Hexagon()
        {
            var hex = new Group();
            for (int i = 0; i < 6; i++)
            {
                var side = HexSide();
                side.Transform = Transformations.RotationY(i * Math.PI / 3.0);
                hex.AddChild(side);
            }
            return hex;
        }
    }
}
