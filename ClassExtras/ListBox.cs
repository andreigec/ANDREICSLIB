using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    public static class ListBoxExtras
    {
        public static void RemoveSelected(ref ListBox lb)
        {
            for (int i = lb.SelectedIndices.Count - 1; i >= 0; i--)
            {
                lb.Items.RemoveAt(lb.SelectedIndices[i]);
            }
        }
    }
}
