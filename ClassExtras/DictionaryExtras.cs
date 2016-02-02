using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Music-File-Info-Editor
    /// </summary>
    public static class DictionaryExtras
    {
        /// <summary>
        /// Gets the key by value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static T GetKeyByValue<T, Y>(Dictionary<T, Y> dict, Y val)
        {
            var bc = dict.Where(s => s.Value.Equals(val)).ToList();
            if (!bc.Any())
                return default(T);

            return bc.First().Key;
        }

        /// <summary>
        /// Merges the two dictionaries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="keep">The keep.</param>
        /// <param name="mergein">The mergein.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> [overwrite existing].</param>
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
        /// <param name="origDict">Input Dictionary, string as the key for the text/name, and a list of strings in the value, for
        /// subitems</param>
        /// <returns>
        /// a list of listviewitems made from a dictionary
        /// </returns>
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
        /// <returns>
        /// a list of listviewitems made from a dictionary
        /// </returns>
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



        /// <summary>
        /// Serialises the specified json.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="json">The json.</param>
        /// <param name="fs">The fs.</param>
        public static void Serialise(this Dictionary<string, object> dictionary, JsonSerializer json, FileStream fs)
        {
            //erase
            fs.SetLength(0);
            using (var writer = new StreamWriter(fs))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    var ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, dictionary);
                    jsonWriter.Flush();
                }
            }
        }

        /// <summary>
        /// load json from a filestream into a dictionary
        /// </summary>
        /// <param name="stream">The s.</param>
        /// <returns></returns>
        public static Dictionary<string, object> Deserialize(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var ser = new JsonSerializer();
                    var ret = ser.Deserialize<Dictionary<string, object>>(jsonReader);
                    return ret;
                }
            }
        }

        /// <summary>
        /// Deserializes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static Dictionary<string, object> Deserialize(string url)
        {
            var res = NetExtras.GetWebPageStream(url);
            var ret = Deserialize(res.Item1);
            return ret;
        }

        /// <summary>
        /// remove empty keys and values
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public static void RemoveEmptyKeysAndValues(ref Dictionary<string, object> dictionary)
        {
            var keys = dictionary.Keys.ToList();
            //remove empty keys
            var remove = keys.Where(string.IsNullOrWhiteSpace).ToList();
            //remove empty values
            foreach (var kvp in dictionary)
            {
                var v = kvp.Value?.ToString();
                if (string.IsNullOrEmpty(v))
                    remove.Add(kvp.Key);
            }
            foreach (var r in remove)
            {
                dictionary.Remove(r);
            }
        }
    }
}