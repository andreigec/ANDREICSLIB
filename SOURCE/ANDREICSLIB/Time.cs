using System;

namespace ANDREICSLIB
{
    public static class TimeUpdates
    {
        public static double GetHours(double totalsecondsin)
        {
            return (totalsecondsin/3600.0)%24;
        }

        public static double GetMinutes(double totalsecondsin)
        {
            return (totalsecondsin/60.0)%60;
        }

        public static double GetDays(double totalsecondsin)
        {
            return ((totalsecondsin/3600.0)/24.0);
        }

        public static string TimeInWords(double totalsecondsin)
        {
            var seconds = (int) (totalsecondsin%60);
            var minutes = (int) Math.Floor(GetMinutes(totalsecondsin));
            var hours = (int) Math.Floor(GetHours(totalsecondsin));
            var days = (int) Math.Floor(GetDays(totalsecondsin));

            string ret = "";
            if (days != 0)
                ret += "\t" + days + " " + Pluralise("Day", days);
            if (hours != 0)
                ret += "\t" + hours + " " + Pluralise("Hour", hours);
            if (minutes != 0)
                ret += "\t" + minutes + " " + Pluralise("Minute", minutes);
            ret += "\t" + seconds + " " + Pluralise("Second", seconds);

            return ret;
        }

        public static string Pluralise(String word, int val)
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
                    ret = StringUpdates.ApplyTrim(ret, false, 1);
            }
            return ret;
        }
    }
}