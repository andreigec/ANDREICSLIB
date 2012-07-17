using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB
{
    public static class TimeUpdates
    {
        public static string TimeInWords(int totalsecondsin)
        {
            int seconds = totalsecondsin % 60;
            int minutes = (totalsecondsin / 60) % 60;
            int hours = (totalsecondsin / 3600) % 24;
            int days = MathUpdates.Floor((totalsecondsin / 3600)/24);

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
            var ret = word;
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
