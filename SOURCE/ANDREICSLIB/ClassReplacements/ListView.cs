using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

//andrei gec

namespace ANDREICSLIB
{
	public class ListViewUpdate : ListView
	{
		private ListViewItem.ListViewSubItem[] SubItemCollectionToRange(ListViewItem.ListViewSubItemCollection lvsic)
		{
			var result = new ListViewItem.ListViewSubItem[lvsic.Count];

			var count = -1;
			foreach (ListViewItem.ListViewSubItem LVSI in lvsic)
			{
				count++;
				if (count == 0)
					continue;

				result[count] = LVSI;
			}

			return result;
		}

		/// <summary>
		/// swap two rows given by their index
		/// </summary>
		/// <param name="index1">First Index to Swap</param>
		/// <param name="index2">Second Index to Swap</param>
		public void SwapIndicies(int index1, int index2)
		{
			Hide();
			if (index1 < 0 || index2 < 0 || index1 >= Items.Count || index2 >= Items.Count)
				return;

			//make clones
			var LVI1 = (ListViewItem)Items[index1].Clone();
			var LVI2 = (ListViewItem)Items[index2].Clone();

			//swap the sub items
			Items[index1].SubItems.Clear();
			Items[index1].SubItems.AddRange(SubItemCollectionToRange(LVI2.SubItems));

			Items[index2].SubItems.Clear();
			Items[index2].SubItems.AddRange(SubItemCollectionToRange(LVI1.SubItems));

			//swap the name and text
			Items[index1].Text = LVI1.Text;
			Items[index2].Text = LVI2.Text;

			Items[index1].Name = LVI1.Text;
			Items[index2].Name = LVI2.Text;
			Show();
		}

		private void AutoResizeListViewColumn(ColumnHeader ch)
		{
			var headerWidth = ch.Text.Length;

			var changeHeader = true;

			foreach (ListViewItem LVI in Items)
			{
				var temp = ch.Index == 0 ? LVI.Text.Length : LVI.SubItems[ch.Index].Text.Length;

				if (temp > headerWidth)
				{
					changeHeader = false;
					break;
				}
			}

			AutoResizeColumn(ch.Index,
							 changeHeader ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		/// <summary>
		/// resize all columns to best fit the header and the contents
		/// </summary>
		public void AutoResize()
		{
			Hide();
			foreach (ColumnHeader CH in Columns)
			{
				AutoResizeListViewColumn(CH);
			}

			Show();
		}

		/// <summary>
		/// Initialise the columns to be those in a list of strings
		/// </summary>
		/// <param name="columnList">The list of strings to be made columns of</param>
		public void InitColumnHeaders(List<String> columnList)
		{
			if (columnList == null)
				return;

			Columns.Clear();
			foreach (var s in columnList)
			{
				Columns.Add(s, s);
			}
		}

		/// <summary>
		/// Select all the items in the list view
		/// </summary>
		public void SelectAllItems()
		{
			SelectedItems.Clear();
			foreach (ListViewItem LVI in Items)
			{
				LVI.Selected = true;
			}
		}

		/// <summary>
		/// get the index of a column name
		/// </summary>
		/// <param name="columnName">the column name</param>
		/// <returns>the index of the column, -1 if not found</returns>
		public int GetColumnNumber(String columnName)
		{
			var count = -1;
			foreach (ColumnHeader CH in Columns)
			{
				count++;

				if (CH.Name.Equals(columnName))
					return count;
			}
			return count;
		}

		public void CopyClassToListView<T>(T from)
		{
			var t = from.GetType();
			var pi = t.GetProperties();
			var fi = t.GetFields();

			foreach (var prop in pi)
			{
				var o = prop.GetValue(from, null);
				AddItemToListView(prop.Name, o);
			}

			foreach (var field in fi)
			{
				var o = field.GetValue(from);
				AddItemToListView(field.Name, o);
			}
		}

		private void AddItemToListView(String key, object value)
		{
			var key2 = key;
			String value2;
			if (value == null)
				value2 = "";
			else
				value2 = value.ToString();

			Items.Add(key2).SubItems.Add(value2);
		}
	}
}