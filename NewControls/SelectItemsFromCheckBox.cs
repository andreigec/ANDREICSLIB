using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/BeadSprite-Pro
    /// </summary>
    public partial class SelectItemsFromCheckBox : Form
    {
        private int mustSelectCount;
        private List<string> returnvalues = new List<string>();

        /// <summary>
        /// Prevents a default instance of the <see cref="SelectItemsFromCheckBox"/> class from being created.
        /// </summary>
        private SelectItemsFromCheckBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the SelectItemsFromCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SelectItemsFromCheckBox_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="labelText">The label text.</param>
        /// <param name="title">The title.</param>
        /// <param name="listBoxItems">The list box items.</param>
        /// <param name="mustSelectCountIN">The must select count in.</param>
        /// <returns></returns>
        public static List<string> ShowDialog(string labelText, string title,
            List<SelectItemFromListBox.SelectItem> listBoxItems,
            int mustSelectCountIN = -1)
        {
            var s = new SelectItemsFromCheckBox();
            s.mustSelectCount = mustSelectCountIN;

            s.Text = title;
            s.label1.Text = labelText;

            s.checkedListBox1.Items.Clear();
            var a = 0;
            foreach (var v in listBoxItems)
            {
                s.checkedListBox1.Items.Add(v.Text);
                s.checkedListBox1.SetSelected(a, v.preselected);
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
            if (checkedListBox1.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount + " items");
                return;
            }
            foreach (var v in checkedListBox1.SelectedItems)
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
    }
}