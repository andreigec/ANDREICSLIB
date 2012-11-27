using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public class ListViewSorter : IComparer
    {
        public int ByColumn { get; set; }

        public int LastSort { get; set; }

        #region IComparer Members

        public Dictionary<IPAddress, long> cachedips = new Dictionary<IPAddress, long>();

        /// <summary>
        /// call this from column click. 
        /// </summary>
        /// <param name="LVS">an instance of listviewsorter</param>
        /// <param name="LV"></param>
        /// <param name="column"></param>
        /// <param name="forceorder">if set to a value, will sort by that all the time, otherwise will sort as normal</param>
        public static void ColumnSort(ListViewSorter LVS, ListView LV, int column, SortOrder? forceorder = null)
        {
            try
            {
                LV.ListViewItemSorter = LVS;
                if (!(LV.ListViewItemSorter is ListViewSorter))
                    return;
                LVS = (ListViewSorter)LV.ListViewItemSorter;
            }
            catch (Exception ex)
            {
                return;
            }

            if (forceorder != null)
            {
                LV.Sorting = (SortOrder)forceorder;
            }
            else
            {

                if (LV.Sorting == SortOrder.Ascending)
                    LV.Sorting = SortOrder.Descending;
                else
                    LV.Sorting = SortOrder.Ascending;
            }
            LVS.ByColumn = column;

            LV.Sort();
        }

        public int Compare(object o1, object o2)
        {
            if (o1 == null || !(o1 is ListViewItem))
                return (0);
            if (o2 == null || !(o2 is ListViewItem))
                return (0);

            var lvi1 = (ListViewItem)o2;
            var str1 = lvi1.SubItems[ByColumn].Text;
            var lvi2 = (ListViewItem)o1;
            var str2 = lvi2.SubItems[ByColumn].Text;
            int result = 0;

            IPAddress ip1 = null;
            IPAddress ip2 = null;

            //starts with a letter
            if (StringUpdates.StringStartsWithLetter(str1) && StringUpdates.StringStartsWithLetter(str2))
            {
                if (lvi1.ListView.Sorting == SortOrder.Descending)
                    result = String.Compare(str1, str2);
                else
                    result = String.Compare(str2, str1);
            }
            //test to see if the string is a number - perform an int compare instead of a string compare
            else if (StringUpdates.StringIsNumber(str1) && StringUpdates.StringIsNumber(str2))
            {
                var s1 = double.Parse(str1);
                var s2 = double.Parse(str2);
                if (lvi1.ListView.Sorting == SortOrder.Descending)
                    result = s1.CompareTo(s2);
                else
                    result = s2.CompareTo(s1);
            }
            //ip address
            else if (IPAddress.TryParse(str1, out ip1) && IPAddress.TryParse(str2, out ip2))
            {
                long s1 = GetCachedIP(ip1);
                long s2 = GetCachedIP(ip2);
                if (lvi1.ListView.Sorting == SortOrder.Descending)
                    result = s1.CompareTo(s2);
                else
                    result = s2.CompareTo(s1);
            }
            //by default, string again
            else
            {
                if (lvi1.ListView.Sorting == SortOrder.Descending)
                    result = String.Compare(str1, str2);
                else
                    result = String.Compare(str2, str1);
            }

            LastSort = ByColumn;
            return (result);
        }

        private long GetCachedIP(IPAddress ip)
        {
            if (cachedips.ContainsKey(ip))
                return cachedips[ip];

            var val = Net.GetAddressAsNumber(ip);
            cachedips.Add(ip, val);
            return val;
        }

        #endregion
    }
}
