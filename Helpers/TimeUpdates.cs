using System;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class TimeUpdates
    {
        /// <summary>
        /// Gets the hours.
        /// </summary>
        /// <param name="totalsecondsin">The totalsecondsin.</param>
        /// <returns></returns>
        public static double GetHours(double totalsecondsin)
        {
            return (totalsecondsin/3600.0)%24;
        }

        /// <summary>
        /// Gets the minutes.
        /// </summary>
        /// <param name="totalsecondsin">The totalsecondsin.</param>
        /// <returns></returns>
        public static double GetMinutes(double totalsecondsin)
        {
            return (totalsecondsin/60.0)%60;
        }

        /// <summary>
        /// Gets the days.
        /// </summary>
        /// <param name="totalsecondsin">The totalsecondsin.</param>
        /// <returns></returns>
        public static double GetDays(double totalsecondsin)
        {
            return ((totalsecondsin/3600.0)/24.0);
        }

        /// <summary>
        /// Times the in words.
        /// </summary>
        /// <param name="totalsecondsin">The totalsecondsin.</param>
        /// <returns></returns>
        public static string TimeInWords(double totalsecondsin)
        {
            var seconds = (int) (totalsecondsin%60);
            var minutes = (int) Math.Floor(GetMinutes(totalsecondsin));
            var hours = (int) Math.Floor(GetHours(totalsecondsin));
            var days = (int) Math.Floor(GetDays(totalsecondsin));

            var ret = "";
            if (days != 0)
                ret += "\t" + days + " " + Pluralise("Day", days);
            if (hours != 0)
                ret += "\t" + hours + " " + Pluralise("Hour", hours);
            if (minutes != 0)
                ret += "\t" + minutes + " " + Pluralise("Minute", minutes);
            ret += "\t" + seconds + " " + Pluralise("Second", seconds);

            return ret;
        }

        /// <summary>
        /// Pluralises the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static string Pluralise(string word, int val)
        {
            var ret = word;
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