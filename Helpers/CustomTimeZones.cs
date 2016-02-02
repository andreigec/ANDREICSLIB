using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Timezone-Sleep-Converter
    /// </summary>
    public class CustomTimeZones
    {
        /// <summary>
        /// The zones
        /// </summary>
        public static List<CustomTimeZones> Zones = new List<CustomTimeZones>();
        /// <summary>
        /// The identifier
        /// </summary>
        public TimeZoneInfo Id;
        /// <summary>
        /// The name
        /// </summary>
        public string name;
        /// <summary>
        /// The ut coffset
        /// </summary>
        public TimeSpan UTCoffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimeZones"/> class.
        /// </summary>
        /// <param name="idIN">The identifier in.</param>
        /// <param name="nameIN">The name in.</param>
        /// <param name="UTCIN">The utcin.</param>
        public CustomTimeZones(TimeZoneInfo idIN, string nameIN, TimeSpan UTCIN)
        {
            Id = idIN;
            name = nameIN;
            UTCoffset = UTCIN;
        }

        /// <summary>
        /// Gets my time zone.
        /// </summary>
        /// <returns></returns>
        public static CustomTimeZones GetMyTimeZone()
        {
            var TZ = TimeZone.CurrentTimeZone;
            var idstr = TZ.StandardName;
            if (TZ.IsDaylightSavingTime(DateTime.Now))
            {
                idstr = TZ.DaylightName;
            }
            return Zones.Find(p => p.name.Equals(idstr));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public new string ToString()
        {
            var sign = "+";
            if (UTCoffset.CompareTo(new TimeSpan(0, 0, 0)) < 0)
                sign = "";
            var min = Math.Abs(UTCoffset.Minutes);
            var mins = min.ToString(CultureInfo.InvariantCulture);
            if (mins == "0")
                mins = "00";
            return "(UTC" + sign + UTCoffset.Hours + ":" + mins + ")" + " " + name;
        }

        /// <summary>
        /// Froms the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static CustomTimeZones FromString(string s)
        {
            return Zones.FirstOrDefault(v => v.ToString().Equals(s));
        }

        /// <summary>
        /// Comparisons the time.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        private static int ComparisonTime(CustomTimeZones t1, CustomTimeZones t2)
        {
            return t1.UTCoffset.CompareTo(t2.UTCoffset);
        }

        /// <summary>
        /// Sorts the specified by name.
        /// </summary>
        /// <param name="byName">if set to <c>true</c> [by name].</param>
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
            var x = TimeZoneInfo.GetSystemTimeZones();
            var timenow = DateTime.Now;
            foreach (var y in x)
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