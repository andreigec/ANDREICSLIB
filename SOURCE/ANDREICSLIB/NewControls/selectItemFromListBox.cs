using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public partial class selectItemFromListBox : Form
	{

		public class listboxitem:IComparable
		{
			public string text="";
			public bool preselected=false;
			int IComparable.CompareTo(object obj)
			{
				return String.Compare(text,((listboxitem)obj).text);
			}
		}

		private int mustSelectCount=0;

		public selectItemFromListBox()
		{
			InitializeComponent();
		}

		private List<string> returnvalues = new List<string>();

		public List<string> ShowDialog(String labelText, String title, List<listboxitem> listBoxItems, bool multiselect,int mustSelectCountIN)
		{
			mustSelectCount = mustSelectCountIN;
			if (multiselect)
				listBox1.SelectionMode= SelectionMode.MultiExtended;

			Text = title;
			label2.Text = labelText;

			listBox1.Items.Clear();
			var a = 0;
			foreach (var v in listBoxItems)
			{
				listBox1.Items.Add(v.text);
					listBox1.SetSelected(a,v.preselected);
				a++;
			}

			ShowDialog();
			return returnvalues;
		}

		private void okbutton_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItems.Count < mustSelectCount)
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
