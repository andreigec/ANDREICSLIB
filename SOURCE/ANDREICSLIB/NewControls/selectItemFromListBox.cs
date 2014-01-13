using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class selectItemFromListBox : Form
    {
        private int mustSelectCount;

        private List<string> returnvalues = new List<string>();

        /// <summary>
        /// call from static showdialog
        /// </summary>
        private selectItemFromListBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// not a control
        /// </summary>
        public class SelectItem
        {
            public bool preselected = false;
            public string Text = "";

            public SelectItem(String text, bool isSelected)
            {
                Text = text;
                preselected = isSelected;
            }
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
        public static List<string> ShowDialog(String labelText, String title, List<SelectItem> listBoxItems,
                                              bool multiselect, int mustSelectCountIN = -1)
        {
            var s = new selectItemFromListBox();
            s.mustSelectCount = mustSelectCountIN;
            if (multiselect)
                s.listBox1.SelectionMode = SelectionMode.MultiExtended;

            s.Text = title;
            s.label2.Text = labelText;

            s.listBox1.Items.Clear();
            int a = 0;
            foreach (SelectItem v in listBoxItems)
            {
                s.listBox1.Items.Add(v.Text);
                s.listBox1.SetSelected(a, v.preselected);
                a++;
            }

            s.ShowDialog();
            return s.returnvalues;
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount.ToString() + " items");
                return;
            }
            foreach (object v in listBox1.SelectedItems)
            {
                returnvalues.Add(v.ToString());
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