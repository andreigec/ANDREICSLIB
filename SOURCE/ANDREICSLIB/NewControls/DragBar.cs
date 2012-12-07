using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public partial class DragBar : UserControl
    {
        #region Delegates

        public delegate void BarEvent(DragBar entry);

        #endregion

        /// <summary>
        /// x value for the end of the bar (set on paint)
        /// </summary>
        private int barEndX;

        /// <summary>
        /// x value for the start of the bar (set on paint)
        /// </summary>
        private int barX;

        private bool mouseDownButton;
        private int mouseDownX;
        //dont set these
        private bool mouseMoveBar;
        private bool mouseResizeBar;
        private bool resizeleft;
        ////////////////
        private int scaleWidth;
        private List<scale> scales;

        public DragBar()
        {
            BarColour = Color.Tomato;
            ScaleColour = Color.White;
            BackColour = Color.WhiteSmoke;
            DrawScale = true;
            BarMaximumValue = 6;
            BarMinimumValue = 4;
            MaximumValue = 10;
            MinimumValue = 0;
            InitializeComponent();
        }

        /////////////////

        [Description("The value the left of the control will be")]
        public int MinimumValue { get; set; }

        [Description("The value the right of the control will be")]
        public int MaximumValue { get; set; }

        [Description("The value the left of the bar will be")]
        public int BarMinimumValue { get; set; }

        [Description("The value the right of the bar will be")]
        public int BarMaximumValue { get; set; }

        [Description("Draw the scale on the bottom of the control")]
        public bool DrawScale { get; set; }

        [Description("The colour of the background")]
        public Color BackColour { get; set; }

        [Description("The colour of the scale")]
        public Color ScaleColour { get; set; }

        [Description("The colour of the bar")]
        public Color BarColour { get; set; }

        [Category("Action")]
        [Description("Triggers when the bar value changes")]
        public event BarEvent BarValueChange;

        protected virtual void OnBarValueChange()
        {
            if (BarValueChange != null)
            {
                BarValueChange(this); // Notify Subscribers
            }
        }

        private int getFontSize()
        {
            int len = MaximumValue.ToString().Length;

            return (int) Font.Size*len + 5;
        }

        private void drawpanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.Clear(BackColour);
            Brush b1 = new SolidBrush(BarColour);

            if (BarMaximumValue > BarMinimumValue)
            {
                int v1 = scales.Where(item => item.number == BarMinimumValue).First().xval;
                int v2 = scales.Where(item => item.number == BarMaximumValue).First().xval;
                var R = new Rectangle(v1, 0, v2 - v1, drawpanel.Height);
                G.FillRectangle(b1, R);
                barX = v1;
                barEndX = v2;
            }
            else
            {
                //start part
                int v2 = scales.Where(item => item.number == BarMaximumValue).First().xval;
                var R = new Rectangle(0, 0, v2, drawpanel.Height);
                G.FillRectangle(b1, R);

                //end part
                int v1 = scales.Where(item => item.number == BarMinimumValue).First().xval;
                int v3 = drawpanel.Width - v1;
                R = new Rectangle(v1, 0, v3, drawpanel.Height);
                G.FillRectangle(b1, R);

                barX = v1;
                barEndX = v2;
            }

            //int a = barX;

            Brush b2 = new SolidBrush(Color.Black);
            //draw number bars
            foreach (scale s in scales)
            {
                G.FillRectangle(b2, s.xval, drawpanel.Height - 5, 2, 5);
            }
        }

        private void DragBar_Resize(object sender, EventArgs e)
        {
            InitScale();
            drawpanel.Invalidate();
            listpanel.Invalidate();
        }

        private void InitScale()
        {
            scales = new List<scale>();
            int width = getFontSize();
            //int count = 23;
            int count = listpanel.Width/width;
            if (count >= MaximumValue) count = MaximumValue;
            width = (listpanel.Width)/MaximumValue;
            scaleWidth = width;

            for (int a = 0; a <= MaximumValue; a++)
            {
                scales.Add(new scale {number = a, xval = width*a});
            }

            for (int a = 0; a < count; a++)
            {
                var val = (int) ((a/(float) count)*MaximumValue);
                scales[val].shownOnScale = true;
            }
        }

        private void listpanel_Paint(object sender, PaintEventArgs e)
        {
            if (DrawScale == false)
                return;
            Graphics G = e.Graphics;
            G.Clear(ScaleColour);
            Brush b1 = new HatchBrush(HatchStyle.DarkHorizontal, ForeColor);

            foreach (scale v in scales.Where(item => item.shownOnScale))
            {
                G.DrawString(v.number.ToString(), Font, b1, v.xval, 0);
            }
        }

        private void DragBar_Load(object sender, EventArgs e)
        {
            if (DrawScale == false)
            {
                listpanel.Hide();
                drawpanel.Dock = DockStyle.Fill;
            }
            else
            {
                listpanel.Height = getFontSize();
            }
        }

        private void WrapBar()
        {
            BarMinimumValue %= MaximumValue;
            BarMaximumValue %= MaximumValue;
            if (BarMinimumValue < 0)
                BarMinimumValue = MaximumValue + BarMinimumValue;
            if (BarMaximumValue < 0)
                BarMaximumValue = MaximumValue + BarMaximumValue;
        }

        private void drawpanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (overCenter(e.X) == false)
                return;

            mouseDownX = e.X;
            mouseDownButton = true;
        }

        private void drawpanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownButton = false;
            mouseMoveBar = false;
            mouseResizeBar = false;
        }

        private bool overLeftResize(int x)
        {
            return (x >= (barX - 10) && x <= (barX + 10));
        }

        private bool overRightResize(int x)
        {
            return (x >= (barEndX - 10) && x <= (barEndX + 10));
        }

        private bool overCenter(int x)
        {
            if (barEndX < barX)
            {
                return ((x >= 0 && x <= barEndX) || (x >= barX && x < drawpanel.Width));
            }
            return (x >= barX && x <= barEndX);
        }

        private void drawpanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownButton == false)
            {
                if (overLeftResize(e.X) || overRightResize(e.X))
                    Cursor = Cursors.VSplit;
                else if (overCenter(e.X))
                    Cursor = Cursors.SizeAll;
                else
                    Cursor = Cursors.Default;

                return;
            }

            if (mouseMoveBar == false && mouseResizeBar == false)
            {
                if (overRightResize(mouseDownX))
                {
                    mouseResizeBar = true;
                    resizeleft = false;
                }
                else if (overLeftResize(mouseDownX))
                {
                    mouseResizeBar = true;
                    resizeleft = true;
                }
                else
                    mouseMoveBar = true;
            }

            if (mouseResizeBar == false && mouseMoveBar == false)
                return;

            int dif = 0;
            bool forward = true;
            if (e.X == mouseDownX)
                return;
            if (e.X > mouseDownX)
                dif = e.X - mouseDownX;
            else
            {
                dif = mouseDownX - e.X;
                forward = false;
            }

            if (dif < scaleWidth)
                return;

            double dif2 = (double) dif/drawpanel.Width;

            var amount = (int) (MaximumValue*dif2);
            if (amount == 0)
                return;
            if (forward == false)
                amount *= -1;

            if (mouseMoveBar)
            {
                BarMinimumValue += amount;
                BarMaximumValue += amount;
                OnBarValueChange();
            }

            else if (mouseResizeBar)
            {
                if (((BarMinimumValue + amount) != BarMaximumValue && resizeleft) ||
                    ((BarMaximumValue + amount) != BarMinimumValue && resizeleft == false))
                {
                    if (resizeleft)
                        BarMinimumValue += amount;
                    else
                        BarMaximumValue += amount;
                    OnBarValueChange();
                }
                else
                    mouseDownButton = false;
            }

            WrapBar();
            mouseDownX = e.X;
            drawpanel.Invalidate();
        }

        #region Nested type: scale

        private class scale
        {
            public int number;
            public bool shownOnScale;
            public int xval;
        }

        #endregion
    }
}