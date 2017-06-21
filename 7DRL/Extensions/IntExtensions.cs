using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Components
{
    public static class IntExtensions
    {
        public static int Clamp(this int self, int lowerBound, int upperBound)
        {
            if (self < lowerBound) self = lowerBound;
            if (self > upperBound) self = upperBound;

            return self;
        }

        public static int Normalize(this int self, int lowerBound, int upperBound, int minNum, int maxNum)
        {
            var num = self;

            num.Clamp(lowerBound, upperBound);

            num = minNum + (num - lowerBound) * (maxNum - minNum) / (upperBound - lowerBound);

            return num;
        }

        public static int NormalizeClampless(this int self, int lowerBound, int upperBound, int minNum, int maxNum)
        {
            var num = self;

            num = minNum + (num - lowerBound) * (maxNum - minNum) / (upperBound - lowerBound);

            return num;
        }
    }
}
