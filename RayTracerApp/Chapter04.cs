using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter04
    {
        public Chapter04()
        {
            var canvas = new Canvas(1000, 1000);
            var plotColour = new Colour(0.3, 0, 1);
            for (int i = 0; i < 12; i++)
            {
                var twelvePoint = new Point(0, 0, 1);
                var rotation = Transformations.RotationY(-i * Math.PI / 6.0);
                var scaling = Transformations.Scaling(canvas.Height * 0.375, canvas.Height * 0.375, canvas.Height * 0.375);
                var moveToMiddle = Transformations.Translation(canvas.Width / 2, 0, canvas.Height / 2);
                var newPoint = moveToMiddle * scaling * rotation * twelvePoint;
                for (int x = -3; x < 4; x++)
                {
                    for (int y = -3; y < 4; y++)
                    {
                        canvas.WritePixel((int)newPoint.X + x, (int)newPoint.Z + y, plotColour);
                    }
                }
            }

            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter04");
            Console.WriteLine("Chapter04 Complete");
        }
    }
}
