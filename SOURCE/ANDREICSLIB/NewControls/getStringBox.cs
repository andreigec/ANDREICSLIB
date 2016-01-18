using System;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class getStringBox : Form
    {
        public string returnvalue = "";

        public getStringBox()
        {
            InitializeComponent();
        }

        public string ShowDialog(string labelText, string title,bool multiline=false)
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
            if (e.KeyChar == 13&&textBox1.Multiline==false)
            {
                returnvalue = textBox1.Text;
                Close();
            }
        }
    }
}