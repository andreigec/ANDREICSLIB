using System;
using System.Collections.Generic;
using System.Drawing;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Meal-Chooser
    /// </summary>
    public static class ColorExtras
    {
        /// <summary>
        /// do the colours match
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns></returns>
        public static bool Equals(this Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }

        /// <summary>
        /// Converts the hexadecimal string to colour.
        /// </summary>
        /// <param name="hexstring">The hexstring.</param>
        /// <returns></returns>
        public static Color? ConvertHexStringToColour(string hexstring)
        {
            try
            {
                var color = Convert.ToInt32(hexstring, 16);
                var c = Color.FromArgb(color);
                c = Color.FromArgb(255, c.R, c.G, c.B);
                return c;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the negative colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns></returns>
        public static Color GetNegativeColour(Color colour)
        {
            return Color.FromArgb(colour.A, 255 - colour.R,
                255 - colour.G, 255 - colour.B);
        }

        /// <summary>
        /// Gets a red green blended colour from passed in int.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static Color GetRedGreenBlendedColour(int p, int min = -100, int max = 100)
        {
            var r = 0;
            var g = 0;
            var b = 0;
            if (p < 0)
            {
                var rv = p;
                if (rv < min)
                    rv = min;
                r = ((int)((rv / (float)min) * 255.0));
            }
            else if (p > 0)
            {
                var gv = p;
                if (gv > max)
                    gv = max;
                g = ((int)((gv / (float)max) * 255.0));
            }
            var ret = Color.FromArgb(r, g, b);
            return ret;
        }

        /// <summary>
        /// Gets the nearest colour.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="possibleColours">The possible colours.</param>
        /// <returns></returns>
        public static Color GetNearestColour(Color c1, List<Color> possibleColours)
        {
            if (possibleColours.Count == 0)
                return Color.Transparent;

            var dblInputRed = Convert.ToDouble(c1.R);
            var dblInputGreen = Convert.ToDouble(c1.G);
            var dblInputBlue = Convert.ToDouble(c1.B);
            var distance = -1.0;
            var nearestColor = Color.Empty;

            foreach (var c in possibleColours)
            {
                var dblTestRed = Math.Pow(Convert.ToDouble(c.R) - dblInputRed, 2.0);
                var dblTestGreen = Math.Pow(Convert.ToDouble(c.G) - dblInputGreen, 2.0);
                var dblTestBlue = Math.Pow(Convert.ToDouble(c.B) - dblInputBlue, 2.0);
                var temp = Math.Sqrt(dblTestBlue + dblTestGreen + dblTestRed);

                if (temp < distance || distance < 0)
                {
                    distance = temp;
                    nearestColor = c;
                }
            }
            return nearestColor;
        }
    }
}