using System;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class GrowingLabel : Label
    {
        private bool mGrowing;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingLabel"/> class.
        /// </summary>
        public GrowingLabel()
        {
            AutoSize = false;
        }

        /// <summary>
        /// Resizes the label.
        /// </summary>
        private void resizeLabel()
        {
            if (mGrowing) return;
            try
            {
                mGrowing = true;
                var sz = new Size(Width, int.MaxValue);
                sz = TextRenderer.MeasureText(Text, Font, sz, TextFormatFlags.WordBreak);
                Height = sz.Height;
            }
            finally
            {
                mGrowing = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:TextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            resizeLabel();
        }

        /// <summary>
        /// Raises the <see cref="E:FontChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            resizeLabel();
        }

        /// <summary>
        /// Raises the <see cref="E:SizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            resizeLabel();
        }
    }
}