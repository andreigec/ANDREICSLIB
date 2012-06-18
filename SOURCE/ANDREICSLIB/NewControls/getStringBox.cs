using System;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public partial class getStringBox : Form
	{
		public String returnvalue = "";

		public getStringBox()
		{
			InitializeComponent();
		}

        public String ShowDialog(String labelText, String title)
        {
            Text = title;
            label1.Text = labelText;

            
            ShowDialog();
			return returnvalue;
        }

		private void okbutton_Click(object sender, EventArgs e)
		{
			returnvalue = textBox1.Text;
			Close();
		}

		private void cancelbutton_Click(object sender, EventArgs e)
		{
			returnvalue = null;
			Close();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar==13)
			{
				returnvalue = textBox1.Text;
				Close();
			}
		}
	}
}
