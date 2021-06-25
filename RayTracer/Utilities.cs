using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer
{
    public static class Utilities
    {
        public const double EPSILON = 0.00001;

        public static bool AlmostEqual(double value1, double value2)
        {
                return Math.Abs(value1 - value2) < EPSILON;
        }

        public static T[] Append<T>(T[] array, T item)
        {
            return new List<T>(array) { item }.ToArray();
        }

        public static T[] AppendMany<T>(T[] array, T[] items)
        {
            var currentItems = new List<T>(array);
            var newItems = new List<T>(items);
            currentItems.AddRange(newItems);
            return currentItems.ToArray();
        }

        public static bool ToStringEquals(string s1, string s2)
        {
            var s1WithoutId = CutOutId(s1);
            var s2WithoutId = CutOutId(s2);
            return s1WithoutId == s2WithoutId;
        }

        public static string CutOutId(string s1)
        {
            var idStart = s1.IndexOf("\nId:");
            var idEnd = s1.IndexOf("\n", idStart + 2);

            return s1.Substring(0, idStart) + s1.Substring(idEnd);
        }

        /*
        Solve cubic equations of the form ax^3 + bx^2 + cx + d = 0
        */
        public static double[][] SolveCubic(double a, double b, double c, double d)
        {
            var roots = new double[3][] {new double[2] {0, 0}, new double[2] {0, 0}, new double[2] {0, 0}};

            var f = ((3 * c / a) - (b * b / (a * a))) / 3.0;
            var g = ((2 * (b * b * b) / (a * a * a)) - (9 * b * c / (a * a)) + (27 * d / a)) / 27.0;
            var h = ((g * g) / 4.0) + ((f * f * f) / 27.0);
            
            if (h <= 0)
            {
                if (h == 0 && g == 0 && f == 0)
                {
                    // There are 3 real and equal roots
                    roots[0][0] = -Math.Cbrt(d / a);
                    roots[1][0] = -Math.Cbrt(d / a);
                    roots[2][0] = -Math.Cbrt(d / a);
                }
                else
                {
                    // There are 3 real roots
                    var i = Math.Sqrt((g * g / 4.0) - h);
                    var j = Math.Cbrt(i);
                    var k = Math.Acos(-g / (2 * i));
                    var bigL = -j;
                    var bigM = Math.Cos(k / 3.0);
                    var bigN = Math.Sqrt(3.0) * Math.Sin(k / 3.0);
                    var bigP = -b / (3.0 * a);
                    roots[0][0] = 2.0 * j * bigM + bigP;
                    roots[1][0] = bigL * (bigM + bigN) + bigP;
                    roots[2][0] = bigL * (bigM - bigN) + bigP;
                }
            }
            else
            {
                // Only 1 real root exists
                var bigP = -b / (3.0 * a);
                var bigR = -g / 2.0 + Math.Sqrt(h);
                var bigS = Math.Cbrt(bigR);
                var bigT = -g / 2.0 - Math.Sqrt(h);
                var bigU = Math.Cbrt(bigT);
                roots[0][0] = bigS + bigU + bigP;
                roots[1][0] = -(bigS + bigU) / 2.0 + bigP;
                roots[1][1] = (bigS - bigU) * Math.Sqrt(3.0) / 2.0;
                roots[2][0] = -(bigS + bigU) / 2.0 + bigP;
                roots[2][1] = -(bigS - bigU) * Math.Sqrt(3.0) / 2.0;
            }

            return roots;
        }

        /*
        Solve quartic equations of the form ax^4 + bx^3 + cx^2 + dx + e = 0
        */
        public static double[] SolveQuartic(double a, double b, double c, double d, double e)
        {
            var roots = new double[4][] {new double[2] {0, 0}, new double[2] {0, 0}, new double[2] {0, 0}, new double[2] {0, 0}};

            var f = c - (b * b * 3.0 / 8.0);
            var g = d + b * b * b / 8.0 - b * c / 2.0;
            var h = e - b * b * b * b * 3.0 / 256.0 + b * b * c / 16.0 - b * d / 4.0;

            var reducedCubicEquation = new double[4] {1, f / 2.0, (f * f - 4.0 * h) / 16.0, -g * g / 64.0};
            var rootsReducedCubic = SolveCubic(reducedCubicEquation[0], reducedCubicEquation[1], reducedCubicEquation[2], reducedCubicEquation[3]);

            for (int i = 0; i < 3; i++)
            {
                rootsReducedCubic[i][0] = Math.Round(rootsReducedCubic[i][0], 6, MidpointRounding.AwayFromZero);
                rootsReducedCubic[i][1] = Math.Round(rootsReducedCubic[i][1], 6, MidpointRounding.AwayFromZero);
            }

            if (rootsReducedCubic[0][1] == 0 && rootsReducedCubic[1][1] == 0 && rootsReducedCubic[2][1] == 0)
            {
                // Only real values
                if (rootsReducedCubic[0][0] >= 0 && rootsReducedCubic[1][0] >= 0 && rootsReducedCubic[2][0] >= 0)
                {
                    var rootsReducedCubicList = new List<double[]>(rootsReducedCubic);
                    var rootsReducedCubicSorted = rootsReducedCubicList.OrderBy(x => x[0]).ToArray();
                    var p = Math.Sqrt(rootsReducedCubicSorted[1][0]);
                    var q = Math.Sqrt(rootsReducedCubicSorted[2][0]);
                    var r = -g / (8 * p * q);
                    var s = b / (4 * a);

                    var valueRealX1 = p + q + r - s;
                    var valueImaginaryX1 = 0;
                    roots[0][0] = valueRealX1;
                    roots[0][1] = valueImaginaryX1;

                    var valueRealX2 = p - q - r - s;
                    var valueImaginaryX2 = 0;
                    roots[1][0] = valueRealX2;
                    roots[1][1] = valueImaginaryX2;

                    var valueRealX3 = -p + q - r - s;
                    var valueImaginaryX3 = 0;
                    roots[2][0] = valueRealX3;
                    roots[2][1] = valueImaginaryX3;

                    var valueRealX4 = -p - q + r - s;
                    var valueImaginaryX4 = 0;
                    roots[3][0] = valueRealX4;
                    roots[3][1] = valueImaginaryX4;
                }
                else
                {
                    var rootsReducedCubicSorted = new double[3][];
                    var index = 0;
                    foreach (var x in rootsReducedCubic)
                    {
                        if (x[0] < 0)
                        {
                            rootsReducedCubicSorted[index] = x;
                            index += 1;
                        }
                    }
                    foreach (var x in rootsReducedCubic)
                    {
                        if (x[0] > 0)
                        {
                            rootsReducedCubicSorted[index] = x;
                            index += 1;
                        }
                    }

                    var p = new double[] {0, Math.Sqrt(Math.Abs(rootsReducedCubicSorted[0][0]))};
                    var q = new double[] {0, Math.Sqrt(Math.Abs(rootsReducedCubicSorted[1][0]))};
                    var pq = -1.0 * p[1] * q[1];
                    var r = -g / (8 * pq);
                    var s = b / (4 * a);

                    var valueRealX1 = r - s;
                    var valueImaginaryX1 = p[1] + q[1];
                    roots[0][0] = valueRealX1;
                    roots[0][1] = valueImaginaryX1;

                    var valueRealX2 = -r - s;
                    var valueImaginaryX2 = p[1] - q[1];
                    roots[1][0] = valueRealX2;
                    roots[1][1] = valueImaginaryX2;

                    var valueRealX3 = -r - s;
                    var valueImaginaryX3 = q[1] - p[1];
                    roots[2][0] = valueRealX3;
                    roots[2][1] = valueImaginaryX3;

                    var valueRealX4 = r - s;
                    var valueImaginaryX4 = -p[1] - q[1];
                    roots[3][0] = valueRealX4;
                    roots[3][1] = valueImaginaryX4;
                }
            }
            else
            {
                // Imaginary part exists in reduced cubic equation
                // We don't want any imaginary solutions for our ray tracer, so return empty entries
                return new double[] {};
            }
            
            var rootsList = new List<double[]>(roots);
            var nonImaginaryRoots = rootsList.Where(x => x[1] == 0).ToArray();
            var results = new List<double>();
            for (int i = 0; i < nonImaginaryRoots.Length; i++)
            {
                results.Add(nonImaginaryRoots[i][0]);
            }
            return results.ToArray();
        }

        /*
        Solve quadratic equations of the form ax^2 + bx + c = 0
        */
        public static double[] Solve2(double coeffA, double coeffB, double coeffC)
        {
            // Normal form: x^2 + px + q = 0
            var p = coeffB / (2.0 * coeffA);
            var q = coeffC / coeffA;

            var D = p * p - q;

            if(Utilities.AlmostEqual(D, 0))
            {
                return new double[] {-p, -p};
            }
            else if (D < 0)
            {
                return new double[] {};
            }
            else
            {
                var sqrtD = Math.Sqrt(D);
                return new double[] {-sqrtD - p, sqrtD - p};
            }
        }

        /*
        Solve cubic equations of the form ax^3 + bx^2 + cx + d = 0
        */
        public static double[] Solve3(double coeffA, double coeffB, double coeffC, double coeffD)
        {
            // Normal form: x^3 + Ax^2 + Bx + D = 0
            var A = coeffB / coeffA;
            var B = coeffC / coeffA;
            var C = coeffD / coeffA;

            // Substitute x = y - A / 3 to eliminate quadratic term: x^3 + px + q = 0
            var aSquared = A * A;
            var p = 1.0 / 3.0 * (-1.0 / 3.0 * aSquared + B);
            var q = 1.0 / 2.0 * (2.0 / 27.0 * A * aSquared - 1.0 / 3.0 * A * B + C);

            // Cardano's Formula:
            var pCubed = p * p * p;
            var D = q * q + pCubed;
            var s = new List<double>();

            if(Utilities.AlmostEqual(D, 0))
            {
                if(Utilities.AlmostEqual(q, 0))
                {
                    // There is one single, triple solution
                    s.Add(0);
                }
                else
                {
                    // There is a single and a double solution
                    var u = Math.Cbrt(-q);
                    s.Add(2 * u);
                    s.Add(-u);
                }
            }
            else if (D < 0)
            {
                // 3 real, distinct solutions
                var phi = 1.0 / 3.0 * Math.Acos(-q / Math.Sqrt(-pCubed));
                var t = 2.0 * Math.Sqrt(-p);
                s.Add(t * Math.Cos(phi));
                s.Add(-t * Math.Cos(phi + Math.PI / 3.0));
                s.Add(-t * Math.Cos(phi - Math.PI / 3.0));
            }
            else
            {
                // Only 1 real solution
                var sqrtD = Math.Sqrt(D);
                var u = Math.Cbrt(sqrtD - q);
                var v = -Math.Cbrt(sqrtD + q);
                s.Add(u + v);
            }

            // Resubstitute values back in
            var sub = 1.0 / 3.0 * A;

            var results = new List<double>();

            foreach (var item in s)
            {
                results.Add(item - sub);
            }

            return results.ToArray();
        }

        /*
        Solve quartic equations of the form ax^4 + bx^3 + cx^2 + dx + e = 0
        */
        public static double[] Solve4(double coeffA, double coeffB, double coeffC, double coeffD, double coeffE)
        {
            // Reduce to normal form: x^4 + Ax^3 + Bx^2 + Cx + D = 0
            var A = coeffB / coeffA;
            var B = coeffC / coeffA;
            var C = coeffD / coeffA;
            var D = coeffE / coeffA;

            // Substitute x = y - A / 4 to eliminate the cubic term: x^4 + px^2 + qx + r = 0
            var aSquared = A * A;
            var p = -3.0 / 8.0 * aSquared + B;
            var q = 1.0 / 8.0 * aSquared * A - 1.0 / 2.0 * A * B + C;
            var r = -3.0 / 256.0 * aSquared * aSquared + 1.0 / 16.0 * aSquared * B - 1.0 / 4.0 * A * C + D;
            var s = new List<double>();

            if (Utilities.AlmostEqual(r, 0))
            {
                // There is no absolute term: y(y^3 + py + q) = 0
                s.AddRange(Utilities.Solve3(1, 0, p, q));
                s.Add(0);
            }
            else
            {
                // Solve resolvent cubic equation
                var cubicSolns = Utilities.Solve3(1, -1.0 / 2.0 * p, -r, 1.0 / 2.0 * r * p - 1.0 / 8.0 * q * q);

                // Take the single real solution...
                var z = cubicSolns[0];

                // ...and construct two quadratic equations
                var u = z * z - r;
                var v = 2.0 * z - p;

                if (Utilities.AlmostEqual(u, 0))
                {
                    u = 0.0;
                }
                else if (u > 0)
                {
                    u = Math.Sqrt(u);
                }
                else
                {
                    // No roots
                    return new double[] {};
                }

                if (Utilities.AlmostEqual(v, 0))
                {
                    v = 0.0;
                }
                else if (v > 0)
                {
                    v = Math.Sqrt(v);
                }
                else
                {
                    // No roots
                    return new double[] {};
                }

                // Solve first quadratic equation
                s.AddRange(Utilities.Solve2(1, q < 0 ? -v : v, z - u));

                // Solve second quadratic equation
                s.AddRange(Utilities.Solve2(1, q < 0 ? v : -v, z + u));
            }

            // Resubstitute values back in
            var sub = 1.0 / 4.0 * A;

            var results = new List<double>();

            foreach (var item in s)
            {
                results.Add(item - sub);
            }

            results.Sort();
            return results.ToArray();
        }
    }
}
