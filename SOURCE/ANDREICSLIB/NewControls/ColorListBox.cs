using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public class ColorListBox : ListBox
    {
        public List<Color> colours;

        public ColorListBox()
        {
            DrawItem += DrawItemHandler;
        }

        [Description("The colours for each item of text")]
        public List<Color> Colours
        {
            get { return colours; }
            set { colours = value; }
        }

        private void DrawItemHandler(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            SolidBrush sb;
            if (colours.Count < e.Index)
                sb = new SolidBrush(Color.Blue);
            else
                sb = new SolidBrush(colours[e.Index]);

            var f = new Font(FontFamily.GenericSansSerif,
                             8, FontStyle.Bold);
            e.Graphics.DrawString(Items[e.Index].ToString(), f, sb, e.Bounds);
        }
    }
}