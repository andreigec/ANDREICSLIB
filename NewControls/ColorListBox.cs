using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Windows.Forms.ListBox" />
    public class ColorListBox : ListBox
    {
        /// <summary>
        /// The colours
        /// </summary>
        public List<Color> colours;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorListBox"/> class.
        /// </summary>
        public ColorListBox()
        {
            DrawItem += DrawItemHandler;
        }

        /// <summary>
        /// Gets or sets the colours.
        /// </summary>
        /// <value>
        /// The colours.
        /// </value>
        [Description("The colours for each item of text")]
        public List<Color> Colours
        {
            get { return colours; }
            set { colours = value; }
        }

        /// <summary>
        /// Draws the item handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DrawItemEventArgs"/> instance containing the event data.</param>
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