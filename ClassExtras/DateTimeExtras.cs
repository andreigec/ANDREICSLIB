using System;
using System.Globalization;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeExtras
    {
        public static CultureInfo MyCulture = new CultureInfo("en-au");
        public static TimeZoneInfo MyTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

        #region returns local
        /// <summary>
        /// UTC to AEST conversion
        /// </summary>
        /// <param name="date">input date</param>
        /// <returns>
        /// Converted time
        /// </returns>
        public static DateTime ToLocal(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Local)
                return date;

            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var ret = TimeZoneInfo.ConvertTimeFromUtc(date, MyTimeZoneInfo);
            return DateTime.SpecifyKind(ret, DateTimeKind.Local);
        }

        /// <summary>
        /// AEST string to AEST datetime
        /// </summary>
        /// <param name="localDateTime">The local date time.</param>
        /// <returns></returns>
        public static DateTime ToLocal(string localDateTime)
        {
            var ret = ParseDateExactForTimeZone(localDateTime, MyTimeZoneInfo);
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
        /// <param name="hoursOffset">The hours offset.</param>
        /// <returns></returns>
        public static DateTime GetLocalMidnightInUtc(string localDateTime = null, int? hoursOffset = null)
        {
            var d = (localDateTime == null ? DateTime.UtcNow.ToLocal() : ToLocal(localDateTime)).Date;
            var offset = hoursOffset ?? 0;
            return ToUTCWithHoursOffset(d, offset);
        }

        /// <summary>
        /// Gets the local midnight in UTC.
        /// </summary>
        /// <param name="anyDateTime">Any date time.</param>
        /// <param name="hoursOffset">The hours offset.</param>
        /// <returns></returns>
        public static DateTime GetLocalMidnightInUtc(this DateTime anyDateTime, int? hoursOffset = null)
        {
            var d = anyDateTime.ToLocal().Date;
            return ToUTCWithHoursOffset(d, hoursOffset);
        }

        /// <summary>
        /// To the UTC with hours offset.
        /// </summary>
        /// <param name="hoursOffset">The hours offset.</param>
        /// <returns></returns>
        public static DateTime ToUTCWithHoursOffset(int? hoursOffset)
        {
            return ToUTCWithHoursOffset(GetLocal(), hoursOffset);
        }

        /// <summary>
        /// To the UTC with hours offset.
        /// </summary>
        /// <param name="localDateTime">The local date time.</param>
        /// <param name="hoursOffset">The hours offset.</param>
        /// <returns></returns>
        public static DateTime ToUTCWithHoursOffset(string localDateTime = null, int? hoursOffset = null)
        {
            var d = (localDateTime == null ? DateTime.UtcNow.ToLocal() : ToLocal(localDateTime));
            return ToUTCWithHoursOffset(d, hoursOffset);
        }

        /// <summary>
        /// convert date to utc, use 12am, and offset
        /// </summary>
        /// <param name="localDateTime">The local date time.</param>
        /// <param name="hoursOffset">The hours offset.</param>
        /// <returns></returns>
        public static DateTime ToUTCWithHoursOffset(this DateTime localDateTime, int? hoursOffset)
        {
            var ret = localDateTime;
            if (ret.Kind != DateTimeKind.Utc)
                ret = ret.ToUniversalTime();

            if (hoursOffset.HasValue)
                ret = ret.AddHours(hoursOffset.Value);

            return DateTime.SpecifyKind(ret, DateTimeKind.Utc);
        }

        #endregion returns UTC

        #region helpers

        /// <summary>
        /// Parses the date for time zone.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns></returns>
        private static DateTime ParseDateExactForTimeZone(string dateTime, TimeZoneInfo timezone)
        {
            var parsedDateLocal = DateTimeOffset.Parse(dateTime, MyCulture);
            var tzOffset = timezone.GetUtcOffset(parsedDateLocal.DateTime);
            var parsedDateTimeZone = new DateTimeOffset(parsedDateLocal.DateTime, tzOffset);
            return parsedDateTimeZone.DateTime;
        }

        #endregion helpers

        #region two

        /// <summary>
        /// Truncates the date
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        public static DateTime TruncateAndRound(this DateTime dateTime, int seconds)
        {
            return seconds <= 0 ? dateTime : dateTime.TruncateToSeconds(1).Round(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// truncate to seconds
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private static DateTime TruncateToSeconds(this DateTime dateTime, int seconds)
        {
            //truncate to nearest second
            return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.FromSeconds(seconds).Ticks));
        }

        /// <summary>
        /// Rounds the specified rounding interval.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="roundingInterval">The rounding interval.</param>
        /// <param name="roundingType">Type of the rounding.</param>
        /// <returns></returns>
        private static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType = MidpointRounding.ToEven)
        {
            var r = Math.Round(time.Ticks / (decimal)roundingInterval.Ticks, roundingType
                );

            var v = Convert.ToInt64(r) * roundingInterval.Ticks;
            return new TimeSpan(v);
        }

        /// <summary>
        /// Rounds the specified date
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="roundingInterval">The rounding interval.</param>
        /// <returns></returns>
        private static DateTime Round(this DateTime datetime, TimeSpan roundingInterval)
        {
            var t = (datetime - DateTime.MinValue).Round(roundingInterval).Ticks;
            return new DateTime(t, datetime.Kind);
        }
        #endregion two
    }
}