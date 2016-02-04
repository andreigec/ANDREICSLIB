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
        internal static int Gap = 1;
        internal List<Control> ControlStack = new List<Control>();
        internal int LastX;
        internal int LastY;

        /// <summary>
        /// Sets the gap between items.
        /// </summary>
        /// <param name="val">The value.</param>
        public static void SetGapBetweenItems(int val)
        {
            Gap = val;
        }

        /// <summary>
        /// Gets the control stack.
        /// </summary>
        /// <returns></returns>
        public List<Control> GetControlStack()
        {
            return ControlStack;
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
            foreach (var C in ControlStack)
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

            if (LastX > 0)
            {
                LastX -= controlW + Gap;
                if (LastX < 0)
                    LastX = 0;
            }
            else
            {
                LastY -= controlH + Gap;

                if (LastY < 0)
                    LastY = 0;
            }

            ControlStack.Remove(rem);
            Controls.Remove(rem);

            //shift all controls down

            a = 0;
            foreach (var C in ControlStack)
            {
                if (a >= index)
                {
                    var nx = C.Location.X - (controlW + Gap);
                    var ny = C.Location.Y - (controlH + Gap);

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
            LastX = 0;
            LastY = 0;
            Controls.Clear();
            ControlStack.Clear();
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

            ListExtras.Swap(ref ControlStack, indexone, indextwo);

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
            int gapy = Gap, gapx = Gap;
            LastX += manualXAdd;
            LastY += manualYAdd;

            C.Location = new Point(C.Location.X + LastX, C.Location.Y + LastY);
            //make sure it has a unique name
            var rnext = 1;
            rnext += Controls.Count;

            if (C.Name.Length == 0)
                C.Name = "n" + rnext;

            if (nextItemOnSameLine == false)
            {
                LastY += C.Size.Height + gapy;
                LastX = 0;
            }
            else
            {
                LastX += C.Size.Width + gapx;
            }
            Controls.Add(C);
            ControlStack.Add(C);
            return Controls[Controls.Count - 1];
        }

        /// <summary>
        /// Gets the name of the control by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Control GetControlByName(string name)
        {
            return ControlStack.FirstOrDefault(c => c.Name.Equals(name));
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
            if (ControlStack.Count == 0)
                return;

            var last = ControlStack[ControlStack.Count - 1];
            ControlStack.RemoveAt(ControlStack.Count - 1);
            Controls.Remove(last);

            if (ControlStack.Count > 0)
            {
                last = ControlStack[ControlStack.Count - 1];

                LastX = last.Location.X;
                LastY = last.Location.Y;

                if (LastX > 0)
                    LastX += last.Width + Gap;
                else
                    LastY += last.Height + Gap;
            }
            else
            {
                LastX = LastY = 0;
            }
        }

        #region border painting

        private Color _borderColour = Color.Black;

        private int _borderWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelReplacement"/> class.
        /// </summary>
        public PanelReplacement()
        {
            Paint += PaintEvent;
        }

        /// <summary>
        /// Gets or sets the border colour.
        /// </summary>
        /// <value>
        /// The border colour.
        /// </value>
        [Description("The colour the border of the panel will have")]
        public Color BorderColour
        {
            get { return _borderColour; }
            set { _borderColour = value; }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        [Description("The width of the border around the panel")]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; }
        }

        private void PaintEvent(object sender, PaintEventArgs e)
        {
            Pen p;
            if (_borderWidth == 0)
                p = new Pen(BackColor);
            else
                p = new Pen(_borderColour);
            e.Graphics.DrawRectangle(p,
                e.ClipRectangle.Left,
                e.ClipRectangle.Top,
                e.ClipRectangle.Width - _borderWidth,
                e.ClipRectangle.Height - _borderWidth);
        }

        #endregion

        #region position stack

        //x,y position stack
        private readonly Stack<Tuple<int, int>> positionStack = new Stack<Tuple<int, int>>();

        /// <summary>
        /// Pushes the position.
        /// </summary>
        public void PushPosition()
        {
            positionStack.Push(new Tuple<int, int>(LastX, LastY));
        }

        /// <summary>
        /// Pops the position.
        /// </summary>
        public void PopPosition()
        {
            var x = positionStack.Pop();
            LastX = x.Item1;
            LastY = x.Item2;
        }


        /// <summary>
        /// Bumps the last position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void BumpLastPosition(int x, int y)
        {
            LastX += x;
            LastY += y;
        }

        #endregion

        #region painting

        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// Suspends the paint.
        /// </summary>
        public void SuspendPaint()
        {
            SuspendPaint(this);
        }

        /// <summary>
        /// Resumes the paint.
        /// </summary>
        public void ResumePaint()
        {
            ResumePaint(this);
        }

        /// <summary>
        /// Suspends the paint.
        /// </summary>
        /// <param name="control">The control.</param>
        public static void SuspendPaint(Control control)
        {
            var msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Resumes the paint.
        /// </summary>
        /// <param name="control">The control.</param>
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