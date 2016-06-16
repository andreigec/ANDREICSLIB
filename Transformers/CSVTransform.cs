using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Transformers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ANDREICSLIB.Transformers.ITransform" />
    public class CSVTransform : ITransform
    {
        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="content">The content.</param>
        /// <param name="startChildrenPoint">The start children point.</param>
        /// <param name="header">if set to <c>true</c> [header].</param>
        /// <param name="uniqueColumn">The unique column.</param>
        /// <returns></returns>
        public Result Save(string filename, Dictionary<string, object> content, List<string> startChildrenPoint,
            bool header, int? uniqueColumn)
        {
            var ret = new Result();
            try
            {
                var c = new CsvExport();

                c.AddRow();

                foreach (var kvp in content)
                {
                    var key = kvp.Key;
                    var str = kvp.Value;
                    if (IsSimple(kvp.Value) == false)
                    {
                        str = PullOut((IEnumerable)kvp.Value);
                    }

                    c[key] = str;
                }

                ret.Items.Add("extracted:" + content.Count);
                c.ExportToFile(filename, header, uniqueColumn);
                ret.Status = content.Count > 0;
            }
            catch (Exception ex)
            {
                ret.ErrorStatus = ex.ToString();
                ret.Items.Add(ex.ToString());
            }

            return ret;
        }

        /// <summary>
        /// Determines whether the specified o is simple.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        private static bool IsSimple(object o)
        {
            if (o == null)
                return true;
            var t = o.GetType();
            return (t == typeof(string) || t == typeof(int) || t == typeof(long));
        }

        /// <summary>
        /// Pulls the out.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">a.</param>
        /// <returns></returns>
        public string PullOut<T>(T a) where T : IEnumerable
        {
            var res = "";
            foreach (var aa in a)
            {
                if (aa.GetType() == typeof(Dictionary<string, object>))
                {
                    var aaa = aa as Dictionary<string, object>;
                    res += "{";

                    foreach (var k in aaa)
                    {
                        if (IsSimple(k.Value))
                            res += k.Key + ":" + k.Value;
                        else
                            res += PullOut((IEnumerable)k.Value);
                    }
                    res += "}";
                }
                else if (aa is IEnumerable && IsSimple(aa) == false)
                {
                    res += "[" + PullOut((IEnumerable)aa) + "]";
                }
                else
                    res += "-" + aa;
            }
            return res.Trim(',', ' ');
        }

        /// <summary>
        /// Loads the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string[][] LoadFile(string filename)
        {
            var str = FileExtras.LoadFile(filename);
            if (str == null)
                return null;

            return LoadContent(str);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static string[][] LoadContent(string content)
        {
            var ret = new List<string[]>();
            //split by new line
            var rows = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //split by ,
            foreach (var r in rows)
            {
                ret.Add(CSVSplitRow(r).ToArray());
            }

            return ret.ToArray();
        }

        /// <summary>
        /// CSVs the split row.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private static List<string> CSVSplitRow(string s)
        {
            var ret = new List<string>();

            var regexObj = new Regex(@"""[^""\r\n]*""|'[^'\r\n]*'|[^,\r\n]+");
            var matchResults = regexObj.Match(s);
            while (matchResults.Success)
            {
                ret.Add(matchResults.Value.Trim());
                matchResults = matchResults.NextMatch();
            }

            return ret;
        }
    }
}