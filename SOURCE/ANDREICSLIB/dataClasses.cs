using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
	public static class DataClasses
	{
		#region Binary Tree
		public class Btree<T>
		{
			public T name;
			public Btree<T> parent;
			public List<Btree<T>> children;

			public void clearChildren()
			{
				if (children == null)
					children = new List<Btree<T>>();
				children.Clear();
			}

			public Btree<T> getChildByName(T nameC)
			{
				if (children == null)
					return null;
				return children.FirstOrDefault(v => v.name.Equals(nameC));
			}

			public void addChild(T nameC)
			{
				var t = new Btree<T>();
				t.name = nameC;
				t.parent = this;
				if (children == null)
					children = new List<Btree<T>>();
				children.Add(t);
			}

			public void removeChild(T nameC)
			{
			redo:
				for (var a = 0; a < children.Count; a++)
				{
					if (children[a].name.Equals(nameC))
					{
						children.RemoveAt(a);
						goto redo;
					}
				}
			}
		}
		#endregion
	}
}
