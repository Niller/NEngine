using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Media;
using Math.Utilities;

namespace NEngine.Editor.Utilities
{
    public class ColorUtilities
    {
        public static Color Lerp(Color start, Color end, float ratio)
        {
            return new Color()
            {
                R = (byte) MathUtilities.Lerp(start.R, end.R, ratio),
                G = (byte) MathUtilities.Lerp(start.G, end.G, ratio),
                B = (byte) MathUtilities.Lerp(start.B, end.B, ratio),
                A = (byte) MathUtilities.Lerp(start.A, end.A, ratio),
            };
        }
    }
}