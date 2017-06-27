namespace _7DRL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Utils;

    public static class Util
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

        public static double Dist(int x1, int y1, int x2, int y2)
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
                if (a.X < worldSize - 1 && a.X > 0 && a.Y < worldSize - 1 && a.Y > 0)
                {
                    // make sure we stay within bounds
                    if (map[a.X, a.Y].Visual == ' ')
                    {
                        if (!points.Contains(new Point(a.X, a.Y)))
                        {
                            points.Add(new Point(a.X, a.Y));
                        }

                        if (!points.Contains(new Point(a.X - 1, a.Y)) && map[a.X - 1, a.Y].Visual == ' '
                            && !pixels.Contains(new Point(a.X - 1, a.Y)) && !badpoints.Contains(new Point(a.X - 1, a.Y)))
                        {
                            pixels.Push(new Point(a.X - 1, a.Y));
                            badpoints.Add(new Point(a.X - 1, a.Y));
                        }

                        if (!points.Contains(new Point(a.X + 1, a.Y)) && map[a.X + 1, a.Y].Visual == ' '
                            && !pixels.Contains(new Point(a.X + 1, a.Y)) && !badpoints.Contains(new Point(a.X + 1, a.Y)))
                        {
                            pixels.Push(new Point(a.X + 1, a.Y));
                            badpoints.Add(new Point(a.X + 1, a.Y));
                        }

                        if (!points.Contains(new Point(a.X, a.Y - 1)) && map[a.X, a.Y - 1].Visual == ' '
                            && !pixels.Contains(new Point(a.X, a.Y - 1)) && !badpoints.Contains(new Point(a.X, a.Y - 1)))
                        {
                            pixels.Push(new Point(a.X, a.Y - 1));
                            badpoints.Add(new Point(a.X, a.Y - 1));
                        }

                        if (!points.Contains(new Point(a.X, a.Y + 1)) && map[a.X, a.Y + 1].Visual == ' '
                            && !pixels.Contains(new Point(a.X, a.Y + 1)) && !badpoints.Contains(new Point(a.X, a.Y + 1)))
                        {
                            pixels.Push(new Point(a.X, a.Y + 1));
                            badpoints.Add(new Point(a.X, a.Y + 1));
                        }
                    }
                    else
                    {
                        badpoints.Add(a);
                    }
                }
            }

            return points;
        }
    }
}
