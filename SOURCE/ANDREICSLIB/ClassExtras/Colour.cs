using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
    public class ColorUpdates
    {
        public static bool TestColourByRGB(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }

        public static Color? HexColourToColour(String hex)
        {
            try
            {
                var color = Convert.ToInt32(hex, 16);
                var c = Color.FromArgb(color);
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

        private static Dictionary<int, Color> colourcache;
        public static Color getColourFromInt(int p, int min = -100, int max = 100)
        {
            if (colourcache == null)
                colourcache = new Dictionary<int, Color>();
            if (colourcache.ContainsKey(p))
                return colourcache[p];

            var r = 0;
            var g = 0;
            var b = 0;
            if (p < 0)
            {
                var rv = p;
                if (rv < min)
                    rv = min;
                r = ((int)(((float)rv / (float)min) * 255.0));
            }
            else if (p > 0)
            {
                var gv = p;
                if (gv > max)
                    gv = max;
                g = ((int)(((float)gv / (float)max) * 255.0));
            }
            var ret = Color.FromArgb(r, g, b);
            colourcache.Add(p, ret);
            return ret;
        }

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
