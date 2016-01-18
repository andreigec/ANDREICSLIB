using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class multipleMessageBox : Form
    {
        private const int Gap = 20;
        public string Result = "";
        public bool Set;

        public multipleMessageBox(string windowText, string labelText, IEnumerable<string> Buttons)
        {
            labelText = labelText.Replace("\r\n", "\n");
            labelText = labelText.Replace("\n", Environment.NewLine);
            Set = false;
            Result = "";
            InitializeComponent();

            Text = windowText;
            textarea.Text = labelText;
            int count = 0;
            foreach (string s in Buttons)
            {
                var b = new Button {Text = s};
                b.Click += buttonpress;
                buttonpanel.AddControl(b, true);
                Control last = buttonpanel.controlStack[buttonpanel.controlStack.Count - 1];
                count++;
                if (count > 3)
                {
                    Size = new Size(last.Location.X + last.Width + Gap, Size.Height);
                }
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void buttonpress(object sender, EventArgs e)
        {
            Set = true;
            Result = ((Button) sender).Text;
            Close();
        }
    }
}