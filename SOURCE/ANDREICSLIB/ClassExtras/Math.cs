using System;

namespace ANDREICSLIB
{
	public static class MathUpdates
	{
		public static int Floor(int v)
		{
			var ret = (int)(Math.Floor((double)v));
			return ret;				
		}

		public static int Ceiling(int v)
		{
			var ret = (int)(Math.Ceiling((double)v));
			return ret;
		}
	}

}
