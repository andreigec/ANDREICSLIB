using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class selectItemFromListView : Form
    {
        private int mustSelectCount;

        private List<ListViewItem> returnvalues = new List<ListViewItem>();

        /// <summary>
        /// call from static showdialog
        /// </summary>
        private selectItemFromListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// return selected values after dialog closes. if canceled, will return null
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="title"></param>
        /// <param name="listBoxItems"></param>
        /// <param name="multiselect"></param>
        /// <param name="mustSelectCountIN"></param>
        /// <returns></returns>
        public static List<ListViewItem> ShowDialog(string labelText, string title, List<ListViewItem> listBoxItems,
                                              bool multiselect, int mustSelectCountIN = -1)
        {
            var s = new selectItemFromListView();
            s.mustSelectCount = mustSelectCountIN;
            s.listView.MultiSelect = multiselect;

            s.Text = title;
            s.label2.Text = labelText;

            s.listView.Items.Clear();
            int a = 0;
            foreach (var lvi in listBoxItems)
            {
                s.listView.Items.Add(lvi);
                a++;
            }

            s.ShowDialog();
            return s.returnvalues;
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount.ToString() + " items");
                return;
            }
            foreach (ListViewItem v in listView.SelectedItems)
            {
                returnvalues.Add(v);
            }
            Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            returnvalues = null;
            Close();
        }

        private void selectItemFromListBox_Load(object sender, EventArgs e)
        {
            label2.MaximumSize = new Size(Width - 10, 0);
        }

        private void selectItemFromListBox_SizeChanged(object sender, EventArgs e)
        {
            label2.MaximumSize = new Size(Width - 30, 0);
        }
    }
}