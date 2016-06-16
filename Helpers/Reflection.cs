using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Backgrounder
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// convert a string into a enum row for copy pasting into c#
        /// eg test one two three = 
        /// [Description('test one two three')] TestOneTwoThree
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToCSHARPFriendlyEnum(string s)
        {
            var s1 = s.ToLower();
            //no symbols
            s1 = Regex.Replace(s1, "[^a-z0-9]", " ");
            //white space
            s1 = s1.Replace("  ", " ");
            s1 = s1.Replace("  ", " ");
            s1 = s1.Replace("  ", " ");

            //camel case
            s1 = Regex.Replace(s1, @"\s([a-z])", s2 => s2.ToString().ToUpper());
            //no space
            s1 = s1.Replace(" ", "");
            //first char cap
            s1 = s1[0].ToString().ToUpper() + new string(s1.Skip(1).ToArray());
            //no non alpha start char
            s1 = Regex.Replace(s1, "^[^A-Z]", s2 => "_" + s2);

            var ret = $"[Description(\"{s}\")]\n{s1}";
            return ret;
        }

#pragma warning disable 1570
        /// <summary>
        /// generate new enum rows of a type ordered by name
        /// eg var str = OrderCSHARPEnum<Enum Type>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string OrderCSHARPEnum<T>()
        {
            var t = typeof(T);
            var v1 = Enum.GetValues(t).Cast<T>();

            var v2 = v1.OrderBy(s => s.ToString()).Cast<Enum>().Select(s => $"[Description(\"{s.GetDescription()}\")] {s.ToString()}").ToList();

            var v3 = string.Join(",\n", v2);
            return v3;
        }

        private static char Separator = '\f';
        private static string Newline = "\r\n";

        /// <summary>
        /// get the name of a passed parameter
        /// </summary>
        /// <param name="memberExpression">() =&gt; variable</param>
        /// <returns>
        /// variable name
        /// </returns>
        public static string GetFieldName(Expression<Func<object>> memberExpression)
        {
            MemberExpression me = null;
            if (memberExpression.Body is MemberExpression)
                me = ((MemberExpression)memberExpression.Body);
            else if (memberExpression.Body is UnaryExpression)
            {
                var ue = ((UnaryExpression)memberExpression.Body);
                me = ue.Operand as MemberExpression;
            }

            if (me == null)
                return null;

            return me.Member.Name;
        }

        /// <summary>
        /// get a field or property of a class instance
        /// </summary>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="fieldname">The fieldname.</param>
        /// <returns></returns>
        public static object GetFieldValue(object classInstance, string fieldname)
        {
            object ret = null;
            var ty = classInstance.GetType();
            var field = ty.GetField(fieldname);
            var field2 = ty.GetProperty(fieldname);

            if (field != null)
                ret = field.GetValue(classInstance);
            else if (field2 != null)
                ret = field2.GetValue(classInstance, null);
            return ret;
        }

        /// <summary>
        /// get a tuple list of the type name and type values of an object
        /// </summary>
        /// <param name="classInstance">the class you want the values for</param>
        /// <returns></returns>
        public static List<Tuple<string, object>> GetFieldNamesAndValues(object classInstance)
        {
            var ty = classInstance.GetType();
            var fields = ty.GetFields();

            return fields.Select(v => new Tuple<string, object>(v.Name, v.GetValue(classInstance))).ToList();
        }

        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <param name="ty">The ty.</param>
        /// <returns></returns>
        public static List<string> GetFieldNames(Type ty)
        {
            var fields = ty.GetFields();
            var propInfos = ty.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var l1 = propInfos.Select(s => s.Name).ToList();
            var l2 = fields.Select(v => v.Name).ToList();
            l1.AddRange(l2);
            return l1;
        }

        /// <summary>
        /// serialise an object to a file
        /// </summary>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static bool SerialiseObject(object classInstance, string filename)
        {
            if (File.Exists(filename) == false)
                FileExtras.CreateFile(filename);

            var r = SerialiseObject(classInstance);

            FileExtras.SaveToFile(filename, r);
            return true;
        }

        /// <summary>
        /// serialise an object to a return string
        /// </summary>
        /// <param name="classInstance">The class instance.</param>
        /// <returns></returns>
        public static string SerialiseObject(object classInstance)
        {
            var r = "";
            var ol = GetFieldNamesAndValues(classInstance);

            foreach (var o in ol)
            {
                r += o.Item1 + Separator + o.Item2 + Newline;
            }
            return r;
        }

        /// <summary>
        /// deserialise a file to an object from a file
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="ignoreErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static object DeserialiseObject(string filename, Type objectType, bool ignoreErrors = true)
        {
            if (File.Exists(filename) == false)
                return null;

            var s = FileExtras.LoadFile(filename);

            if (string.IsNullOrEmpty(s))
                return null;

            var instance = DeserialiseObject(objectType, s, ignoreErrors);
            return instance;
        }

        /// <summary>
        /// deserialise an object from a serialised string
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="serialisedObjectString">The serialised object string.</param>
        /// <param name="ignoreErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static object DeserialiseObject(Type objectType, string serialisedObjectString, bool ignoreErrors = true)
        {
            var s2 = StringExtras.SplitString(serialisedObjectString, Newline);

            var tl = new List<Tuple<string, string>>();
            foreach (var s3 in s2)
            {
                var s4 = StringExtras.SplitString(s3, Separator.ToString());
                if (s4.Length != 2)
                {
                    if (ignoreErrors)
                        continue;
                    return null;
                }

                var fieldname = s4[0];
                var fieldval = s4[1];

                tl.Add(new Tuple<string, string>(fieldname, fieldval));
            }
            var instance = DeserialiseObject(objectType, tl, ignoreErrors);
            return instance;
        }

        /// <summary>
        /// deserialise an object from a list of tuple string,string s
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="objectFieldNameAndValues">field name,field val</param>
        /// <param name="ignoreErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static object DeserialiseObject(Type objectType, List<Tuple<string, string>> objectFieldNameAndValues,
            bool ignoreErrors = true)
        {
            var instance = Activator.CreateInstance(objectType);

            foreach (var t in objectFieldNameAndValues)
            {
                //try field
                var field = objectType.GetField(t.Item1);
                if (field != null)
                {
                    try
                    {
                        field.SetValue(instance, Convert.ChangeType(t.Item2, field.FieldType));
                    }
                    catch (Exception)
                    {
                        if (ignoreErrors)
                            continue;
                        return null;
                    }
                }

                //try property
                var pi = objectType.GetProperty(t.Item1);
                if (pi != null)
                {
                    try
                    {
                        pi.SetValue(instance, Convert.ChangeType(t.Item2, pi.PropertyType));
                    }
                    catch (Exception)
                    {
                        if (ignoreErrors)
                            continue;
                        return null;
                    }
                }

                //field hasnt been set
                if (field == null && pi == null)
                {
                    if (ignoreErrors)
                        continue;
                    return null;
                }
            }

            return instance;
        }
    }
}