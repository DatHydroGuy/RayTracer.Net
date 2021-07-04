using System;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Camera
    {
        private Matrix _transform;
        private Matrix _transformInverse;
        private double _halfWidth;
        private double _halfHeight;

        public int HSize { get; }

        public int VSize { get; }

        public double FieldOfView { get; }

        public Matrix Transform
        {
            get => _transform;
            set { 
                _transform = value;
                _transformInverse = value.Inverse();
                }
        }
        public Matrix TransformInverse
        {
            get => _transformInverse;
            set { 
                _transformInverse = value;
                _transform = value.Inverse();
                }
        }
        public double PixelSize { get; }

        public Camera(int hSize = 200, int vSize = 125, double fieldOfView = Math.PI / 3.0)
        {
            HSize = hSize;
            VSize = vSize;
            FieldOfView = fieldOfView;
            Transform = Matrix.Identity(4);
            PixelSize = CalculatePixelSize(hSize, vSize, fieldOfView);
        }

        private double CalculatePixelSize(int hSize, int vSize, double fieldOfView)
        {
            var halfView = Math.Tan(fieldOfView / 2.0);
            var aspectRatio = hSize / (double)vSize;

            if (aspectRatio >= 1)
            {
                _halfWidth = halfView;
                _halfHeight = halfView / aspectRatio;
            }
            else
            {
                _halfWidth = halfView * aspectRatio;
                _halfHeight = halfView;
            }

            return _halfWidth * 2.0 / hSize;
        }

        public Ray RayForPixel(int pixelX, int pixelY, double xJitter = 0.5, double yJitter = 0.5)
        {
            // Calculate offset from edge of canvas to center of pixel
            var xOffset = (pixelX + xJitter) * PixelSize;
            var yOffset = (pixelY + yJitter) * PixelSize;

            // Calculate untransformed coordinates of the given pixel in world space
            // (remember that camera looks towards -z, so +x is to the left!)
            var worldX = _halfWidth - xOffset;
            var worldY = _halfHeight - yOffset;

            // Use camera matrix to "un-transform" the target pixel from world space into camera space (remember that pixel.z = -1)
            var pixel = TransformInverse * new Point(worldX, worldY, -1);

            // Use camera matrix to "un-transform" the origin from world space into camera space
            var origin = TransformInverse * new Point(0, 0, 0);

            // Calculate the ray's direction
            var direction = (pixel - origin).Normalise();

            return new Ray(origin, direction);
        }

        public Canvas Render(World w, int reflectionDepth = 5, int antialiasLevel = 1, bool showProgress = false)
        {
            var startTime = DateTime.Now;
            var image = new Canvas(HSize, VSize);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var jitterSize = 1.0 / antialiasLevel;
            var colourDivisor = 1.0 / (antialiasLevel * antialiasLevel);
            var oldComplete = -1;

            for (var y = 0; y < VSize; y++)
            {
                // for (int x = 0; x < HSize; x++)
                // {
                //     if (antialiasLevel == 1)
                //     {
                //         var ray = RayForPixel(x, y);
                //         var colour = w.ColourAt(ray, reflectionDepth);
                //         image.WritePixel(x, y, colour);
                //     }
                //     else
                //     {
                //         var baseColour = Colour.BLACK;
                //         for (int yy = 0; yy < antialiasLevel; yy++)
                //         {
                //             for (int xx = 0; xx < antialiasLevel; xx++)
                //             {
                //                 var xJitter = jitterSize * (xx + rnd.NextDouble());
                //                 var yJitter = jitterSize * (yy + rnd.NextDouble());
                //                 var ray = RayForPixel(x, y, xJitter, yJitter);
                //                 baseColour += w.ColourAt(ray, reflectionDepth) * colourDivisor;
                //             }
                //         }
                //         image.WritePixel(x, y, baseColour);
                //     }
                // }
                var y1 = y;
                Parallel.For(0, HSize, x =>
                {
                    if (antialiasLevel == 1)
                    {
                        var ray = RayForPixel(x, y1);
                        var colour = w.ColourAt(ray, reflectionDepth);
                        image.WritePixel(x, y1, colour);
                    }
                    else
                    {
                        var baseColour = Colour.BLACK;
                        for (var yy = 0; yy < antialiasLevel; yy++)
                        {
                            for (var xx = 0; xx < antialiasLevel; xx++)
                            {
                                var xJitter = jitterSize * (xx + rnd.NextDouble());
                                var yJitter = jitterSize * (yy + rnd.NextDouble());
                                var ray = RayForPixel(x, y1, xJitter, yJitter);
                                baseColour += w.ColourAt(ray, reflectionDepth) * colourDivisor;
                            }
                        }
                        image.WritePixel(x, y1, baseColour);
                    }
                });

                if (!showProgress)
                    continue;
                var complete = 100 * y / VSize;
                if (complete == oldComplete)
                    continue;
                Console.WriteLine($"{complete}% complete");
                oldComplete = complete;
            }

            Console.WriteLine("100% complete");
            var elapsedTime = DateTime.Now - startTime;
            var formattedElapsed = elapsedTime.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"Time taken: {formattedElapsed}");
            return image;
        }
    }
}
