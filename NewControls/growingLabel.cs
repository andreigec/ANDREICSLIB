using System;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class GrowingLabel : Label
    {
        private bool mGrowing;

        public GrowingLabel()
        {
            AutoSize = false;
        }

        private void resizeLabel()
        {
            if (mGrowing) return;
            try
            {
                mGrowing = true;
                var sz = new Size(Width, Int32.MaxValue);
                sz = TextRenderer.MeasureText(Text, Font, sz, TextFormatFlags.WordBreak);
                Height = sz.Height;
            }
            finally
            {
                mGrowing = false;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            resizeLabel();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            resizeLabel();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            resizeLabel();
        }
    }
}