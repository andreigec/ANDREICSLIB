using System;
using System.Windows.Forms;

namespace ANDREICSLIB.Licensing
{
    public partial class HelpScreen : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpScreen"/> class.
        /// </summary>
        public HelpScreen()
        {
            InitializeComponent();
            AutoSize = true;
        }

        /// <summary>
        /// Resize the form according to the setting of <see cref="P:System.Windows.Forms.Form.AutoSizeMode" />.
        /// </summary>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override sealed bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
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
        /// Handles the FormClosing event of the helpScreen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void helpScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Licensing.ShowingHelp = false;
        }
    }
}