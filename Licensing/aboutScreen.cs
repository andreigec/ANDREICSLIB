using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class AboutScreen : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutScreen"/> class.
        /// </summary>
        public AboutScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the aboutScreen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void aboutScreen_Load(object sender, EventArgs e)
        {
            closebutton.Select();
        }

        /// <summary>
        /// Handles the Click event of the closebutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void closebutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the otherapptext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LinkClickedEventArgs"/> instance containing the event data.</param>
        private void otherapptext_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        /// <summary>
        /// Handles the FormClosing event of the aboutScreen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void aboutScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            LicensingHelpers.ShowingAbout = false;
        }
    }
}