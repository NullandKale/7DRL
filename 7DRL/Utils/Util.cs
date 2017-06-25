using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Extensions;
using _7DRL.Utils;

namespace _7DRL
{
    class Util
    {
        public static T Choose<T>(T[] keys, float[] weights, Random rng)
        {
            var ran = ((float)rng.NextDouble()).Normalize(0, 1, 0, weights.Sum());

            var max = 0f;

            var target = 0;

            for (var i = 0; i < keys.Length; i++)
            {
                max += weights[i];

                if (ran < max)
                {
                    target = i;
                    break;
                }
            }

            return keys[target];
        }

        public static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(Game.g.rng.Next(v.Length));
        }

        public static double dist(int x1, int y1, int x2, int y2)
        {
            double x = Math.Pow(Math.Abs(x1 - x2), 2);
            double y = Math.Pow(Math.Abs(y1 - y2), 2);

            return Math.Sqrt(x + y);
        }

        public static List<Point> FloodFill(Tile[,] map, Point pt, int worldSize)
        {
            List<Point> badpoints = new List<Point>();
            List<Point> points = new List<Point>();
            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.x < worldSize - 1 && a.x > 0 &&
                        a.y < worldSize - 1 && a.y > 0)//make sure we stay within bounds
                {
                    if (map[a.x, a.y].Visual == ' ')
                    {
                        if (!points.Contains(new Point(a.x, a.y)))
                        {
                            points.Add(new Point(a.x, a.y));
                        }

                        if (!points.Contains(new Point(a.x - 1, a.y)) && map[a.x - 1, a.y].Visual == ' '
                            && !pixels.Contains(new Point(a.x - 1, a.y)) && !badpoints.Contains(new Point(a.x - 1, a.y)))
                        {
                            pixels.Push(new Point(a.x - 1, a.y));
                        }
                        if (!points.Contains(new Point(a.x + 1, a.y)) && map[a.x + 1, a.y].Visual == ' '
                            && !pixels.Contains(new Point(a.x + 1, a.y)) && !badpoints.Contains(new Point(a.x + 1, a.y)))
                        {
                            pixels.Push(new Point(a.x + 1, a.y));
                        }
                        if (!points.Contains(new Point(a.x, a.y - 1)) && map[a.x, a.y - 1].Visual == ' '
                            && !pixels.Contains(new Point(a.x, a.y - 1)) && !badpoints.Contains(new Point(a.x, a.y - 1)))
                        {
                            pixels.Push(new Point(a.x, a.y - 1));
                        }
                        if (!points.Contains(new Point(a.x + 1, a.y)) && map[a.x, a.y + 1].Visual == ' '
                            && !pixels.Contains(new Point(a.x, a.y + 1)) && !badpoints.Contains(new Point(a.x, a.y + 1)))
                        {
                            pixels.Push(new Point(a.x, a.y + 1));
                        }
                    }
                    else
                    {
                        badpoints.Add(a);
                    }
                }
                Console.WriteLine("Generating Map " + pixels.Peek().x + ", " + pixels.Peek().y);
            }

            return points;
        }
    }
}
