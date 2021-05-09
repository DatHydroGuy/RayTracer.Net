using System;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Camera
    {
        private int _hSize;
        private int _vSize;
        private double _fov;
        private Matrix _transform;
        private Matrix _transformInverse;
        private double _pixelSize;
        private double _halfWidth;
        private double _halfHeight;

        public int HSize
        {
            get { return _hSize; }
            set { _hSize = value; }
        }
        public int VSize
        {
            get { return _vSize; }
            set { _vSize = value; }
        }
        public double FieldOfView
        {
            get { return _fov; }
            set { _fov = value; }
        }
        public Matrix Transform
        {
            get { return _transform; }
            set { 
                _transform = value;
                _transformInverse = value.Inverse();
                }
        }
        public Matrix TransformInverse
        {
            get { return _transformInverse; }
            set { 
                _transformInverse = value;
                _transform = value.Inverse();
                }
        }
        public double PixelSize
        {
            get { return _pixelSize; }
            set { _pixelSize = value; }
        }
        
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
            var aspectRatio = (double)hSize / (double)vSize;

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
            Random rnd = new Random((int)DateTime.Now.Ticks);
            var jitterSize = 1.0 / (double)antialiasLevel;
            var colourDivisor = 1.0 / (double)(antialiasLevel * antialiasLevel);
            int oldComplete = -1;

            for (int y = 0; y < VSize; y++)
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
                Parallel.For(0, HSize, x =>
                {
                    if (antialiasLevel == 1)
                    {
                        var ray = RayForPixel(x, y);
                        var colour = w.ColourAt(ray, reflectionDepth);
                        image.WritePixel(x, y, colour);
                    }
                    else
                    {
                        var baseColour = Colour.BLACK;
                        for (int yy = 0; yy < antialiasLevel; yy++)
                        {
                            for (int xx = 0; xx < antialiasLevel; xx++)
                            {
                                var xJitter = jitterSize * (xx + rnd.NextDouble());
                                var yJitter = jitterSize * (yy + rnd.NextDouble());
                                var ray = RayForPixel(x, y, xJitter, yJitter);
                                baseColour += w.ColourAt(ray, reflectionDepth) * colourDivisor;
                            }
                        }
                        image.WritePixel(x, y, baseColour);
                    }
                });

                if (showProgress)
                {
                    int complete = (int)(100 * y / VSize);
                    if (complete != oldComplete)
                    {
                        Console.WriteLine($"{complete}% complete");
                        oldComplete = complete;
                    }
                }
            }

            Console.WriteLine("100% complete");
            TimeSpan elapsedTime = DateTime.Now - startTime;
            var formattedElapsed = elapsedTime.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"Time taken: {formattedElapsed}");
            return image;
        }
    }
}
