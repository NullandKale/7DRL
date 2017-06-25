using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Extensions;

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
    }
}
