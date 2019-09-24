using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math.Utilities
{
    public static class MathUtilities
    {
        public static bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static float Lerp(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        public static float Clamp(float value, float min = 0, float max = 1)
        {
            return System.Math.Max(min, System.Math.Min(value, max));
        }
    }
}
