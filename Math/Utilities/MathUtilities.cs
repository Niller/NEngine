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
    }
}
