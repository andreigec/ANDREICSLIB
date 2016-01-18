using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB
{
    public class FormConfigRestore
    {
        private const string Separator = "\f";
        private const string ListSeparator = "\a";
        private const string TypeSep = "\b";
        private const string NewLine = "\r\n";

        private static void SaveProperty(ref string output, Control C)
        {
            if (C is CheckBox)
            {
                var V = (CheckBox)C;
                output += V.Name + Separator + "Checked" + Separator + V.Checked + NewLine;
            }
            else if (C is ComboBox)
            {
                var V = (ComboBox)C;
                output += V.Name + Separator + "Text" + Separator + V.Text + NewLine;
            }

            else if (C is ListBox)
            {
                var lb = C as ListBox;
                string o = lb.Name + Separator + "Items" + Separator;
                foreach (object i in lb.Items)
                {
                    o += i + ListSeparator;
                }
                output += o + NewLine;
            }
            else if (C is ListView)
            {
                var lv = C as ListView;
                string o = lv.Name + Separator + "Items" + Separator;
                foreach (ListViewItem i in lv.Items)
                {
                    string name = i.Name;
                    if (string.IsNullOrWhiteSpace(name))
                        name = i.Text;

                    o += name + ListSeparator + i.Text + ListSeparator;
                }
                output += o + NewLine;
            }
            else
            {
                output += C.Name + Separator + "Text" + Separator + C.Text + NewLine;
            }
        }

        private static void SaveProperty(ref string output, ToolStripItem tsi)
        {
            if (tsi is ToolStripMenuItem)
            {
                var v = (ToolStripMenuItem)tsi;
                output += v.Name + Separator + "Checked" + Separator + v.Checked + NewLine;
            }
        }

        /// <summary>
        /// set the value of the control object
        /// </summary>
        /// <param name="c"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private static bool LoadProperty(object c, String propertyName, object value)
        {
            //manually resolve first if possible
            bool ok = LoadPropertyManual(c, propertyName, value);
            if (ok)
                return true;

            Type t = c.GetType();
            PropertyInfo p = t.GetProperty(propertyName);
            Type t2 = p.PropertyType;
            try
            {
                p.SetValue(c, Convert.ChangeType(value, t2), null);
                return true;
            }
            catch
            {
            }
            return false;
        }

        private static bool LoadPropertyManual(object c, String propertyName, object value)
        {
            if (value == null)
                return false;

            if (c is ListBox && propertyName.Equals("Items"))
            {
                var lb = c as ListBox;
                var s = new[] { ListSeparator };
                string[] v = value.ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);
                foreach (string v2 in v)
                {
                    lb.Items.Add(v2);
                }
                return true;
            }
            if (c is ListView && propertyName.Equals("Items"))
            {
                var lv = c as ListView;
                var s = new[] { ListSeparator };
                string[] v = value.ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);
                if (v.Length % 2 == 0)
                {
                    for (int a = 0; a < v.Length; a += 2)
                    {
                        lv.Items.Add(v[a + 1]).Name = v[a];
                    }
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
            foreach (Control c in baseform.Controls)
            {
                if (c.Name.Equals(name))
                {
                    LoadProperty(c, propertyName, value);
                    return;
                }

                if (c is MenuStrip)
                {
                    var ms = (MenuStrip)c;
                    foreach (ToolStripItem tsi in ms.Items)
                    {
                        if (LoadProperty(name, propertyName, tsi, value))
                            return;
                    }
                }
                else
                {
                    if (c.Controls.Count > 0)
                    {
                        if (LoadProperty(name, propertyName, c, value))
                            return;
                    }
                }
            }
        }

        private static bool LoadProperty(String name, String propertyName, Control checkthis, object value)
        {
            foreach (Control c in checkthis.Controls)
            {
                if (c.Name.Equals(name))
                {
                    LoadProperty(c, propertyName, value);
                    return true;
                }

                if (c is MenuStrip)
                {
                    var ms = (MenuStrip)c;
                    foreach (ToolStripItem TSI in ms.Items)
                    {
                        if (LoadProperty(name, propertyName, TSI, value))
                            return true;
                    }
                }
                else
                {
                    if (c.Controls.Count > 0)
                    {
                        if (LoadProperty(name, propertyName, c, value))
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
                var tsmi = (ToolStripMenuItem)checkthis;

                foreach (ToolStripItem tsi in tsmi.DropDownItems)
                {
                    if (LoadProperty(name, propertyName, tsi, value))
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
        /// load the saved config file. will automatically load all the control values, and return the manual strings
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

                string f = FileExtras.LoadFile(filename);

                string[] parts = StringExtras.SplitString(f, TypeSep);

                //first part is controls and stuff
                string[] controls = StringExtras.SplitString(parts[0], NewLine);

                foreach (string line in controls)
                {
                    string[] split = StringExtras.SplitString(line, Separator);
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
                    string[] strings = StringExtras.SplitString(parts[1], NewLine);
                    foreach (string line in strings)
                    {
                        string[] split = StringExtras.SplitString(line, Separator);
                        if (split.Length < 2)
                            continue;

                        ret.Add(new Tuple<string, string>(split[0], split[1]));
                    }
                }

                return ret;
            }
            catch (Exception)
            {
                //on error, delete the config
                try
                {
                    File.Delete(filename);
                }
                catch
                {
                }
                return null;
            }
        }

        /// <summary>
        /// save controls, tool strips, and manually saved strings
        /// call this on form load
        /// </summary>
        /// <param name="baseform"></param>
        /// <param name="filename"></param>
        /// <param name="saveControls">a list of form controlls to save, except tool strip items</param>
        /// <param name="saveToolStripItems">list of tool strip menu items which the checked value should be saved for</param>
        /// <param name="literalStrings">a list of tuple string/strings to manually save</param>
        /// <returns></returns>
        public static bool SaveConfig(Form baseform, String filename, IEnumerable<Control> saveControls = null,
                                      IEnumerable<ToolStripItem> saveToolStripItems = null,
                                      IEnumerable<Tuple<string, string>> literalStrings = null)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);

                String output = "";
                if (saveControls != null)
                {
                    foreach (Control c in saveControls)
                    {
                        SaveProperty(ref output, c);
                    }
                }

                if (saveToolStripItems != null)
                {
                    foreach (ToolStripItem tsi in saveToolStripItems)
                    {
                        SaveProperty(ref output, tsi);
                    }
                }

                output += TypeSep;

                if (literalStrings != null)
                {
                    foreach (var s in literalStrings)
                    {
                        output += s.Item1 + Separator + s.Item2 + NewLine;
                    }
                }

                FileExtras.SaveToFile(filename, output);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}