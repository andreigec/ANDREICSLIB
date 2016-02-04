using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Word-Find-Solver
    /// </summary>
    public static class ContextMenuStripExtras
    {
        /// <summary>
        /// get the parent of a context menu, either opening up, or a tool strip right click
        /// </summary>
        /// <param name="senderToolStrip">The sender tool strip.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static object GetContextParent(IComponent senderToolStrip, Type t)
        {
            ContextMenuStrip lb2 = null;
            if (senderToolStrip is ToolStripDropDownItem)
            {
                var lb1 = (ToolStripDropDownItem) senderToolStrip;

                lb2 = lb1.Owner as ContextMenuStrip;
                if (lb2 == null)
                    return null;
            }
            else if (senderToolStrip is ContextMenuStrip)
            {
                lb2 = (ContextMenuStrip) senderToolStrip;
            }

            if (lb2 == null)
                return null;

            try
            {
                var ret = Convert.ChangeType(lb2.SourceControl, t);
                return ret;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}