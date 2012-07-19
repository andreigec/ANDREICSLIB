using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public class FormConfigRestore
    {
        private const string separator = "\f";
        private const string typesep = "\b";
        private const string newline = "\r\n";

        private static void SaveProperty(ref string output, Control C)
        {
            if (C is CheckBox)
            {
                var V = (CheckBox)C;
                output += V.Name + separator + "Checked" + separator + V.Checked + newline;
            }

            else if (C is ListBox)
            {
                var lb = C as ListBox;
                var o = lb.Name + separator + "Items" + separator;
                foreach (var i in lb.Items)
                {
                    o += i + "|";
                }
                output += o + newline;
            }
            else
            {
                output += C.Name + separator + "Text" + separator + C.Text + newline;
            }
        }

        private static void SaveProperty(ref string output, ToolStripItem TSI)
        {
            if (TSI is ToolStripMenuItem)
            {
                var V = (ToolStripMenuItem)TSI;
                output += V.Name + separator + "Checked" + separator + V.Checked + newline;
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

        /// <summary>
        /// load a saved config file
        /// </summary>
        /// <param name="baseform">pass the base form</param>
        /// <param name="filename">the saved config flename</param>
        /// <returns>returns null on error, and a list of tuples of saved literal strings otherwise</returns>
        public static List<Tuple<String, String>> LoadConfig(Form baseform, String filename)
        {
            var ret = new List<Tuple<String, String>>();
            try
            {
                if (File.Exists(filename) == false)
                    return null;

                var f = FileUpdates.LoadFile(filename);

                var parts = StringUpdates.SplitString(f, typesep);

                //first part is controls and stuff
                var controls = StringUpdates.SplitString(parts[0], newline);

                foreach (var line in controls)
                {
                    var split = StringUpdates.SplitString(line, separator);
                    if (split.Length < 2)
                        continue;

                    String v = null;
                    if (split.Length >= 3)
                        v = split[2];

                    LoadProperty(baseform, split[0], split[1], v);
                }

                //second part is literal strings
                if (parts.Length >= 2)
                {
                    var strings = StringUpdates.SplitString(parts[1], newline);
                    foreach (var line in strings)
                    {
                        var split = StringUpdates.SplitString(line, separator);
                        if (split.Length < 2)
                            continue;

                        ret.Add(new Tuple<string, string>(split[0], split[1]));
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                //on error, delete the config
                try
                {
                    File.Delete(filename);
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        public static bool SaveConfig(Form baseform, String filename, List<Control> saveControls = null, List<ToolStripItem> saveToolStripItems = null, List<Tuple<String, String>> LiteralStrings = null)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);

                String output = "";
                if (saveControls != null)
                {
                    foreach (var C in saveControls)
                    {
                        SaveProperty(ref output, C);
                    }
                }

                if (saveToolStripItems != null)
                {
                    foreach (var TSI in saveToolStripItems)
                    {
                        SaveProperty(ref output, TSI);
                    }
                }

                output += typesep;

                if (LiteralStrings != null)
                {
                    foreach (var s in LiteralStrings)
                    {
                        output += s.Item1 + separator + s.Item2 + newline;
                    }
                }

                FileUpdates.SaveToFile(filename, output);
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }
    }

}