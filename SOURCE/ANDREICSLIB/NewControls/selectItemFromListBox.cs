using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ANDREICSLIB.NewControls;

namespace ANDREICSLIB
{
	public partial class selectItemFromListBox : Form
	{
        /// <summary>
        /// call from static showdialog
        /// </summary>
        private selectItemFromListBox()
        {
            InitializeComponent();
        }

		private int mustSelectCount=0;

		private List<string> returnvalues = new List<string>();

        /// <summary>
        /// return selected values after dialog closes. if canceled, will return null
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="title"></param>
        /// <param name="listBoxItems"></param>
        /// <param name="multiselect"></param>
        /// <param name="mustSelectCountIN"></param>
        /// <returns></returns>
		public static List<string> ShowDialog(String labelText, String title, List<SelectItem> listBoxItems, bool multiselect,int mustSelectCountIN=-1)
        {
            var s = new selectItemFromListBox();
			s.mustSelectCount = mustSelectCountIN;
			if (multiselect)
                s.listBox1.SelectionMode = SelectionMode.MultiExtended;

            s.Text = title;
            s.label2.Text = labelText;

            s.listBox1.Items.Clear();
			var a = 0;
			foreach (var v in listBoxItems)
			{
                s.listBox1.Items.Add(v.text);
                s.listBox1.SetSelected(a, v.preselected);
				a++;
			}

            s.ShowDialog();
            return s.returnvalues;
		}

		private void okbutton_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItems.Count < mustSelectCount&&mustSelectCount!=-1)
			{
				MessageBox.Show("You must select at least "+mustSelectCount.ToString()+" items");
				return;
			}
			foreach (var v in listBox1.SelectedItems)
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
