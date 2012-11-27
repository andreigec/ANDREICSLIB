using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    public static class CheckedListBoxUpdates
    {
        public static void CheckAll(CheckedListBox cb,bool value)
        {
            for (int a = 0; a < cb.Items.Count; a++)
            {
                cb.SetItemChecked(a, value);
            }
        }

        public static void CheckItem(CheckedListBox cb,String item)
        {
            for (int a=0;a<cb.Items.Count;a++)
            {
                if (cb.Items[a].Equals(item))
                {
                    cb.SetItemChecked(a,true);
                }
            }
        }

    }
}
