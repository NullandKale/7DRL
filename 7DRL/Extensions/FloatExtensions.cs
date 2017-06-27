namespace _7DRL.Extensions
{
    public static class FloatExtensions
    {
        public static float Clamp(this float self, float lowerBound, float upperBound)
        {
            if (self < lowerBound)
            {
                self = lowerBound;
            }

            if (self > upperBound)
            {
                self = upperBound;
            }

            return self;
        }

        public static float Normalize(this float self, float lowerBound, float upperBound, float minNum, float maxNum)
        {
            var num = self;

            num.Clamp(lowerBound, upperBound);

            num = minNum + ((num - lowerBound) * (maxNum - minNum) / (upperBound - lowerBound));

            return num;
        }

        public static float NormalizeClampless(this float self, float lowerBound, float upperBound, float minNum, float maxNum)
        {
            var num = self;

            num = minNum + ((num - lowerBound) * (maxNum - minNum) / (upperBound - lowerBound));

            return num;
        }
    }
}
