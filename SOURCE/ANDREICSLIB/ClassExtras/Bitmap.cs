using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ANDREICSLIB
{
    public class BitmapExtras
    {
        /// <summary>
        /// convert a bitmap to grayscale
        /// </summary>
        /// <param name="Bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }

        /// <summary>
        /// initialise a bitmap with a width height and colour
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Bitmap InitBitmap(int width, int height, Color c)
        {
            var b = new Bitmap(width, height);
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    b.SetPixel(x, y, c);
                }
            }
            return b;
        }

        /// <summary>
        /// convert all RGB values in a bitmap that arent white, to black
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Bitmap NonWhiteToBlack(Bitmap b, Color c)
        {
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    var p = b.GetPixel(x, y);
                    bool white = !(p.R != 255 || p.G != 255 || p.B != 255);
                    b.SetPixel(x, y, white ? Color.White : Color.Black);
                }
            }
            return b;
        }

        /// <summary>
        /// replace all colours apart from the one passed in with white, and the passed in colour as black
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bitmap OnlyAllowBlackAndColour(Bitmap Bmp, int r, int g, int b)
        {
            //int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    int rgb = 255;
                    const int blackd = 50;
                    if ((c.R <= blackd && c.G <= blackd && c.B <= blackd) || (c.R == r && c.G == g && c.B == b))
                    {
                        rgb = 0;
                    }
                    //rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }

        /// <summary>
        /// load an image file, and convert to a 2d array of Colors
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        /// <summary>
        /// stretch an image to the desired width and height
        /// </summary>
        /// <param name="sourceBMP"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap StretchBitmap(Bitmap sourceBMP, int width, int height)
        {
            var ret = new Bitmap(sourceBMP, width, height);
            return ret;
        }

        /// <summary>
        /// resize a bitmap to the desired width and height.
        /// </summary>
        /// <param name="sourceBMP"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="antiAlias"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height, bool antiAlias = false)
        {
            var bmp = new Bitmap(width, height);
            var graph = Graphics.FromImage(bmp);
            graph.InterpolationMode = antiAlias ? InterpolationMode.High : InterpolationMode.NearestNeighbor;
            graph.CompositingQuality = antiAlias ? CompositingQuality.HighQuality : CompositingQuality.Default;
            graph.SmoothingMode = antiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
            graph.DrawImage(sourceBMP, new Rectangle(0, 0, width, height));
            return bmp;
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

        /// <summary>
        /// draw a line between to x/y coords in a bitmap
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        public static void DrawLine(Bitmap b, Color c, Tuple<int, int> one, Tuple<int, int> two)
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

        /// <summary>
        /// perform locking on a bitmap to increase read speed
        /// </summary>
        /// <param name="b"></param>
        /// <param name="doLock"></param>
        public static void LockBitmap(Bitmap b, bool doLock)
        {
            b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);
        }

        /// <summary>
        /// crop the image based on a new width and height
        /// </summary>
        /// <param name="inbit">in image</param>
        /// <param name="newWidth">a size, or -1 for the input size</param>
        /// <param name="newHeight">a size, or -1 for the input size</param>
        /// <param name="startx"> </param>
        /// <param name="starty"> </param>
        /// <returns></returns>
        public static Bitmap Crop(Bitmap inbit, int newWidth = -1, int newHeight = -1, int startx = 0, int starty = 0)
        {
            int w = newWidth == -1 ? inbit.Width : newWidth;
            int h = newHeight == -1 ? inbit.Height : newHeight;
            var rect = new Rectangle(startx, starty, w, h);

            return inbit.Clone(rect, PixelFormat.DontCare);
        }

        /// <summary>
        /// return true if the bitmap only contains a certain colour
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsOnlyColour(Bitmap b, Color c)
        {
            for (int y = 0; y < b.Height; y++)
            {
                if (RowOrColIsColour(b, y, true, c) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// return true if a row of column is a certain colour
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <param name="isRow"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool RowOrColIsColour(Bitmap b, int value, bool isRow, Color c)
        {
            int w = b.Width;
            int h = b.Height;
            int to = isRow ? w : h;

            for (int v = 0; v < to; v++)
            {
                Color c2;
                if (isRow)
                    c2 = b.GetPixel(v, value);
                else
                    c2 = b.GetPixel(value, v);


                if (c2.R != c.R || c2.G != c.G || c2.B != c.B)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// remove all straight whitespace around the outsides of an image. 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="keepPureWhite">if true, will return an image intact if it is only white</param>
        /// <param name="padx"></param>
        /// <param name="pady"></param>
        /// <returns></returns>
        public static Bitmap RemoveExcessWhitespace(Bitmap b, bool keepPureWhite = true, int padx = 0, int pady = 0)
        {
            //remove rows of white
            int starty = -1;
            int endy = -1;
            int startx = -1;
            int endx = -1;
            for (int y = 0; y < b.Height; y++)
            {
                if (starty == -1)
                {
                    if (RowOrColIsColour(b, y, true, Color.White) == false)
                        starty = y;
                }

                if (endy == -1)
                {
                    var ey = (b.Height - 1) - y;
                    if (RowOrColIsColour(b, ey, true, Color.White) == false)
                        endy = ey;
                }

                if (starty != -1 && endy != -1)
                    break;
            }

            for (int x = 0; x < b.Width; x++)
            {
                if (startx == -1)
                {
                    if (RowOrColIsColour(b, x, false, Color.White) == false)
                        startx = x;
                }

                if (endx == -1)
                {
                    var ex = (b.Width - 1) - x;
                    if (RowOrColIsColour(b, ex, false, Color.White) == false)
                        endx = ex;
                }

                if (startx != -1 && endx != -1)
                    break;
            }

            if (starty == -1 || startx == -1 || endy == -1 || endx == -1)
            {
                if (keepPureWhite)
                    return b;
                throw new Exception("error resizing");
            }

            var nb = Crop(b, endx - startx, endy - starty, startx, starty);

            return nb;
        }
    }
}