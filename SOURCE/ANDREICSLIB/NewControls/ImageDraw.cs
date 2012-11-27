using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class ImageDraw : UserControl
    {
        public ImageDraw()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// translate a position over the image to the location on the original image
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetRealPos(Control c, int x, int y)
        {
            //get actual image end pos
            var v21 = new Vector(c.BackgroundImage.Width, c.BackgroundImage.Height);
            var mm2 = MathUpdates.ConvertMatrix(GetMatrix(c));
            var v22 = Vector.Multiply(v21, mm2);

            //if cursor is outside, ignore
            if (x > v22.X || y > v22.Y)
                return null;

            //get percentages
            var px = x / v22.X;
            var py = y / v22.Y;

            //get original image translated pos
            var ox = c.BackgroundImage.Width * px;
            var oy = c.BackgroundImage.Height * py;

            var oxi = (int)Math.Round(ox);
            var oyi = (int) Math.Round(oy);

            return new Tuple<int, int>(oxi,oyi);
        }

        public static Matrix GetMatrix(Control c)
        {
            double heightscale = 1;
            double widthscale = 1;

            if (c.BackgroundImageLayout == ImageLayout.Stretch || c.BackgroundImageLayout == ImageLayout.Zoom)
            {
                heightscale = (double)c.Height / c.BackgroundImage.Height;
                widthscale = (double)c.Width / c.BackgroundImage.Width;

                if (c.BackgroundImageLayout == ImageLayout.Zoom)
                {
                    if (heightscale < widthscale)
                    {
                        widthscale = heightscale;
                    }
                    else
                    {
                        heightscale = widthscale;
                    }
                }
            }

            while (((c.BackgroundImage.Width*widthscale)>c.Width)||
                ((c.BackgroundImage.Height*heightscale)>c.Height))
            {
                widthscale -= 0.02;
                heightscale -= 0.02;
            }

            //make sure it fits
            widthscale = Math.Round(widthscale, 2);
            heightscale = Math.Round(heightscale, 2);

            return new Matrix((float)widthscale, 0, 0, (float)heightscale, 0, 0);   
        }

        public static void PaintF(Graphics g,Control c)
        {
            g.Clear(c.BackColor);

            if (c.BackgroundImage == null)
                return;
           
            g.Transform = GetMatrix(c);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(c.BackgroundImage, new Rectangle(0, 0, c.BackgroundImage.Width, c.BackgroundImage.Height), 0, 0, c.BackgroundImage.Width, c.BackgroundImage.Height, GraphicsUnit.Pixel); 
        }

        public static void PaintEvent(object sender, PaintEventArgs e)
        {
            PaintF(e.Graphics, sender as Control);
        }
    }
}
