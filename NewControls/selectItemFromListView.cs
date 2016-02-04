using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/MTG-Proxy-Maker
    /// </summary>
    public partial class SelectItemFromListView : Form
    {
        private int mustSelectCount;
        private List<ListViewItem> returnvalues = new List<ListViewItem>();

        /// <summary>
        /// call from static showdialog
        /// </summary>
        private SelectItemFromListView()
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
        public static List<ListViewItem> ShowDialog(string labelText, string title, List<ListViewItem> listBoxItems,
            bool multiselect, int mustSelectCountIN = -1)
        {
            var s = new SelectItemFromListView();
            s.mustSelectCount = mustSelectCountIN;
            s.listView.MultiSelect = multiselect;

            s.Text = title;
            s.label2.Text = labelText;

            s.listView.Items.Clear();
            var a = 0;
            foreach (var lvi in listBoxItems)
            {
                s.listView.Items.Add(lvi);
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
            if (listView.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount + " items");
                return;
            }
            foreach (ListViewItem v in listView.SelectedItems)
            {
                returnvalues.Add(v);
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
    }
}