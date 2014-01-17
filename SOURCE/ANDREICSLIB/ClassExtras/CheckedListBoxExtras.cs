using System;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    public static class CheckedListBoxExtras
    {
        /// <summary>
        /// check all items 
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="value"></param>
        public static void CheckAll(CheckedListBox cb, bool value)
        {
            for (int a = 0; a < cb.Items.Count; a++)
            {
                cb.SetItemChecked(a, value);
            }
        }

        /// <summary>
        /// check a certain item
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="item"></param>
        public static void CheckItem(CheckedListBox cb, String item)
        {
            for (int a = 0; a < cb.Items.Count; a++)
            {
                if (cb.Items[a].Equals(item))
                {
                    cb.SetItemChecked(a, true);
                }
            }
        }
    }
}