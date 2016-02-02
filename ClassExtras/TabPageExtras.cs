using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class TabPageExtras
    {
        /// <summary>
        /// Sets the enable on all tab page controls.
        /// </summary>
        /// <param name="tp">The tp.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public static void SetEnableOnAllTabPageControls(TabPage tp, bool enabled)
        {
            foreach (Control c in tp.Controls)
            {
                c.Enabled = enabled;
            }
        }

        /// <summary>
        /// Sets the enable on all tab pages in tab control.
        /// </summary>
        /// <param name="tc">The tc.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="excluding">The excluding.</param>
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