using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.ClassReplacements
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/XQueens
    /// </summary>
    public class PanelReplacement : Panel
    {
        public static int gap = 1;
        public List<Control> controlStack = new List<Control>();
        private int lastX;
        private int lastY;

        public static void SetGapBetweenItems(int val)
        {
            gap = val;
        }

        /// <summary>
        /// make sure the panel fits all the controls in it
        /// </summary>
        /// <param name="resize">The resize.</param>
        /// <param name="checkControls">The check controls.</param>
        /// <param name="extrawidth">The extrawidth.</param>
        /// <param name="extraheight">The extraheight.</param>
        public static void FitPanel(Control resize, Control checkControls, int extrawidth = 0, int extraheight = 0)
        {
            var maxx = -1;
            var maxy = -1;

            foreach (Control c in checkControls.Controls)
            {
                var w = c.Location.X + c.Width;
                var h = c.Location.Y + c.Height;
                if (maxx == -1 || w > maxx)
                    maxx = w;
                if (maxy == -1 || h > maxy)
                    maxy = h;
            }
            resize.Width = maxx + extrawidth;
            resize.Height = maxy + extraheight;
        }

        /// <summary>
        /// Removes the control.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveControl(string name)
        {
            //remove control
            Control rem = null;
            var index = -1;
            var a = 0;
            foreach (var C in controlStack)
            {
                if (C.Name.Equals(name))
                {
                    rem = C;
                    index = a;
                    break;
                }
                a++;
            }
            if (rem == null)
                return;

            var controlW = rem.Size.Width;
            var controlH = rem.Size.Height;
            //bool islast = (index == (controlStack.Count - 1));

            if (lastX > 0)
            {
                lastX -= controlW + gap;
                if (lastX < 0)
                    lastX = 0;
            }
            else
            {
                lastY -= controlH + gap;

                if (lastY < 0)
                    lastY = 0;
            }

            controlStack.Remove(rem);
            Controls.Remove(rem);

            //shift all controls down

            a = 0;
            foreach (var C in controlStack)
            {
                if (a >= index)
                {
                    var nx = C.Location.X - (controlW + gap);
                    var ny = C.Location.Y - (controlH + gap);

                    if (nx < 0)
                        nx = 0;

                    if (ny < 0)
                        ny = 0;

                    if (C.Location.X > 0)
                        C.Location = new Point(nx, C.Location.Y);
                    else
                        C.Location = new Point(nx, ny);
                }
                a++;
            }
        }

        /// <summary>
        /// Clears the controls.
        /// </summary>
        public void ClearControls()
        {
            lastX = 0;
            lastY = 0;
            Controls.Clear();
            controlStack.Clear();
        }

        /// <summary>
        /// Switches the control locations.
        /// </summary>
        /// <param name="indexone">The indexone.</param>
        /// <param name="indextwo">The indextwo.</param>
        public void SwitchControlLocations(int indexone, int indextwo)
        {
            var one = Controls[indexone];
            var two = Controls[indextwo];

            ListExtras.Swap(ref controlStack, indexone, indextwo);

            //switch the control locs
            var x = one.Location.X;
            var y = one.Location.Y;
            one.Location = new Point(two.Location.X, two.Location.Y);
            two.Location = new Point(x, y);

            //switch the item locs
            SwapControls(one, two);
        }

        /// <summary>
        /// Swaps the controls.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        public void SwapControls(Control one, Control two)
        {
            var alphaIndex = Controls.IndexOf(one);
            var betaIndex = Controls.IndexOf(two);
            Controls.SetChildIndex(one, betaIndex);
            Controls.SetChildIndex(two, alphaIndex);
        }

        /// <summary>
        /// Adds the control.
        /// </summary>
        /// <param name="C">The c.</param>
        /// <param name="nextItemOnSameLine">if set to <c>true</c> [next item on same line].</param>
        /// <param name="manualYAdd">The manual y add.</param>
        /// <param name="manualXAdd">The manual x add.</param>
        /// <returns></returns>
        public Control AddControl(Control C, bool nextItemOnSameLine, int manualYAdd = 0, int manualXAdd = 0)
        {
            int gapy = gap, gapx = gap;
            lastX += manualXAdd;
            lastY += manualYAdd;

            C.Location = new Point(C.Location.X + lastX, C.Location.Y + lastY);
            //make sure it has a unique name
            var rnext = 1;
            rnext += Controls.Count;

            if (C.Name.Length == 0)
                C.Name = "n" + rnext;

            if (nextItemOnSameLine == false)
            {
                lastY += C.Size.Height + gapy;
                lastX = 0;
            }
            else
            {
                lastX += C.Size.Width + gapx;
            }
            Controls.Add(C);
            controlStack.Add(C);
            return Controls[Controls.Count - 1];
        }

        /// <summary>
        /// Gets the name of the control by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Control GetControlByName(string name)
        {
            return controlStack.FirstOrDefault(c => c.Name.Equals(name));
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PanelReplacement Clone()
        {
            var O = new PanelReplacement();
            O.Width = Width;
            O.Height = Height;
            O.BackColor = BackColor;
            return O;
        }

        /// <summary>
        /// Removes the last control.
        /// </summary>
        public void RemoveLastControl()
        {
            if (controlStack.Count == 0)
                return;

            var last = controlStack[controlStack.Count - 1];
            controlStack.RemoveAt(controlStack.Count - 1);
            Controls.Remove(last);

            if (controlStack.Count > 0)
            {
                last = controlStack[controlStack.Count - 1];

                lastX = last.Location.X;
                lastY = last.Location.Y;

                if (lastX > 0)
                    lastX += last.Width + gap;
                else
                    lastY += last.Height + gap;
            }
            else
            {
                lastX = lastY = 0;
            }
        }

        #region border painting

        public Color borderColour = Color.Black;

        public int borderWidth;

        public PanelReplacement()
        {
            Paint += paintEvent;
        }

        [Description("The colour the border of the panel will have")]
        public Color BorderColour
        {
            get { return borderColour; }
            set { borderColour = value; }
        }

        [Description("The width of the border around the panel")]
        public int BorderWidth
        {
            get { return borderWidth; }
            set { borderWidth = value; }
        }

        private void paintEvent(object sender, PaintEventArgs e)
        {
            Pen p;
            if (borderWidth == 0)
                p = new Pen(BackColor);
            else
                p = new Pen(borderColour);
            e.Graphics.DrawRectangle(p,
                e.ClipRectangle.Left,
                e.ClipRectangle.Top,
                e.ClipRectangle.Width - borderWidth,
                e.ClipRectangle.Height - borderWidth);
        }

        #endregion

        #region position stack

        //x,y position stack

        public Stack<Tuple<int, int>> positionStack = new Stack<Tuple<int, int>>();

        public void pushPosition()
        {
            positionStack.Push(new Tuple<int, int>(lastX, lastY));
        }

        public void popPosition()
        {
            var x = positionStack.Pop();
            lastX = x.Item1;
            lastY = x.Item2;
        }


        public void bumpLastPosition(int x, int y)
        {
            lastX += x;
            lastY += y;
        }

        #endregion

        #region painting

        private const int WM_SETREDRAW = 0x000B;

        public void SuspendPaint()
        {
            SuspendPaint(this);
        }

        public void ResumePaint()
        {
            ResumePaint(this);
        }

        public static void SuspendPaint(Control control)
        {
            var msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumePaint(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            var wparam = new IntPtr(1);
            var msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }

        #endregion painting
    }
}