using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ANDREICSLIB
{
	public abstract class ObjectUpdates : Object
	{
		/// <summary>
		/// Add a tooltip to a control
		/// </summary>
		/// <param name="C">the control to add the tooltip to</param>
		/// <param name="text">the tooltip text</param>
		public static void AddToolTip(Control C, String text)
		{
			if (C == null)
				return;

			var TT = new ToolTip();
			TT.SetToolTip(C, text);
		}

        public static object CloneObject(object o)
        {
            var t = o.GetType();
            var properties = t.GetProperties();

            var p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);

            if (o is ComboBox)
            {
                foreach (String s in ((ComboBox)o).Items)
                {
                    ((ComboBox)p).Items.Add(s);
                }
            }

            else if (o is ListBox)
            {
                foreach (String s in ((ListBox)o).Items)
                {
                    ((ListBox)p).Items.Add(s);
                }
            }

            else if (o is ListView)
            {
                foreach (ListViewItem s in ((ListView)o).Items)
                {
                    ((ListView)p).Items.Add(s);
                }
            }

            //if we copy the parent property, it will add the newly cloned object to those lists which we dont want,
            //so temporarily clear the parent
            Control oparent = null;
            if (o is Control)
            {
                oparent = ((Control)o).Parent;
                ((Control)o).Parent = null;
            }

            foreach (var pi in properties)
            {
                try
                {
                    if (pi.CanWrite)
                    {
                        pi.SetValue(p, pi.GetValue(o, null), null);
                    }
                }
                catch (Exception)
                {

                }

            }

            if (o is Control)
            {
                ((Control)o).Parent = oparent;
            }

            return p;
        }
	}
}
