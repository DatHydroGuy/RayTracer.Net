using System;
using RayTracer;

namespace RayTracerApp
{
    public class Chapter03
    {
        public Chapter03()
        {
            var canvas = new Canvas();
            canvas.SetColour(new Colour(0.2, 0, 0.4));
            for (int i = 0; i < canvas.Height; i++)
            {
                canvas.WritePixel(i, i, new Colour(0.8, 1, 0.6));
            }

            canvas.WriteImageFile("I:\\Programming\\DotNetCore\\RayTracer\\RayTracer\\Images\\chapter03");
            Console.WriteLine("Chapter03 Complete");
        }
    }
}
