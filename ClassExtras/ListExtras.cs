using System;
using System.Collections.Generic;
using System.Linq;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Phrase-Profiler
    /// </summary>
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

        public class IntersectResult<T>
        {
            public IntersectResult(List<T> sameElements, List<T> oneOnly, List<T> twoOnly)
            {
                SameElements = sameElements;
                OneOnly = oneOnly;
                TwoOnly = twoOnly;
            }

            public List<T> SameElements { get; set; }
            public List<T> OneOnly { get; set; }
            public List<T> TwoOnly { get; set; }
        }

        public static IntersectResult<T> Intersect<T>(List<T> one, List<T> two)
        {
            var same = new List<T>();
            var oneonly = new List<T>();
            var twoonly = new List<T>();

            var merged = one.Select(s => s).ToList();
            merged.AddRange(two);
            merged = merged.Distinct().ToList();

            foreach (var t in merged)
            {
                var onehas = one.Contains(t);
                var twohas = two.Contains(t);
                if (onehas && twohas)
                    same.Add(t);
                else if (onehas)
                    oneonly.Add(t);
                else
                    twoonly.Add(t);
            }

            var ret = new IntersectResult<T>(same, oneonly, twoonly);
            return ret;
        }

        public static List<T> Initialise<T>(int count, T val) where T : class
        {
            var ret = new List<T>();
            for (int a = 0; a < count; a++)
                ret.Add(val);

            return ret;
        }

        public static void InsertAlphabetically(ref List<string> l, List<string> toAdd)
        {
            foreach (var ta in toAdd)
            {
                InsertAlphabetically(ref l, ta);
            }
        }

        public static void InsertAlphabetically(ref List<string> l, string toAdd)
        {
            int loc;
            for (loc = 0; loc < l.Count && l[loc].CompareTo(toAdd) < 0; loc++) ;
            l.Insert(loc, toAdd);
        }
    }
}