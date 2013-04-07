using System;
using System.Collections.Generic;
using System.Linq;

namespace ANDREICSLIB.ClassExtras
{
    public static class ListExtras
    {
        public static List<TNewType> ChangeListTyping<TNewType>(List<object> inlist) where TNewType : class
        {
            return inlist.Select(o => o as TNewType).ToList();
        }

        public static List<List<T>> GetAllCombinations<T>(List<T> inlist, int minlength = 1, int maxlength = -1)
        {
            var ret = new List<List<T>>();

            //gradually take more and more items
            for (int takesize = minlength; takesize <= inlist.Count; takesize++)
            {
                if (takesize > maxlength && maxlength > -1)
                    return ret;

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

                    ret.Add(concat);
                }
            }

            return ret;
        }

        public static void Swap<T>(ref List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public static String Serialise(List<object> list, String sep = ", ")
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