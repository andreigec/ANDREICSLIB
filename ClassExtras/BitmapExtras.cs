using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/MTG-Proxy-Maker
    /// </summary>
    public class BitmapExtras
    {

        /// <summary>
        /// Applies the grayscale.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        public static Bitmap ApplyGrayscale(Bitmap bitmap)
        {
            int rgb;
            Color c;

            for (var y = 0; y < bitmap.Height; y++)
                for (var x = 0; x < bitmap.Width; x++)
                {
                    c = bitmap.GetPixel(x, y);
                    rgb = (c.R + c.G + c.B) / 3;
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return bitmap;
        }

        /// <summary>
        /// Initializes the bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="defaultColour">The default colour.</param>
        /// <returns></returns>
        public static Bitmap InitBitmap(int width, int height, Color defaultColour)
        {
            var b = new Bitmap(width, height);
            for (var y = 0; y < b.Height; y++)
            {
                for (var x = 0; x < b.Width; x++)
                {
                    b.SetPixel(x, y, defaultColour);
                }
            }
            return b;
        }

        /// <summary>
        /// Changes all non white pixels to black.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        public static Bitmap ChangeAllNonWhitePixelsToColour(Bitmap bitmap, Color replace)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var p = bitmap.GetPixel(x, y);
                    var white = !(p.R != 255 || p.G != 255 || p.B != 255);
                    bitmap.SetPixel(x, y, white ? Color.White : replace);
                }
            }
            return bitmap;
        }



        /// <summary>
        /// Transforms all grey pixels to black.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Bitmap TransformAllGreyPixelsToBlack(Bitmap bitmap, int r, int g, int b)
        {
            //int rgb;
            Color c;

            for (var y = 0; y < bitmap.Height; y++)
                for (var x = 0; x < bitmap.Width; x++)
                {
                    c = bitmap.GetPixel(x, y);
                    var rgb = 255;
                    const int blackd = 50;
                    if ((c.R <= blackd && c.G <= blackd && c.B <= blackd) || (c.R == r && c.G == g && c.B == b))
                    {
                        rgb = 0;
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return bitmap;
        }


        /// <summary>
        /// Loads the file to colour matrix.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static Color[][] LoadFileToColourMatrix(string filename)
        {
            try
            {
                var fs = new FileStream(filename, FileMode.Open);

                var s = Image.FromStream(fs);
                var b = new Bitmap(s);

                return ConvertBitmapToColourMatrix(b);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Stretches the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Bitmap StretchBitmap(Bitmap bitmap, int width, int height)
        {
            var ret = new Bitmap(bitmap, width, height);
            return ret;
        }

        /// <summary>
        /// Resizes the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="antiAlias">if set to <c>true</c> [anti alias].</param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height, bool antiAlias = false)
        {
            var bmp = new Bitmap(width, height);
            var graph = Graphics.FromImage(bmp);
            graph.InterpolationMode = antiAlias ? InterpolationMode.High : InterpolationMode.NearestNeighbor;
            graph.CompositingQuality = antiAlias ? CompositingQuality.HighQuality : CompositingQuality.Default;
            graph.SmoothingMode = antiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
            graph.DrawImage(bitmap, new Rectangle(0, 0, width, height));
            return bmp;
        }

        /// <summary>
        /// Converts the bitmap to a colour matrix.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private static Color[][] ConvertBitmapToColourMatrix(Bitmap b)
        {
            var w = b.Width;
            var h = b.Height;

            var ret = new Color[h][];

            for (var y = 0; y < h; y++)
            {
                ret[y] = new Color[w];
                for (var x = 0; x < w; x++)
                {
                    var p = b.GetPixel(x, y);
                    ret[y][x] = Color.FromArgb(p.R, p.G, p.B);
                }
            }

            return ret;
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="pointXYOne">The point xy one.</param>
        /// <param name="pointXYTwo">The point xy two.</param>
        public static void DrawLine(Bitmap bitmap, Color colour, Tuple<int, int> pointXYOne, Tuple<int, int> pointXYTwo)
        {
            var x = pointXYOne.Item1;
            var y = pointXYOne.Item2;
            while (x != pointXYTwo.Item1 || y != pointXYTwo.Item2)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                bitmap.SetPixel(x, y, colour);
                if (x > pointXYTwo.Item1)
                    x--;
                else if (x < pointXYTwo.Item1)
                    x++;

                if (y > pointXYTwo.Item2)
                    y--;
                else if (y < pointXYTwo.Item2)
                    y++;
            }
        }

        /// <summary>
        /// Locks the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="doLock">if set to <c>true</c> [do lock].</param>
        public static void LockBitmap(Bitmap bitmap, bool doLock)
        {
            bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        }

        /// <summary>
        /// crop the image based on a new width and height
        /// </summary>
        /// <param name="bitmap">in image</param>
        /// <param name="newWidth">a size, or -1 for the input size</param>
        /// <param name="newHeight">a size, or -1 for the input size</param>
        /// <param name="startx"> </param>
        /// <param name="starty"> </param>
        /// <returns></returns>
        public static Bitmap Crop(Bitmap bitmap, int newWidth = -1, int newHeight = -1, int startx = 0, int starty = 0)
        {
            var w = newWidth == -1 ? bitmap.Width : newWidth;
            var h = newHeight == -1 ? bitmap.Height : newHeight;
            var rect = new Rectangle(startx, starty, w, h);

            return bitmap.Clone(rect, PixelFormat.DontCare);
        }

        /// <summary>
        /// Determines whether [is all pixels a colour] [the specified bitmap].
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="colour">The colour.</param>
        /// <returns></returns>
        public static bool IsAllPixelsAColour(Bitmap bitmap, Color colour)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                if (IsRowOrColSameColour(bitmap, y, true, colour) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the bitmap has the entire row/col of the same colour
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="rowOrCol">The row or col.</param>
        /// <param name="isRow">if set to <c>true</c> [is row].</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static bool IsRowOrColSameColour(Bitmap bitmap, int rowOrCol, bool isRow, Color c)
        {
            var w = bitmap.Width;
            var h = bitmap.Height;
            var to = isRow ? w : h;

            for (var v = 0; v < to; v++)
            {
                var c2 = isRow ? bitmap.GetPixel(v, rowOrCol) : bitmap.GetPixel(rowOrCol, v);

                if (c2.R != c.R || c2.G != c.G || c2.B != c.B)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Removes the excess whitespace.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="keepPureWhite">if set to <c>true</c>, wont throw an exception if entire bitmap is white</param>
        /// <param name="padx">The padx.</param>
        /// <param name="pady">The pady.</param>
        /// <returns></returns>
        /// <exception cref="Exception">error resizing</exception>
        public static Bitmap RemoveExcessWhitespace(Bitmap bitmap, bool keepPureWhite = true, int padx = 0, int pady = 0)
        {
            //remove rows of white
            var starty = -1;
            var endy = -1;
            var startx = -1;
            var endx = -1;
            for (var y = 0; y < bitmap.Height; y++)
            {
                if (starty == -1)
                {
                    if (IsRowOrColSameColour(bitmap, y, true, Color.White) == false)
                        starty = y;
                }

                if (endy == -1)
                {
                    var ey = (bitmap.Height - 1) - y;
                    if (IsRowOrColSameColour(bitmap, ey, true, Color.White) == false)
                        endy = ey;
                }

                if (starty != -1 && endy != -1)
                    break;
            }

            for (var x = 0; x < bitmap.Width; x++)
            {
                if (startx == -1)
                {
                    if (IsRowOrColSameColour(bitmap, x, false, Color.White) == false)
                        startx = x;
                }

                if (endx == -1)
                {
                    var ex = (bitmap.Width - 1) - x;
                    if (IsRowOrColSameColour(bitmap, ex, false, Color.White) == false)
                        endx = ex;
                }

                if (startx != -1 && endx != -1)
                    break;
            }

            if (starty == -1 || startx == -1 || endy == -1 || endx == -1)
            {
                if (keepPureWhite)
                    return bitmap;
                throw new Exception("error resizing");
            }

            var nb = Crop(bitmap, endx - startx, endy - starty, startx, starty);

            return nb;
        }
    }
}