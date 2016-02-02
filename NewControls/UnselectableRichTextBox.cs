using System;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public class UnselectableRichTextBox : RichTextBox
    {
        /// <summary>
        /// Unselectables the text box.
        /// </summary>
        public void UnselectableTextBox()
        {
            //Set it to read only by default
            ReadOnly = true;
        }

        /// <summary>
        /// Raises the <see cref="E:GotFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            //Prevent contents from being selected initally on focus
            DeselectAll();
        }

        /// <summary>
        /// WNDs the proc.
        /// </summary>
        /// <param name="m">The m.</param>
        protected override void WndProc(ref Message m)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_LBUTTONDOWN = 0x201;
            const int WM_LBUTTONDBLCLK = 0x203;
            const int WM_RBUTTONDOWN = 0x204;

            if ((m.Msg == WM_KEYDOWN) || (m.Msg == WM_LBUTTONDOWN) ||
                (m.Msg == WM_LBUTTONDBLCLK) || (m.Msg == WM_RBUTTONDOWN))
            {
                DeselectAll();
                return;
            }

            base.WndProc(ref m);
        }
    }
}