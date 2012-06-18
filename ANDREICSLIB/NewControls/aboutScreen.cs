using System;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public partial class aboutScreen : Form
	{
		public aboutScreen()
		{
			InitializeComponent();
		}

		private void aboutScreen_Load(object sender, EventArgs e)
		{
			closebutton.Select();
		}

		private void closebutton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void otherapptext_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);
		}

		private void aboutScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			Licensing.ShowingAbout = false;
		}

		
	}
}
