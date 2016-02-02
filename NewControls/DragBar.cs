using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Timezone-Sleep-Converter
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class DragBar : UserControl
    {
        #region Delegates

        public delegate void BarEvent(DragBar entry);

        #endregion

        /// <summary>
        ///     x value for the end of the bar (set on paint)
        /// </summary>
        private int barEndX;

        /// <summary>
        ///     x value for the start of the bar (set on paint)
        /// </summary>
        private int barX;

        private bool mouseDownButton;
        private int mouseDownX;
        //dont set these
        private bool mouseMoveBar;
        private bool mouseResizeBar;
        private bool resizeleft;
        private List<scale> scales;
        ////////////////
        private int scaleWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragBar"/> class.
        /// </summary>
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

        /// <summary>
        /// Called when [bar value change].
        /// </summary>
        protected virtual void OnBarValueChange()
        {
            if (BarValueChange != null)
            {
                BarValueChange(this); // Notify Subscribers
            }
        }

        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <returns></returns>
        private int getFontSize()
        {
            var len = MaximumValue.ToString().Length;

            return (int) Font.Size*len + 5;
        }

        /// <summary>
        /// Handles the Paint event of the drawpanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        private void drawpanel_Paint(object sender, PaintEventArgs e)
        {
            var G = e.Graphics;
            G.Clear(BackColour);
            Brush b1 = new SolidBrush(BarColour);

            if (BarMaximumValue > BarMinimumValue)
            {
                var v1 = scales.Where(item => item.number == BarMinimumValue).First().xval;
                var v2 = scales.Where(item => item.number == BarMaximumValue).First().xval;
                var R = new Rectangle(v1, 0, v2 - v1, drawpanel.Height);
                G.FillRectangle(b1, R);
                barX = v1;
                barEndX = v2;
            }
            else
            {
                //start part
                var v2 = scales.Where(item => item.number == BarMaximumValue).First().xval;
                var R = new Rectangle(0, 0, v2, drawpanel.Height);
                G.FillRectangle(b1, R);

                //end part
                var v1 = scales.Where(item => item.number == BarMinimumValue).First().xval;
                var v3 = drawpanel.Width - v1;
                R = new Rectangle(v1, 0, v3, drawpanel.Height);
                G.FillRectangle(b1, R);

                barX = v1;
                barEndX = v2;
            }

            //int a = barX;

            Brush b2 = new SolidBrush(Color.Black);
            //draw number bars
            foreach (var s in scales)
            {
                G.FillRectangle(b2, s.xval, drawpanel.Height - 5, 2, 5);
            }
        }

        /// <summary>
        /// Handles the Resize event of the DragBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DragBar_Resize(object sender, EventArgs e)
        {
            InitScale();
            drawpanel.Invalidate();
            listpanel.Invalidate();
        }

        /// <summary>
        /// Initializes the scale.
        /// </summary>
        private void InitScale()
        {
            scales = new List<scale>();
            var width = getFontSize();
            //int count = 23;
            var count = listpanel.Width/width;
            if (count >= MaximumValue) count = MaximumValue;
            width = (listpanel.Width)/MaximumValue;
            scaleWidth = width;

            for (var a = 0; a <= MaximumValue; a++)
            {
                scales.Add(new scale {number = a, xval = width*a});
            }

            for (var a = 0; a < count; a++)
            {
                var val = (int) ((a/(float) count)*MaximumValue);
                scales[val].shownOnScale = true;
            }
        }

        /// <summary>
        /// Handles the Paint event of the listpanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        private void listpanel_Paint(object sender, PaintEventArgs e)
        {
            if (DrawScale == false)
                return;
            var G = e.Graphics;
            G.Clear(ScaleColour);
            Brush b1 = new HatchBrush(HatchStyle.DarkHorizontal, ForeColor);

            foreach (var v in scales.Where(item => item.shownOnScale))
            {
                G.DrawString(v.number.ToString(), Font, b1, v.xval, 0);
            }
        }

        /// <summary>
        /// Handles the Load event of the DragBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Wraps the bar.
        /// </summary>
        private void WrapBar()
        {
            BarMinimumValue %= MaximumValue;
            BarMaximumValue %= MaximumValue;
            if (BarMinimumValue < 0)
                BarMinimumValue = MaximumValue + BarMinimumValue;
            if (BarMaximumValue < 0)
                BarMaximumValue = MaximumValue + BarMaximumValue;
        }

        /// <summary>
        /// Handles the MouseDown event of the drawpanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void drawpanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (overCenter(e.X) == false)
                return;

            mouseDownX = e.X;
            mouseDownButton = true;
        }

        /// <summary>
        /// Handles the MouseUp event of the drawpanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void drawpanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownButton = false;
            mouseMoveBar = false;
            mouseResizeBar = false;
        }

        /// <summary>
        /// Overs the left resize.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private bool overLeftResize(int x)
        {
            return (x >= (barX - 10) && x <= (barX + 10));
        }

        /// <summary>
        /// Overs the right resize.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private bool overRightResize(int x)
        {
            return (x >= (barEndX - 10) && x <= (barEndX + 10));
        }

        /// <summary>
        /// Overs the center.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private bool overCenter(int x)
        {
            if (barEndX < barX)
            {
                return ((x >= 0 && x <= barEndX) || (x >= barX && x < drawpanel.Width));
            }
            return (x >= barX && x <= barEndX);
        }

        /// <summary>
        /// Handles the MouseMove event of the drawpanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
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

            var dif = 0;
            var forward = true;
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

            var dif2 = (double) dif/drawpanel.Width;

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