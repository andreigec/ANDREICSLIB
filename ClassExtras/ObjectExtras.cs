using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Image-Scripter
    /// </summary>
    public abstract class ObjectExtras
    {
        /// <summary>
        /// Add a tooltip to a control
        /// </summary>
        /// <param name="C">the control to add the tooltip to</param>
        /// <param name="text">the tooltip text</param>
        public static void AddToolTip(Control C, string text)
        {
            if (C == null)
                return;

            var TT = new ToolTip();
            TT.SetToolTip(C, text);
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>
        /// The copied object.
        /// </returns>
        /// <exception cref="ArgumentException">The type must be serializable.;source</exception>
        public static T Clone<T>(T source)
        {
            if (!typeof (T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static object CloneObject(object o)
        {
            var t = o.GetType();
            var properties = t.GetProperties();

            var p = t.InvokeMember("", BindingFlags.CreateInstance, null, o, null);

            if (o is ComboBox)
            {
                foreach (string s in ((ComboBox) o).Items)
                {
                    ((ComboBox) p).Items.Add(s);
                }
            }

            else if (o is ListBox)
            {
                foreach (string s in ((ListBox) o).Items)
                {
                    ((ListBox) p).Items.Add(s);
                }
            }

            else if (o is ListView)
            {
                foreach (ListViewItem s in ((ListView) o).Items)
                {
                    ((ListView) p).Items.Add(s);
                }
            }

            //if we copy the parent property, it will add the newly cloned object to those lists which we dont want,
            //so temporarily clear the parent
            Control oparent = null;
            if (o is Control)
            {
                oparent = ((Control) o).Parent;
                ((Control) o).Parent = null;
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
                ((Control) o).Parent = oparent;
            }

            return p;
        }

        /// <summary>
        /// Gets the name of the field by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static T GetFieldByName<T>(object instance, string fieldName)
        {
            try
            {
                var myType = instance.GetType();
                // Get the FieldInfo of MyClass.
                var myFields = myType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var ret = default(T);
                foreach (var t in myFields)
                {
                    if (t.Name == fieldName)
                    {
                        ret = (T) t.GetValue(instance);
                        break;
                    }
                }
                return ret;
            }
            catch
            {
                return default(T);
            }
        }
    }
}