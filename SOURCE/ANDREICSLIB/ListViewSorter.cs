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
        public bool Enabled = true;

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
            if (Enabled == false)
                return 0;

            if (o1 == null || !(o1 is ListViewItem))
                return (0);
            if (o2 == null || !(o2 is ListViewItem))
                return (0);

            var lvi1 = (ListViewItem)o2;
            var str1 = lvi1.SubItems[ByColumn].Text;
            var lvi2 = (ListViewItem)o1;
            var str2 = lvi2.SubItems[ByColumn].Text;

            var r = CompareNatural(str1, str2);

            if (lvi1.ListView.Sorting == SortOrder.Descending)
                return r;
            else
            {
                return r== -1 ? 1 : -1;
            }
        }

        private static int CompareNatural(String x,String y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int lx = x.Length, ly = y.Length;

            for (int mx = 0, my = 0; mx < lx && my < ly; mx++, my++)
            {
                if (char.IsDigit(x[mx]) && char.IsDigit(y[my]))
                {
                    long vx = 0, vy = 0;

                    for (; mx < lx && char.IsDigit(x[mx]); mx++)
                        vx = vx * 10 + x[mx] - '0';

                    for (; my < ly && char.IsDigit(y[my]); my++)
                        vy = vy * 10 + y[my] - '0';

                    if (vx != vy)
                        return vx > vy ? 1 : -1;
                }

                if (mx < lx && my < ly && x[mx] != y[my])
                    return x[mx] > y[my] ? 1 : -1;
            }

            return lx - ly; 
        }

        #endregion
    }
}
