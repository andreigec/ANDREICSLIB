using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
	public class CustomTimeZones
	{
		public TimeZoneInfo id;
		public string name;
		public TimeSpan UTCoffset;

		public static List<CustomTimeZones> zones = new List<CustomTimeZones>();

		public static CustomTimeZones getMyTimeZone()
		{
			var TZ= TimeZone.CurrentTimeZone;
			String idstr = TZ.StandardName;
			if (TZ.IsDaylightSavingTime(DateTime.Now))
			{
				idstr = TZ.DaylightName;
			}
			return zones.Find(p=>p.name.Equals(idstr));
		}

		public new string ToString()
		{
			String sign = "+";
			if (UTCoffset.CompareTo(new TimeSpan(0,0,0)) < 0)
				sign = "";
			int min = Math.Abs(UTCoffset.Minutes);
			String mins = min.ToString();
			if (mins == "0")
				mins = "00";
			return "(UTC"+sign+UTCoffset.Hours + ":" + mins + ")" + " " + name;
		}

		public static CustomTimeZones FromString(String s)
		{
			foreach (var v in zones)
			{
				if (v.ToString().Equals(s))
					return v;
			}
			return null;
		}

		public CustomTimeZones(TimeZoneInfo idIN, String nameIN, TimeSpan UTCIN)
		{
			id = idIN;
			name = nameIN;
			UTCoffset = UTCIN;
		}

		private static int comparisonTime(CustomTimeZones t1, CustomTimeZones t2)
		{
			return t1.UTCoffset.CompareTo(t2.UTCoffset);
		}

		public static void Sort(bool byName)
		{
			if (byName)
				zones.Sort((p1, p2) => p1.name.CompareTo(p2.name));
			else
				zones.Sort(comparisonTime);
		}

		/// <summary>
		/// Create the list of timezones before using
		/// </summary>
		public static void Create()
		{
			var x = TimeZoneInfo.GetSystemTimeZones();
			var timenow = DateTime.Now;
			foreach (var y in x)
			{
				String n;
				TimeSpan UTCoff;
				if (y.IsDaylightSavingTime(timenow))
				{
					n = y.DaylightName;
					UTCoff = y.BaseUtcOffset.Add(TimeSpan.FromHours(1.0));
				}
				else
				{
					n = y.StandardName;
					UTCoff = y.BaseUtcOffset;
				}
				zones.Add(new CustomTimeZones(y,n, UTCoff));
			}
			Sort(true);
		}
	}
}
