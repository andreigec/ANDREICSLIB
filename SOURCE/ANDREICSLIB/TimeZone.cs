using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ANDREICSLIB
{
    public class CustomTimeZones
    {
        public static List<CustomTimeZones> Zones = new List<CustomTimeZones>();
        public TimeSpan UTCoffset;
        public TimeZoneInfo Id;
        public string name;

        public CustomTimeZones(TimeZoneInfo idIN, string nameIN, TimeSpan UTCIN)
        {
            Id = idIN;
            name = nameIN;
            UTCoffset = UTCIN;
        }

        public static CustomTimeZones GetMyTimeZone()
        {
            TimeZone TZ = TimeZone.CurrentTimeZone;
            string idstr = TZ.StandardName;
            if (TZ.IsDaylightSavingTime(DateTime.Now))
            {
                idstr = TZ.DaylightName;
            }
            return Zones.Find(p => p.name.Equals(idstr));
        }

        public new string ToString()
        {
            string sign = "+";
            if (UTCoffset.CompareTo(new TimeSpan(0, 0, 0)) < 0)
                sign = "";
            int min = Math.Abs(UTCoffset.Minutes);
            string mins = min.ToString(CultureInfo.InvariantCulture);
            if (mins == "0")
                mins = "00";
            return "(UTC" + sign + UTCoffset.Hours + ":" + mins + ")" + " " + name;
        }

        public static CustomTimeZones FromString(string s)
        {
            return Zones.FirstOrDefault(v => v.ToString().Equals(s));
        }

        private static int ComparisonTime(CustomTimeZones t1, CustomTimeZones t2)
        {
            return t1.UTCoffset.CompareTo(t2.UTCoffset);
        }

        public static void Sort(bool byName)
        {
            if (byName)
                Zones.Sort((p1, p2) => string.Compare(p1.name, p2.name, StringComparison.Ordinal));
            else
                Zones.Sort(ComparisonTime);
        }

        /// <summary>
        /// Create the list of timezones before using
        /// </summary>
        public static void Create()
        {
            ReadOnlyCollection<TimeZoneInfo> x = TimeZoneInfo.GetSystemTimeZones();
            DateTime timenow = DateTime.Now;
            foreach (TimeZoneInfo y in x)
            {
                string n;
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
                Zones.Add(new CustomTimeZones(y, n, UTCoff));
            }
            Sort(true);
        }
    }
}