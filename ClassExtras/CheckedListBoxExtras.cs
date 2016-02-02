using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/BeadSprite-Pro
    /// </summary>
    public static class CheckedListBoxExtras
    {
        /// <summary>
        /// Checks all.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void CheckAll(CheckedListBox cb, bool value)
        {
            for (var a = 0; a < cb.Items.Count; a++)
            {
                cb.SetItemChecked(a, value);
            }
        }

        /// <summary>
        /// Checks the item.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <param name="item">The item.</param>
        public static void CheckItem(CheckedListBox cb, string item)
        {
            for (var a = 0; a < cb.Items.Count; a++)
            {
                if (cb.Items[a].Equals(item))
                {
                    cb.SetItemChecked(a, true);
                }
            }
        }
    }
}