using System;
using System.Collections.Generic;
using System.Linq;

namespace ANDREICSLIB.ClassExtras
{
    public static class ListExtras
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
  (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static List<List<T>> GetAllCombinations<T>(this List<T> inlist, int minlength = 1, int maxlength = -1)
        {
            var temp = new Dictionary<string, List<T>>();

            //gradually take more and more items
            for (var takesize = minlength; takesize <= inlist.Count; takesize++)
            {
                if (takesize > maxlength && maxlength > -1)
                    return temp.Select(s => s.Value).ToList();

                bool breaktime = false;
                for (int take = 0; take < inlist.Count; take++)
                {
                    var concat = new List<T>();
                    for (int take2 = 0; take2 < takesize; take2++)
                    {
                        if ((take + takesize) > inlist.Count)
                        {
                            breaktime = true;
                            break;
                        }

                        if (concat.Contains(inlist[take + take2]) == false)
                            concat.Add(inlist[take + take2]);
                    }

                    if (breaktime)
                        break;

                    var concatKey = concat.Aggregate("", (a, b) => a + b);

                    if (temp.ContainsKey(concatKey) == false)
                        temp.Add(concatKey, concat);
                }
            }
            return temp.Select(s => s.Value).ToList();
        }

        public static void Swap<T>(ref List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public static string Serialise(List<object> list, string sep = ", ")
        {
            string ret = "";
            foreach (object v in list)
            {
                if (ret.Length == 0)
                    ret = v.ToString();
                else
                {
                    ret += sep + v;
                }
            }

            return ret;
        }

        public static bool ContainsLoopThrough<T>(List<object> list, T val)
        {
            return Enumerable.Contains(list, val);
        }
    }
}