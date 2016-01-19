using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Transformers
{
    public class CSVTransform : ITransform
    {
        private static bool IsSimple(object o)
        {
            if (o == null)
                return true;
            var t = o.GetType();
            return (t == typeof(string) || t == typeof(int) || t == typeof(long));
        }

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
            return res.Trim(new[] { ',', ' ' });
        }

        public Result Save(string filename, Dictionary<string, object> content, List<string> startChildrenPoint, bool header, int? uniqueColumn)
        {
            var ret = new Result();
            try
            {
                var c = new CsvExport();

                c.AddRow();

                foreach (KeyValuePair<string, object> kvp in content)
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

        public static string[][] Load(string filename)
        {
            List<string[]> ret = new List<string[]>();

            var str = FileExtras.LoadFile(filename);
            if (str == null)
                return null;
            //split by new line
            var rows = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //split by ,
            foreach (var r in rows)
            {
                ret.Add(CSVSplitRow(r).ToArray());
            }

            return ret.ToArray();
        }

        private static List<string> CSVSplitRow(string s)
        {
            var ret = new List<string>();

            Regex regexObj = new Regex(@"""[^""\r\n]*""|'[^'\r\n]*'|[^,\r\n]+");
            Match matchResults = regexObj.Match(s);
            while (matchResults.Success)
            {
                ret.Add(matchResults.Value);
                matchResults = matchResults.NextMatch();
            }

            return ret;
        }
    }
}
