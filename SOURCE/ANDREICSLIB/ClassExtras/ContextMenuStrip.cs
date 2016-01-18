using System;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Word-Find-Solver
    /// </summary>
    public static class ContextMenuStripExtras
    {
        /// <summary>
        /// get the parent of a context menu, either opening up, or a tool strip right click
        /// </summary>
        /// <param name="senderToolStrip"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetContextParent(object senderToolStrip, Type t)
        {
            ContextMenuStrip lb2 = null;
            if (senderToolStrip is ToolStripDropDownItem)
            {
                var lb1 = senderToolStrip as ToolStripDropDownItem;
                if (lb1 == null)
                    return null;

                lb2 = lb1.Owner as ContextMenuStrip;
                if (lb2 == null)
                    return null;
            }
            else if (senderToolStrip is ContextMenuStrip)
            {
                lb2 = senderToolStrip as ContextMenuStrip;
                if (lb2 == null)
                    return null;
            }


            object ret = null;
            try
            {
                ret = Convert.ChangeType(lb2.SourceControl, t);
                return ret;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}