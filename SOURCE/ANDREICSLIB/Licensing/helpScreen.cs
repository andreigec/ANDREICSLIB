using System;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public partial class helpScreen : Form
	{
		public helpScreen()
		{
			InitializeComponent();
			AutoSize = true;
		}

		public override sealed bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		private void closebutton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void helpScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			Licensing.ShowingHelp = false;
		}
	}
}