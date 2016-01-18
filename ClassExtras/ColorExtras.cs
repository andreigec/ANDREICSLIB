using System;
using System.Collections.Generic;
using System.Drawing;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Meal-Chooser
    /// </summary>
    public class ColorExtras
    {
        private static Dictionary<Tuple<int, int>, Dictionary<int, Color>> colourCache;

        public static bool TestColourByRGB(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }

        public static Color? HexColourToColour(string hex)
        {
            try
            {
                int color = Convert.ToInt32(hex, 16);
                Color c = Color.FromArgb(color);
                c = Color.FromArgb(255, c.R, c.G, c.B);
                return c;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Color getNegative(Color inColour)
        {
            return Color.FromArgb(inColour.A, 255 - inColour.R,
                                  255 - inColour.G, 255 - inColour.B);
        }

        public static Color getColourFromInt(int p, int min = -100, int max = 100)
        {
            if (colourCache == null)
                colourCache = new Dictionary<Tuple<int, int>, Dictionary<int, Color>>();

            //minmax tuple key
            var tk = new Tuple<int, int>(min, max);
            if (colourCache.ContainsKey(tk) == false)
                colourCache.Add(tk, new Dictionary<int, Color>());

            if (colourCache.ContainsKey(tk) && colourCache[tk].ContainsKey(p))
                return colourCache[tk][p];

            int r = 0;
            int g = 0;
            int b = 0;
            if (p < 0)
            {
                int rv = p;
                if (rv < min)
                    rv = min;
                r = ((int) ((rv/(float) min)*255.0));
            }
            else if (p > 0)
            {
                int gv = p;
                if (gv > max)
                    gv = max;
                g = ((int) ((gv/(float) max)*255.0));
            }
            Color ret = Color.FromArgb(r, g, b);
            colourCache[tk].Add(p, ret);
            return ret;
        }

        public static Color GetNearestColour(Color c1, List<Color> possibleColours)
        {
            if (possibleColours.Count == 0)
                return Color.Transparent;

            double dblInputRed = Convert.ToDouble(c1.R);
            double dblInputGreen = Convert.ToDouble(c1.G);
            double dblInputBlue = Convert.ToDouble(c1.B);
            double distance = -1.0;
            Color nearestColor = Color.Empty;

            foreach (Color c in possibleColours)
            {
                double dblTestRed = Math.Pow(Convert.ToDouble(c.R) - dblInputRed, 2.0);
                double dblTestGreen = Math.Pow(Convert.ToDouble(c.G) - dblInputGreen, 2.0);
                double dblTestBlue = Math.Pow(Convert.ToDouble(c.B) - dblInputBlue, 2.0);
                double temp = Math.Sqrt(dblTestBlue + dblTestGreen + dblTestRed);

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