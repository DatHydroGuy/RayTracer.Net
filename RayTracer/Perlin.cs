using System;

namespace RayTracer
{
    public class Perlin
    {

        public static double Perlin3(double x, double y, double z, int octaves, double persistence, double lacunarity)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double totalAmplitude = 0;          // Used for normalising result into range 0.0 to 1.0
            for (var i = 0; i < octaves; i++)
            {
                total += Calculate(x * frequency, y * frequency, z * frequency) * amplitude;
                totalAmplitude += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / totalAmplitude;
        }

        private static readonly int[] Permutation = { 151,160,137,91,90,15,					// Hash lookup table as defined by Ken Perlin.  This is a randomly
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,	// arranged array of all numbers from 0-255 inclusive.
            190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
            129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
            49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private static readonly int[] DoublePermutation;                            // Doubled permutation to avoid overflow

        static Perlin()
        {
            DoublePermutation = new int[512];
            for (var x = 0; x < 512; x++)
            {
                DoublePermutation[x] = Permutation[x % 256];
            }
        }

        private static double Calculate(double x, double y, double z)
        {
            var xi = ((int)(Math.Floor(x) % 256) + 256) % 256;		// Calculate the "unit cube" that the point asked will be located in
            var yi = ((int)(Math.Floor(y) % 256) + 256) % 256;		// The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
            var zi = ((int)(Math.Floor(z) % 256) + 256) % 256;		// plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.

            var xf = x - (int)x;								    // We also fade the location to smooth the result.
            if (x < 0)
                xf = 1 + xf;
            
            var yf = y - (int)y;
            if (y < 0)
                yf = 1 + yf;
            
            var zf = z - (int)z;
            if (z < 0)
                zf = 1 + zf;
            
            var u = Fade(xf);
            var v = Fade(yf);
            var w = Fade(zf);

            var a = DoublePermutation[xi] + yi;								        // This here is Perlin's hash function.  We take our x value (remember,
            var aa = DoublePermutation[a] + zi;								        // between 0 and 255) and get a random value (from our p[] array above) between
            var ab = DoublePermutation[a + 1] + zi;								    // 0 and 255.  We then add y to it and plug that into p[], and add z to that.
            var b = DoublePermutation[xi + 1] + yi;								    // Then, we get another random value by adding 1 to that and putting it into p[]
            var ba = DoublePermutation[b] + zi;								        // and add z to it.  We do the whole thing over again starting with x+1.  Later
            var bb = DoublePermutation[b + 1] + zi;                                  // we plug aa, ab, ba, and bb back into p[] along with their +1's to get another set.
                                                                    // in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
                                                                    // These are all interpolated together using u, v, and w below.

            var x1 = Lerp(Grad(DoublePermutation[aa], xf, yf, zf),			// This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
                      Grad(DoublePermutation[ba], xf - 1, yf, zf),			    // our final gradient values.  Then, we interpolate between those gradients with the u value to get
                      u);										                        // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
            var x2 = Lerp(Grad(DoublePermutation[ab], xf, yf - 1, zf),		// we interpolate between the y-values to get a z-value.
                      Grad(DoublePermutation[bb], xf - 1, yf - 1, zf),
                      u);										                        // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
            var y1 = Lerp(x1, x2, v);                                       // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
                                                                                        // to zi.  The other 3 parameters are your possible return values (see grad()), which are actually
            x1 = Lerp(Grad(DoublePermutation[aa + 1], xf, yf, zf - 1),		    // the vectors from the edges of the unit cube to the point in the unit cube itself.
                      Grad(DoublePermutation[ba + 1], xf - 1, yf, zf - 1),
                      u);
            x2 = Lerp(Grad(DoublePermutation[ab + 1], xf, yf - 1, zf - 1),
                      Grad(DoublePermutation[bb + 1], xf - 1, yf - 1, zf - 1),
                      u);
            var y2 = Lerp(x1, x2, v);

            return (Lerp(y1, y2, w) + 1) / 2;						// For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
        }

        private static double Grad(int hash, double x, double y, double z)
        {
            var h = hash & 15;									    // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
            var u = h < 8 /* 0b1000 */ ? x : y;                  // If the most significant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

            double v;                                               // In Ken Perlin's original implementation this was another conditional operator (?:).  I
                                                                    // expanded it for readability.

            switch (h)
            {
                /* 0b0100 */
                // If the first and second significant bits are 0 set v = y
                case < 4:
                    v = y;
                    break;
                case 12:
                /* 0b1100 */
                /* 0b1110*/
                // If the first and second significant bits are 1 set v = x
                case 14:
                    v = x;
                    break;
                // If the first and second significant bits are not equal (0/1, 1/0) set v = z
                default:
                    v = z;
                    break;
            }

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);     // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
        }

        private static double Fade(double t)
        {
            // Fade function as defined by Ken Perlin.  This eases coordinate values
            // so that they will "ease" towards integral values.  This ends up smoothing
            // the final output.
            return t * t * t * (t * (t * 6 - 15) + 10);			    // 6t^5 - 15t^4 + 10t^3
        }

        private static double Lerp(double a, double b, double x)
        {
            return a + x * (b - a);
        }
    }
}
