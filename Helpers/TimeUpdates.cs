﻿using System;
using System.Collections.Generic;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class TimeUpdates
    {
        public class TimeR
        {
            public TimeR(string name, Func<TimeSpan, double> get, double limit)
            {
                Name = name;
                Get = get;
                Limit = limit;
            }

            public string Name { get; set; }
            public Func<TimeSpan, double> Get { get; }
            public double Limit { get; }

            public static TimeR MS = new TimeR("Millisecond", s => s.TotalMilliseconds, 1001);
            public static TimeR S = new TimeR("Second", s => s.TotalSeconds, 61);
            public static TimeR M = new TimeR("Minute", s => s.TotalMinutes, 61);
            public static TimeR H = new TimeR("Hour", s => s.TotalHours, 24);
            public static TimeR D = new TimeR("Day", s => s.TotalDays, 265);

            public static List<TimeR> list = new List<TimeR>() { MS, S, M, H, D };
        }

        public static string TimeInWords(TimeSpan ts)
        {
            foreach (var l in TimeR.list)
            {
                var val = l.Get(ts);
                if (val < l.Limit)
                    return Pluralise(l.Name, val);
            }

            return Pluralise(TimeR.MS.Name, TimeR.MS.Get(ts));
        }

        public static string Pluralise(string word, double val)
        {
            string ret = word;
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
    }
}