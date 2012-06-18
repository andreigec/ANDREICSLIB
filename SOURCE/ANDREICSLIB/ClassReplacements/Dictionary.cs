using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public class DictionaryUpdates<TKey, TValue> : Dictionary<TKey, TValue>
	{
		/// <summary>
		/// Convert a dictionary of key/value to a list of listviewitems
		/// </summary>
		/// <param name="origDict">Input Dictionary, string as the key for the text/name, and a list of strings in the value, for subitems</param>
		/// <returns>a list of listviewitems made from a dictionary</returns>
		public static List<ListViewItem> DictToListOfListViewItems(Dictionary<String, List<String>> origDict)
		{
			var lvil = new List<ListViewItem>();

			foreach (var kvp in origDict)
			{
				var lvi = new ListViewItem {Text = kvp.Key, Name = kvp.Key};
				foreach (String s in kvp.Value)
					lvi.SubItems.Add(s);
				lvil.Add(lvi);
			}
			return lvil;
		}

		/// <summary>
		/// Convert a dictionary of key/value to a list of listviewitems
		/// </summary>
		/// <param name="origDict">Input Dictionary, string as the key for the text/name, and a string in the value, for a subitem</param>
		/// <returns>a list of listviewitems made from a dictionary</returns>
		public static List<ListViewItem> DictToListOfListViewItems(Dictionary<String, String> origDict)
		{
			var result = new Dictionary<string, List<string>>();

			foreach (var kvp in origDict)
			{
				var newl = new List<string> {kvp.Value};
				result.Add(kvp.Key, newl);
			}
			return DictToListOfListViewItems(result);
		}
	}
}