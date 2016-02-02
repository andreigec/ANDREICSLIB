using System;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Histogram-OCR-Trainer
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class GetStringImageCompare : Form
    {
        public string[] returnvalue;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStringImageCompare"/> class.
        /// </summary>
        public GetStringImageCompare()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="labelText">The label text.</param>
        /// <param name="title">The title.</param>
        /// <param name="i">The i.</param>
        /// <param name="maxlen">The maxlen.</param>
        /// <param name="il">The il.</param>
        /// <returns></returns>
        public string[] ShowDialog(string labelText, string title, Image i, int maxlen = -1,
            ImageLayout il = ImageLayout.Center)
        {
            Text = title;
            label.Text = labelText;
            imagepanel.BackgroundImage = i;
            imagepanel.BackgroundImageLayout = il;

            if (maxlen >= 0)
                textbox.MaxLength = maxlen;

            ShowDialog();
            return returnvalue;
        }

        /// <summary>
        /// Handles the Load event of the GetStringImageCompare control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GetStringImageCompare_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the okbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okbutton_Click(object sender, EventArgs e)
        {
            returnvalue = textbox.Text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            Close();
        }

        /// <summary>
        /// Handles the Click event of the cancelbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}