using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB
{
    public class PanelReplacement : Panel
    {
        #region border painting

        public Color borderColour = Color.Black;

        public int borderWidth = 0;

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
            Tuple<int, int> x = positionStack.Pop();
            lastX = x.Item1;
            lastY = x.Item2;
        }


        public void bumpLastPosition(int x, int y)
        {
            lastX += x;
            lastY += y;
        }

        #endregion

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
        public static void fitPanel(Control resize, Control checkControls, int extrawidth = 0, int extraheight = 0)
        {
            int maxx = -1;
            int maxy = -1;

            foreach (Control c in checkControls.Controls)
            {
                int w = c.Location.X + c.Width;
                int h = c.Location.Y + c.Height;
                if (maxx == -1 || w > maxx)
                    maxx = w;
                if (maxy == -1 || h > maxy)
                    maxy = h;
            }
            resize.Width = maxx + extrawidth;
            resize.Height = maxy + extraheight;
        }

        public void removeControl(String name)
        {
            //remove control
            Control rem = null;
            int index = -1;
            int a = 0;
            foreach (Control C in controlStack)
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

            int controlW = rem.Size.Width;
            int controlH = rem.Size.Height;
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
            foreach (Control C in controlStack)
            {
                if (a >= index)
                {
                    int nx = C.Location.X - (controlW + gap);
                    int ny = C.Location.Y - (controlH + gap);

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

        public void clearControls()
        {
            lastX = 0;
            lastY = 0;
            Controls.Clear();
            controlStack.Clear();
        }

        public void switchControlLocations(int indexone, int indextwo)
        {
            Control one = Controls[indexone];
            Control two = Controls[indextwo];

            ListUpdates.Swap(ref controlStack, indexone, indextwo);

            //switch the control locs
            int x = one.Location.X;
            int y = one.Location.Y;
            one.Location = new Point(two.Location.X, two.Location.Y);
            two.Location = new Point(x, y);

            //switch the item locs
            swapControls(one, two);
        }

        public void swapControls(Control one, Control two)
        {
            int alphaIndex = Controls.IndexOf(one);
            int betaIndex = Controls.IndexOf(two);
            Controls.SetChildIndex(one, betaIndex);
            Controls.SetChildIndex(two, alphaIndex);
        }

        public Control addControl(Control C, bool nextItemOnSameLine, int manualYAdd = 0, int manualXAdd = 0)
        {
            int gapy = gap, gapx = gap;
            lastX += manualXAdd;
            lastY += manualYAdd;

            C.Location = new Point(C.Location.X + lastX, C.Location.Y + lastY);
            //make sure it has a unique name
            int rnext = 1;
            rnext += Controls.Count;

            if (C.Name.Length == 0)
                C.Name = "n" + rnext.ToString();

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

        public Control getControlByName(String name)
        {
            return controlStack.FirstOrDefault(c => c.Name.Equals(name));
        }

        public PanelReplacement Clone()
        {
            var O = new PanelReplacement();
            O.Width = Width;
            O.Height = Height;
            O.BackColor = BackColor;
            return O;
        }

        public void removeLastControl()
        {
            if (controlStack.Count == 0)
                return;

            Control last = controlStack[controlStack.Count - 1];
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
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                                                      IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumePaint(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            var wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                                                     IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }

        #endregion painting
    }
}