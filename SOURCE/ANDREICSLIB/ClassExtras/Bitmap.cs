using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ANDREICSLIB
{
    public class BitmapUpdates
    {
        public static Color[][] ImageFileToColorMatrix(String filename)
        {
            try
            {
                var fs = new FileStream(filename, FileMode.Open);

                Image s = Image.FromStream(fs);
                var b = new Bitmap(s);

                return BitmapToColorMatrix(b);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Color[][] BitmapToColorMatrix(Bitmap b)
        {
            int w = b.Width;
            int h = b.Height;

            var ret = new Color[h][];

            for (int y = 0; y < h; y++)
            {
                ret[y] = new Color[w];
                for (int x = 0; x < w; x++)
                {
                    Color p = b.GetPixel(x, y);
                    ret[y][x] = Color.FromArgb(p.R, p.G, p.B);
                }
            }

            return ret;
        }

        public static void drawLine(Bitmap b, Color c, Tuple<int, int> one, Tuple<int, int> two)
        {
            int x = one.Item1;
            int y = one.Item2;
            while (x != two.Item1 || y != two.Item2)
            {
                if (x < 0 || y < 0 || x >= b.Width || y >= b.Height)
                    return;
                b.SetPixel(x, y, c);
                if (x > two.Item1)
                    x--;
                else if (x < two.Item1)
                    x++;

                if (y > two.Item2)
                    y--;
                else if (y < two.Item2)
                    y++;
            }
        }

        public static void lockBitmap(Bitmap b, bool doLock)
        {
            b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);
        }
    }
}