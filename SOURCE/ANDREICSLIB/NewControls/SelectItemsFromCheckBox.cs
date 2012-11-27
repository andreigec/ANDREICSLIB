using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class SelectItemsFromCheckBox : Form
    {
        private int mustSelectCount = 0;
        private List<string> returnvalues = new List<string>();

        private SelectItemsFromCheckBox()
        {
            InitializeComponent();
        }

        private void SelectItemsFromCheckBox_Load(object sender, EventArgs e)
        {

        }

        public static List<String> ShowDialog(String labelText, String title, List<SelectItem> listBoxItems, int mustSelectCountIN=-1)
        {
            var s = new SelectItemsFromCheckBox();
            s.mustSelectCount = mustSelectCountIN;

            s.Text = title;
            s.label1.Text = labelText;
            
            s.checkedListBox1.Items.Clear();
            var a = 0;
            foreach (var v in listBoxItems)
            {
                s.checkedListBox1.Items.Add(v.text);
                s.checkedListBox1.SetSelected(a, v.preselected);
                a++;
            }

            s.ShowDialog();
            return s.returnvalues;
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItems.Count < mustSelectCount&&mustSelectCount!=-1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount.ToString() + " items");
                return;
            }
            foreach (var v in checkedListBox1.SelectedItems)
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
    }
}
