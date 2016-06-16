using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class TimeUpdates
    {
        /// <summary>
        /// 
        /// </summary>
        public class TimeR
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeR"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="get">The get.</param>
            /// <param name="limit">The limit.</param>
            public TimeR(string name, Func<TimeSpan, double> get, double limit)
            {
                Name = name;
                Get = get;
                Limit = limit;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }
            /// <summary>
            /// Gets the get.
            /// </summary>
            /// <value>
            /// The get.
            /// </value>
            public Func<TimeSpan, double> Get { get; }
            /// <summary>
            /// Gets the limit.
            /// </summary>
            /// <value>
            /// The limit.
            /// </value>
            public double Limit { get; }

            /// <summary>
            /// </summary>
            public static TimeR Ms = new TimeR("Millisecond", s => s.TotalMilliseconds, 2001);
            /// <summary>
            /// </summary>
            public static TimeR S = new TimeR("Second", s => s.TotalSeconds, 121);
            /// <summary>
            /// </summary>
            public static TimeR M = new TimeR("Minute", s => s.TotalMinutes, 61);
            /// <summary>
            /// </summary>
            public static TimeR H = new TimeR("Hour", s => s.TotalHours, 24);
            /// <summary>
            /// </summary>
            public static TimeR D = new TimeR("Day", s => s.TotalDays, 265);

            /// <summary>
            /// The times
            /// </summary>
            public static List<TimeR> Times = new List<TimeR>() { Ms, S, M, H, D };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string TimeInWords(this TimeSpan ts)
        {
            foreach (var l in TimeR.Times)
            {
                var val = l.Get(ts);
                if (val <= l.Limit)
                    return Pluralise(l.Name, val);
            }

            return Pluralise(TimeR.Ms.Name, TimeR.Ms.Get(ts));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Pluralise(this string word, double val)
        {
            string ret = val.ToString("N") + " " + word;
            if (val != 1)
            {
                if (ret.EndsWith("s") == false)
                    ret = ret + "s";
            }
            else
            {
                if (ret.EndsWith("s"))
                    ret = StringExtras.ApplyTrim(ret, false, 1);
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ElapsedInWords(this Stopwatch t)
        {
            return TimeInWords(TimeSpan.FromMilliseconds(t.ElapsedMilliseconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string IntervalInWords(this Timer t)
        {
            return TimeInWords(TimeSpan.FromMilliseconds(t.Interval));
        }
    }
}