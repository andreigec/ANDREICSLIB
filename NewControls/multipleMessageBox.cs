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
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsSet { get; private set; }

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
            IsSet = false;
            Result = "";
            InitializeComponent();

            Text = windowText;
            textarea.Text = labelText;
            var count = 0;
            foreach (var s in Buttons)
            {
                var b = new Button { Text = s };
                b.Click += buttonpress;
                buttonpanel.AddControl(b, true);
                var cs = buttonpanel.GetControlStack();
                var last = cs[cs.Count - 1];
                count++;
                if (count > 3)
                {
                    Size = new Size(last.Location.X + last.Width + Gap, Size.Height);
                }
            }
        }

        /// <summary>
        /// </summary>
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
            IsSet = true;
            Result = ((Button)sender).Text;
            Close();
        }
    }
}