using System;
using RayTracer;

namespace RayTracerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // var ch03 = new Chapter03();
            // var ch04 = new Chapter04();
            // var ch05 = new Chapter05();
            // var ch06 = new Chapter06();
            // var ch07 = new Chapter07();
            // var ch08 = new Chapter08();
            // var ch09 = new Chapter09();
            // var ch10 = new Chapter10();
            // var blendedPatternTest = new BlendedPatternTest();
            // var nestedPatternTest = new NestedPatternTest();
            // var perturbedPatternTest = new PerturbedPatternTest();
            // var reflectionTest = new ReflectionTest();
            // var chapter11ImageTest = new Chapter11Image();
            // var chapter11PlayTest = new Chapter11Play();
            // var chapter11WaterTest = new Chapter11WaterTest();
            // var chapter12 = new Chapter12();
            // var chapter13_1 = new Chapter13InfCylinders();
            // var chapter13_2 = new Chapter13ConsCylinders();
            // var chapter13_3 = new Chapter13CapCylinders();
            // var chapter13_4 = new Chapter13Cones();
            // var chapter14 = new Chapter14Groups();
            // var chapter14AABB = new Chapter14AABB();
            // var chapter14NoAABB = new Chapter14NoAABB();
            // var chapter14SphereCubeNoAABB = new Chapter14SphereCubeNoAABB();
            // var chapter14SphereCubeAABB = new Chapter14SphereCubeAABB();
            // var chapter15Triangles = new Chapter15Triangles();
            // var chapter15ObjTeapot = new Chapter15ObjTeapot();
            // var chapter15ObjNormals = new Chapter15ObjNormals();
            // var chapter16CSGHexagon = new Chapter16CSGHexagon();
            // var chapter16CSGHollowSphereCube = new Chapter16CSGHollowSphereCube();
            // var bvhDragon = new BVHDragon();

            if (args.Length != 1)
            {
                Console.WriteLine("ERROR: No YAML file argument found to render!");
                Console.WriteLine("");
                Console.WriteLine("Usage:");
                Console.WriteLine("\t\t\t\tRAYTRACERAPP YAMLFILE");
                Console.WriteLine("");
                Console.WriteLine("Application Description:");
                Console.WriteLine("\tReads a YAML formatted scene description and renders it in ray-traced glory!");
                Console.WriteLine("");
                Console.WriteLine("Argument Description:");
                Console.WriteLine("\tyamlFile\t\tThe YAML-formatted scene description file to be rendered.");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(2);
            }

            var yaml = new YamlReader(args[0]);     // To debug: MAKE SURE args of launch.json is set before using this!  e.g. "args": ["..\\YAML\\Cover.yml"],

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
