using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public class FormConfigRestore
    {
        private const char separator = '\f';

        private static void SaveProperty(StreamWriter SW, Control C)
        {
            if (C is CheckBox)
            {
                var V = (CheckBox)C;
                SW.WriteLine(V.Name + separator + "Checked" + separator + V.Checked);
            }

            else if (C is ListBox)
            {
                var lb = C as ListBox;
                var o = lb.Name + separator + "Items" + separator;
                foreach (var i in lb.Items)
                {
                    o += i + "|";
                }
                SW.WriteLine(o);
            }
            else
            {
                SW.WriteLine(C.Name + separator + "Text" + separator + C.Text);
            }
        }

        private static void SaveProperty(StreamWriter SW, ToolStripItem TSI)
        {
            if (TSI is ToolStripMenuItem)
            {
                var V = (ToolStripMenuItem)TSI;
                SW.WriteLine(V.Name + separator + "Checked" + separator + V.Checked);
            }
        }

        /// <summary>
        /// set the value of the control object
        /// </summary>
        /// <param name="C"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private static bool LoadProperty(object C, String propertyName, object value)
        {
            //manually resolve first if possible
            var ok = LoadPropertyManual(C, propertyName, value);
            if (ok)
                return true;

            var t = C.GetType();
            var p = t.GetProperty(propertyName);
            var t2 = p.PropertyType;
            try
            {
                p.SetValue(C, Convert.ChangeType(value, t2), null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool LoadPropertyManual(object C, String propertyName, object value)
        {
            if (value == null)
                return false;

            if (C is ListBox && propertyName.Equals("Items"))
            {
                var lb = C as ListBox;
                var s = new[] { '|' };
                var v = value.ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);
                foreach (var v2 in v)
                {
                    lb.Items.Add(v2);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// find the matching control for the name given
        /// </summary>
        /// <param name="baseform"></param>
        /// <param name="name"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private static void LoadProperty(Form baseform, String name, String propertyName, object value = null)
        {
            foreach (Control C in baseform.Controls)
            {
                if (C.Name.Equals(name))
                {
                    LoadProperty(C, propertyName, value);
                    return;
                }

                if (C is MenuStrip)
                {
                    var MS = (MenuStrip)C;
                    foreach (ToolStripItem TSI in MS.Items)
                    {
                        if (LoadProperty(name, propertyName, TSI, value))
                            return;
                    }
                }
                else
                {
                    if (C.Controls.Count > 0)
                    {
                        if (LoadProperty(name, propertyName, C, value))
                            return;
                    }
                }
            }
        }

        private static bool LoadProperty(String name, String propertyName, Control checkthis, object value)
        {
            foreach (Control C in checkthis.Controls)
            {
                if (C.Name.Equals(name))
                {
                    LoadProperty(C, propertyName, value);
                    return true;
                }

                if (C is MenuStrip)
                {
                    var MS = (MenuStrip)C;
                    foreach (ToolStripItem TSI in MS.Items)
                    {
                        if (LoadProperty(name, propertyName, TSI, value))
                            return true;
                    }
                }
                else
                {
                    if (C.Controls.Count > 0)
                    {
                        if (LoadProperty(name, propertyName, C, value))
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool LoadProperty(String name, String propertyName, ToolStripItem checkthis, object value)
        {
            if (checkthis is ToolStripMenuItem)
            {
                var TSMI = (ToolStripMenuItem)checkthis;

                foreach (ToolStripItem TSI in TSMI.DropDownItems)
                {
                    if (LoadProperty(name, propertyName, TSI, value))
                        return true;
                }
            }

            if (checkthis.Name.Equals(name))
            {
                LoadProperty(checkthis, propertyName, value);
                return true;
            }

            return false;
        }

        public static bool LoadConfig(Form baseform, String filename)
        {
            FileStream FS = null;
            StreamReader SR = null;
            var f = new char[] { separator };
            try
            {
                if (File.Exists(filename) == false)
                    return false;

                FS = new FileStream(filename, FileMode.Open);
                SR = new StreamReader(FS);

                String s = null;
                while ((s = SR.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(s))
                        continue;
                    var split = s.Split(f, StringSplitOptions.RemoveEmptyEntries);
                    String v = null;
                    if (split.Length >= 3)
                        v = split[2];
                    LoadProperty(baseform, split[0], split[1], v);
                }

                SR.Close();
                FS.Close();
            }
            catch (Exception ex)
            {
                if (SR != null)
                    SR.Close();
                if (FS != null)
                    FS.Close();
                //on error delete config
                if (File.Exists(filename))
                    File.Delete(filename);
                return false;
            }
            return true;
        }

        public static bool SaveConfig(Form baseform, String filename, List<Control> saveControls,
                                      List<ToolStripItem> saveToolStripItems = null)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);

                var FS = new FileStream(filename, FileMode.CreateNew);
                var SW = new StreamWriter(FS);
                foreach (var C in saveControls)
                {
                    SaveProperty(SW, C);
                }

                if (saveToolStripItems != null)
                {
                    foreach (var TSI in saveToolStripItems)
                    {
                        SaveProperty(SW, TSI);
                    }
                }

                SW.Close();
                FS.Close();
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }
    }

}