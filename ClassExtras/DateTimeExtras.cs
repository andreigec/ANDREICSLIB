using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANDREICSLIB.ClassExtras
{
    public static class DateTimeExtras
    {
        #region returns local
        /// <summary>
        /// UTC to AEST conversion
        /// </summary>
        /// <param name="date">input date</param>
        /// <returns>Converted time</returns>
        public static DateTime ToLocal(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Local)
                return date;

            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var ret = TimeZoneInfo.ConvertTimeFromUtc(date, GetTimeZoneInfo());
            return DateTime.SpecifyKind(ret, DateTimeKind.Local);
        }

        /// <summary>
        /// AEST string to AEST datetime
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <returns></returns>
        public static DateTime ToLocal(string localDateTime)
        {
            var ret = ParseDateExactForTimeZone(localDateTime, GetTimeZoneInfo());
            return DateTime.SpecifyKind(ret, DateTimeKind.Local);
        }

        /// <summary>
        /// current AEST datetime
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLocal()
        {
            var ret = DateTime.UtcNow.ToLocal();
            return DateTime.SpecifyKind(ret, DateTimeKind.Local);
        }
        #endregion returns local

        #region returns UTC
        /// <summary>
        /// AEST string to AEST datetime (default will use now), 12am, to UTC datetime (optional offset)
        /// </summary>
        /// <param name="localDateTime">if null will use current datetime</param>
        /// <returns></returns>
        public static DateTime GetLocalMidnightInUtc(string localDateTime = null, int? hoursOffset = null)
        {
            var d = (localDateTime == null ? DateTime.UtcNow.ToLocal() : ToLocal(localDateTime)).Date;
            var offset = hoursOffset ?? 0;
            return ToUTCWithHoursOffset(d, offset);
        }

        public static DateTime GetLocalMidnightInUtc(this DateTime anyDateTime, int? hoursOffset = null)
        {
            var d = anyDateTime.ToLocal().Date;
            return ToUTCWithHoursOffset(d, hoursOffset);
        }

        public static DateTime ToUTCWithHoursOffset(int? hoursOffset)
        {
            return ToUTCWithHoursOffset(GetLocal(), hoursOffset);
        }

        public static DateTime ToUTCWithHoursOffset(string localDateTime = null, int? hoursOffset = null)
        {
            var d = (localDateTime == null ? DateTime.UtcNow.ToLocal() : ToLocal(localDateTime));
            return ToUTCWithHoursOffset(d, hoursOffset);
        }

        /// <summary>
        /// convert date to utc, use 12am, and offset
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <returns></returns>
        public static DateTime ToUTCWithHoursOffset(this DateTime localDateTime, int? hoursOffset)
        {
            DateTime ret = localDateTime;
            if (ret.Kind != DateTimeKind.Utc)
                ret = ret.ToUniversalTime();

            if (hoursOffset.HasValue)
                ret = ret.AddHours(hoursOffset.Value);

            return DateTime.SpecifyKind(ret, DateTimeKind.Utc);
        }
        #endregion returns UTC

        #region helpers
        private static TimeZoneInfo GetTimeZoneInfo()
        {
            var cstZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            return cstZone;
        }

        private static CultureInfo GetCultureInfo()
        {
            return new CultureInfo("en-au");
        }
        private static DateTime ParseDateExactForTimeZone(string dateTime, TimeZoneInfo timezone)
        {
            var parsedDateLocal = DateTimeOffset.Parse(dateTime, GetCultureInfo());
            var tzOffset = timezone.GetUtcOffset(parsedDateLocal.DateTime);
            var parsedDateTimeZone = new DateTimeOffset(parsedDateLocal.DateTime, tzOffset);
            return parsedDateTimeZone.DateTime;
        }
        #endregion helpers

        #region two
        public static DateTime TruncateAndRound(this DateTime dateTime, int seconds)
        {
            return seconds <= 0 ? dateTime : dateTime.Truncate().Round(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// truncate to the second
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        private static DateTime Truncate(this DateTime dateTime)
        {
            //truncate to nearest second
            return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.FromSeconds(1).Ticks));
        }

        private static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType)
        {
            var r = Math.Round(time.Ticks / (decimal)roundingInterval.Ticks, roundingType
                );

            var v = Convert.ToInt64(r) * roundingInterval.Ticks;
            return new TimeSpan(v);
        }

        private static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval)
        {
            return Round(time, roundingInterval, MidpointRounding.ToEven);
        }

        private static DateTime Round(this DateTime datetime, TimeSpan roundingInterval)
        {
            var t = (datetime - DateTime.MinValue).Round(roundingInterval).Ticks;
            return new DateTime(t, datetime.Kind);
        }
        #endregion two
    }
}
