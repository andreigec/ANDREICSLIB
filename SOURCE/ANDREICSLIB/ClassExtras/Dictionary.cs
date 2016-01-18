using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ANDREICSLIB.ClassExtras
{
    public static class DictionaryExtras
    {
        public static T GetKeyByValue<T, Y>(Dictionary<T, Y> dict, Y val)
        {
            IEnumerable<KeyValuePair<T, Y>> bc = dict.Where(s => s.Value.Equals(val));
            if (!bc.Any())
                return default(T);

            return bc.First().Key;
        }

        public static void MergeTwoDictionaries<T, Y>(ref Dictionary<T, Y> keep, Dictionary<T, Y> mergein,
                                                      bool overwriteExisting = true)
        {
            lock (mergein)
            {
                foreach (var kvp in mergein)
                {
                    if (keep.ContainsKey(kvp.Key) == false)
                        keep.Add(kvp.Key, kvp.Value);
                    else
                    {
                        if (overwriteExisting)
                            keep[kvp.Key] = kvp.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Convert a dictionary of key/value to a list of listviewitems
        /// </summary>
        /// <param name="origDict">Input Dictionary, string as the key for the text/name, and a list of strings in the value, for subitems</param>
        /// <returns>a list of listviewitems made from a dictionary</returns>
        public static List<ListViewItem> DictToListOfListViewItems(this Dictionary<string, List<string>> origDict)
        {
            var lvil = new List<ListViewItem>();

            foreach (var kvp in origDict)
            {
                var lvi = new ListViewItem { Text = kvp.Key, Name = kvp.Key };
                foreach (var s in kvp.Value)
                    lvi.SubItems.Add(s);
                lvil.Add(lvi);
            }
            return lvil;
        }

        /// <summary>
        /// Convert a dictionary of key/value to a list of listviewitems
        /// </summary>
        /// <param name="origDict">Input Dictionary, string as the key for the text/name, and a string in the value, for a subitem</param>
        /// <returns>a list of listviewitems made from a dictionary</returns>
        public static List<ListViewItem> DictToListOfListViewItems(this Dictionary<string, string> origDict)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var kvp in origDict)
            {
                var newl = new List<string> { kvp.Value };
                result.Add(kvp.Key, newl);
            }
            return DictToListOfListViewItems(result);
        }

        public static void Serialise(this Dictionary<string, object> d, JsonSerializer js, FileStream fs)
        {
            //erase
            fs.SetLength(0);
            using (var writer = new StreamWriter(fs))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, d);
                    jsonWriter.Flush();
                }
            }
        }

        /// <summary>
        /// load json from a filestream into a dictionary
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Deserialize(FileStream s)
        {
            using (var reader = new StreamReader(s))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer ser = new JsonSerializer();
                    var ret = ser.Deserialize<Dictionary<string, object>>(jsonReader);
                    return ret;
                }
            }
        }
    }
}