using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
	public class ListUpdates<T> : List<T>
	{
		public void Swap(int index1, int index2)
		{
			var temp = this[index1];
			this[index1] = this[index2];
			this[index2] = temp;
		}

		public String Serialise(String sep = ", ")
		{
			var ret = "";
			foreach (var v in this)
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

		public bool ContainsLoopThrough(T val)
		{
		    return Enumerable.Contains(this, val);
		}
	}
}
