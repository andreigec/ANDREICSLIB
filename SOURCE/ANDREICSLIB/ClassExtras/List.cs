using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB.ClassExtras
{
    public static class ListUpdates
    {
        public static List<TNewType> ChangeListTyping<TNewType>(List<object> inlist) where TNewType : class
        {
            return inlist.Select(o => o as TNewType).ToList();
        }

        public static void Swap<T>(ref List<T> list,  int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public static String Serialise(List<object> list, String sep = ", ")
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

        public static bool ContainsLoopThrough<T>(List<object> list, T val)
        {
            return Enumerable.Contains(list, val);
        }
    }
}
