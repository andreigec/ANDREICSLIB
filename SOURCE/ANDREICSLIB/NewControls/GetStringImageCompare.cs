using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class GetStringImageCompare : Form
    {
        public GetStringImageCompare()
        {
            InitializeComponent();
        }

        public string[] returnvalue;

        public string[] ShowDialog(String labelText, String title, Image i, int maxlen = -1, ImageLayout il = ImageLayout.Center)
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

        private void GetStringImageCompare_Load(object sender, EventArgs e)
        {
            textbox.Focus();
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            OkClick();
        }

        private void OkClick()
        {
            returnvalue = textbox.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                OkClick();
        }
    }
}
