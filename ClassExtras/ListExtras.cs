using System;
using System.Collections.Generic;
using System.Linq;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Phrase-Profiler
    /// </summary>
    public static class ListExtras
    {
        /// <summary>
        /// Distinct by 
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Gets all combinations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inlist">The inlist.</param>
        /// <param name="minlength">The minlength.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <returns></returns>
        public static List<List<T>> GetAllCombinations<T>(this List<T> inlist, int minlength = 1, int maxlength = -1)
        {
            var temp = new Dictionary<string, List<T>>();

            //gradually take more and more items
            for (var takesize = minlength; takesize <= inlist.Count; takesize++)
            {
                if (takesize > maxlength && maxlength > -1)
                    return temp.Select(s => s.Value).ToList();

                var breaktime = false;
                for (var take = 0; take < inlist.Count; take++)
                {
                    var concat = new List<T>();
                    for (var take2 = 0; take2 < takesize; take2++)
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

        /// <summary>
        /// Swaps the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="index2">The index2.</param>
        public static void Swap<T>(ref List<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// Serialises the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="sep">The sep.</param>
        /// <returns></returns>
        public static string Serialise(List<object> list, string sep = ", ")
        {
            var ret = "";
            foreach (var v in list)
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

        /// <summary>
        /// Determines whether [contains loop through] [the specified list].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool ContainsLoopThrough<T>(List<object> list, T val)
        {
            return Enumerable.Contains(list, val);
        }

        /// <summary>
        /// Intersects the specified one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Initialises the specified count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count">The count.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static List<T> Initialise<T>(int count, T val) where T : class
        {
            var ret = new List<T>();
            for (var a = 0; a < count; a++)
                ret.Add(val);

            return ret;
        }

        /// <summary>
        /// Inserts the alphabetically.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="toAdd">To add.</param>
        public static void InsertAlphabetically(ref List<string> l, List<string> toAdd)
        {
            foreach (var ta in toAdd)
            {
                InsertAlphabetically(ref l, ta);
            }
        }

        /// <summary>
        /// Inserts the alphabetically.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <param name="toAdd">To add.</param>
        public static void InsertAlphabetically(ref List<string> l, string toAdd)
        {
            int loc;
            for (loc = 0; loc < l.Count && l[loc].CompareTo(toAdd) < 0; loc++) ;
            l.Insert(loc, toAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class IntersectResult<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IntersectResult{T}"/> class.
            /// </summary>
            /// <param name="sameElements">The same elements.</param>
            /// <param name="oneOnly">The one only.</param>
            /// <param name="twoOnly">The two only.</param>
            public IntersectResult(List<T> sameElements, List<T> oneOnly, List<T> twoOnly)
            {
                SameElements = sameElements;
                OneOnly = oneOnly;
                TwoOnly = twoOnly;
            }

            /// <summary>
            /// Gets or sets the same elements.
            /// </summary>
            /// <value>
            /// The same elements.
            /// </value>
            public List<T> SameElements { get; set; }
            /// <summary>
            /// Gets or sets the one only.
            /// </summary>
            /// <value>
            /// The one only.
            /// </value>
            public List<T> OneOnly { get; set; }
            /// <summary>
            /// Gets or sets the two only.
            /// </summary>
            /// <value>
            /// The two only.
            /// </value>
            public List<T> TwoOnly { get; set; }
        }
    }
}