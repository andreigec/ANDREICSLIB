using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class TabPageExtras
    {
        public static void SetEnableOnAllTabPageControls(TabPage tp, bool enabled)
        {
            foreach (Control c in tp.Controls)
            {
                c.Enabled = enabled;
            }
        }

        public static void SetEnableOnAllTabPagesInTabControl(TabControl tc, bool enabled,
                                                              List<TabPage> excluding = null)
        {
            foreach (TabPage tp in tc.TabPages)
            {
                if (excluding != null && excluding.Contains(tp))
                    continue;

                SetEnableOnAllTabPageControls(tp, enabled);
            }
        }
    }
}