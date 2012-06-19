using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
	public class ColorUpdates
	{
		public static Color getNegative(Color inColour)
		{
			return Color.FromArgb(255 - inColour.R,
			  255 - inColour.G, 255 - inColour.B);
		}

		private static Dictionary<int, Color> colourcache;
		public static Color getColourFromInt(int p,int min=-100,int max=100)
		{
			if (colourcache==null)
				colourcache=new Dictionary<int, Color>();
			if (colourcache.ContainsKey(p))
				return colourcache[p];

			var r = 0;
			var g = 0;
			var b = 0;
			if (p < 0)
			{
				var rv = p;
				if (rv < min)
					rv = min;
				r = ((int)(((float)rv / (float)min) * 255.0));
			}
			else if (p > 0)
			{
				var gv = p;
				if (gv > max)
					gv = max;
				g = ((int)(((float)gv / (float)max) * 255.0));
			}
			var ret = Color.FromArgb(r, g, b);
			colourcache.Add(p, ret);
			return ret;
		}
	}
}
