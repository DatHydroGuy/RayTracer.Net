using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter16CSGHollowSphereCube
    {
        public Chapter16CSGHollowSphereCube()
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

            var shp = MakeShape();
            shp.Transform = Transformations.Translation(0, 1, 0) * Transformations.Scaling(1.33, 1.33, 1.33) * Transformations.RotationY(-0.3);

            var world = new World();
            world.AddShapesToWorld(new Shape[] {floor, sky, shp});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 3, -5), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(-0.5, 2.5, -5), new Point(0, 1, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 8, true);
            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter16CSGHollowSphereCubeAA");
            Console.WriteLine("Chapter16CSGHollowSphereCube Complete");
        }

        public Shape MakeShape()
        {
            var bar1 = new Cylinder();
            bar1.Minimum = -1;
            bar1.Maximum = 1;
            bar1.Closed = true;
            bar1.Transform = Transformations.Scaling(0.5, 1, 0.5);
            bar1.Material = new Material();
            bar1.Material.Colour = Colour.RED;
            bar1.Material.Diffuse = 0.1;
            bar1.Material.Shininess = 300;

            var bar2 = new Cylinder();
            bar2.Minimum = -1;
            bar2.Maximum = 1;
            bar2.Closed = true;
            bar2.Transform = Transformations.RotationZ(Math.PI / 2.0) * Transformations.Scaling(0.5, 1, 0.5);
            bar2.Material = new Material();
            bar2.Material.Colour = Colour.GREEN;
            bar2.Material.Diffuse = 0.1;
            bar2.Material.Shininess = 300;

            var bar3 = new Cylinder();
            bar3.Minimum = -1;
            bar3.Maximum = 1;
            bar3.Closed = true;
            bar3.Transform = Transformations.RotationX(Math.PI / 2.0) * Transformations.Scaling(0.5, 1, 0.5);
            bar3.Material = new Material();
            bar3.Material.Colour = Colour.BLUE;
            bar3.Material.Diffuse = 0.1;
            bar3.Material.Shininess = 300;

            var bars = new CSG(CsgOperation.Union, bar1, bar2);
            bars = new CSG(CsgOperation.Union, bars, bar3);

            var sphere = new Sphere();
            sphere.Material = new Material();
            sphere.Material.Colour = new Colour(0.3, 0.3, 0.3);
            sphere.Material.Diffuse = 0.1;
            sphere.Material.Shininess = 300;

            var cube = new Cube();
            cube.Transform = Transformations.Scaling(0.75, 0.75, 0.75);
            cube.Material = new Material();
            cube.Material.Colour = new Colour(0.05, 0.05, 0.05);
            cube.Material.Diffuse = 0.1;
            cube.Material.Shininess = 300;
            cube.Material.Reflective = 1;

            var result = new CSG(CsgOperation.Intersect, sphere, cube);
            result = new CSG(CsgOperation.Difference, result, bars);
            
            return result;
        }
    }
}
