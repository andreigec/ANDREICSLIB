using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Meal-Chooser
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class SelectItemFromListBox : Form
    {
        private int mustSelectCount;
        private List<string> returnvalues = new List<string>();

        /// <summary>
        /// call from static showdialog
        /// </summary>
        private SelectItemFromListBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// return selected values after dialog closes. if canceled, will return null
        /// </summary>
        /// <param name="labelText">The label text.</param>
        /// <param name="title">The title.</param>
        /// <param name="listBoxItems">The list box items.</param>
        /// <param name="multiselect">if set to <c>true</c> [multiselect].</param>
        /// <param name="mustSelectCountIN">The must select count in.</param>
        /// <returns></returns>
        public static List<string> ShowDialog(string labelText, string title, List<SelectItem> listBoxItems,
            bool multiselect, int mustSelectCountIN = -1)
        {
            var s = new SelectItemFromListBox();
            s.mustSelectCount = mustSelectCountIN;
            if (multiselect)
                s.listBox1.SelectionMode = SelectionMode.MultiExtended;

            s.Text = title;
            s.label2.Text = labelText;

            s.listBox1.Items.Clear();
            var a = 0;
            foreach (var v in listBoxItems)
            {
                s.listBox1.Items.Add(v.Text);
                s.listBox1.SetSelected(a, v.preselected);
                a++;
            }

            s.ShowDialog();
            return s.returnvalues;
        }

        /// <summary>
        /// Handles the Click event of the okbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okbutton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount + " items");
                return;
            }
            foreach (var v in listBox1.SelectedItems)
            {
                returnvalues.Add(v.ToString());
            }
            Close();
        }

        /// <summary>
        /// Handles the Click event of the cancelbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            returnvalues = null;
            Close();
        }

        /// <summary>
        /// Handles the Load event of the selectItemFromListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void selectItemFromListBox_Load(object sender, EventArgs e)
        {
            label2.MaximumSize = new Size(Width - 10, 0);
        }

        /// <summary>
        /// Handles the SizeChanged event of the selectItemFromListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void selectItemFromListBox_SizeChanged(object sender, EventArgs e)
        {
            label2.MaximumSize = new Size(Width - 30, 0);
        }

        /// <summary>
        /// not a control
        /// </summary>
        public class SelectItem
        {
            public bool preselected;
            public string Text = "";

            /// <summary>
            /// Initializes a new instance of the <see cref="SelectItem"/> class.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
            public SelectItem(string text, bool isSelected)
            {
                Text = text;
                preselected = isSelected;
            }
        }
    }
}