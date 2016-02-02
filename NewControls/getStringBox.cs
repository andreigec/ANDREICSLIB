using System;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// example usage: https://github.com/andreigec/COD4-Server-Tool
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class GetStringBox : Form
    {
        public string returnvalue = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStringBox"/> class.
        /// </summary>
        public GetStringBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="labelText">The label text.</param>
        /// <param name="title">The title.</param>
        /// <param name="multiline">if set to <c>true</c> [multiline].</param>
        /// <returns></returns>
        public string ShowDialog(string labelText, string title, bool multiline = false)
        {
            Text = title;
            label1.Text = labelText;

            textBox1.Multiline = multiline;

            var starty = label1.Size.Height + 50;

            Height = 200 + starty;


            panel1.Height = starty;
            ShowDialog();
            return returnvalue;
        }

        /// <summary>
        /// Handles the Click event of the okbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okbutton_Click(object sender, EventArgs e)
        {
            returnvalue = textBox1.Text;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the cancelbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            returnvalue = null;
            Close();
        }

        /// <summary>
        /// Handles the KeyPress event of the textBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && textBox1.Multiline == false)
            {
                returnvalue = textBox1.Text;
                Close();
            }
        }
    }
}