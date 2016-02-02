using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Music-File-Info-Editor
    /// </summary>
    public partial class MultipleMessageBox : Form
    {
        private const int Gap = 20;
        public string Result = "";
        public bool Set;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleMessageBox"/> class.
        /// </summary>
        /// <param name="windowText">The window text.</param>
        /// <param name="labelText">The label text.</param>
        /// <param name="Buttons">The buttons.</param>
        public MultipleMessageBox(string windowText, string labelText, IEnumerable<string> Buttons)
        {
            labelText = labelText.Replace("\r\n", "\n");
            labelText = labelText.Replace("\n", Environment.NewLine);
            Set = false;
            Result = "";
            InitializeComponent();

            Text = windowText;
            textarea.Text = labelText;
            var count = 0;
            foreach (var s in Buttons)
            {
                var b = new Button {Text = s};
                b.Click += buttonpress;
                buttonpanel.AddControl(b, true);
                var last = buttonpanel.controlStack[buttonpanel.controlStack.Count - 1];
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

        /// <summary>
        /// Buttonpresses the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonpress(object sender, EventArgs e)
        {
            Set = true;
            Result = ((Button) sender).Text;
            Close();
        }
    }
}