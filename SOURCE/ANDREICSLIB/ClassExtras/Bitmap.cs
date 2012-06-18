using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ANDREICSLIB
{
	public class BitmapUpdates
	{
        public static void drawLine(Bitmap b, Color c, Tuple<int, int> one, Tuple<int, int> two)
		{
			int x = one.Item1;
			int y = one.Item2;
			while (x!=two.Item1||y!=two.Item2)
			{
				if (x < 0 || y < 0 || x >= b.Width || y >= b.Height)
					return;
				b.SetPixel(x,y,c);
				if (x > two.Item1)
					x--;
				else if (x < two.Item1)
					x++;

				if (y > two.Item2)
					y--;
				else if (y<two.Item2)
				y++;
			}
		}

		public static void lockBitmap(Bitmap b,bool doLock)
		{
			b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);
		}
	}
}
