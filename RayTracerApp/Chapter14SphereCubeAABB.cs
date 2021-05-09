using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter14SphereCubeAABB
    {
        public Chapter14SphereCubeAABB()
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

            var world = new World();

            const int NUM = 5;
            Shape[,,] hs = new Shape[NUM,NUM,NUM];
            for (int x = 0; x < NUM; x++)
            {
                for (int y = 0; y < NUM; y++)
                {
                    for (int z = 0; z < NUM; z++)
                    {
                        hs[x,y,z] = BallGroup(x, y, z);
                        world.AddShapeToWorld(hs[x,y,z]);
                    }
                }
            }

            // world.AddShapesToWorld(new Shape[] {floor, sky});
            world.AddLightsToWorld(new Light[] {Light.PointLight(new Point(-5, 5, -10), new Colour(1, 1, 1))});

            var camera = new Camera(1000, 600, Math.PI / 3);
            camera.Transform = Transformations.ViewTransform(new Point(-3, 5, -7), new Point(0, -0.25, 0), new Vector(0, 1, 0));

            var canvas = camera.Render(world, 8, 1, true);
            canvas.WritePpmFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter14SphereCubeAABB");
            Console.WriteLine("Chapter14SphereCubeAABB Complete");
        }

        public Shape Ball(int x, int y, int z)
        {
            var glass = new Sphere();
            glass.Transform = Transformations.Translation(0.5 * (x - 2), 0.5 * (y - 2), 0.5 * (z - 2)) * Transformations.Scaling(0.2, 0.2, 0.2);
            glass.Material = new Material();
            glass.Material.Colour = new Colour(0.1 * x, 0.1 * y, 0.1 * z);
            return glass;
        }

        public Shape BallGroup(int x, int y, int z)
        {
            var bg = new Group();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var b = Ball(i, j, k);
                        b.Material.Colour += new Colour(x * 0.2, y * 0.2, z * 0.2);
                        bg.AddChild(b);
                    }
                }
            }

            bg.Transform = Transformations.Translation(x - 1, y - 1, z - 1);
            return bg;
        }

        // public Shape HexSide()
        // {
        //     var side = new Group();
        //     side.AddChild(HexCorner());
        //     side.AddChild(HexEdge());
        //     return side;
        // }

        // public Shape Hexagon()
        // {
        //     var hex = new Group();
        //     for (int i = 0; i < 6; i++)
        //     {
        //         var side = HexSide();
        //         side.Transform = Transformations.RotationY(i * Math.PI / 3.0);
        //         hex.AddChild(side);
        //     }
        //     return hex;
        // }
    }
}
