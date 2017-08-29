using System;

namespace SXF.Utils
{
    public class DateHelper
    {
        #region 常用日期函数
        /// <summary>
        /// 上月第一天
        /// </summary>
        public static string PreMonthFirstDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 上月最后一天
        /// </summary>
        public static string PreMonthLastDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyy-MM-dd"); }
        }
        /// <summary>
        /// 上周一
        /// </summary>
        public static string PreWeekMonday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(-7).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 上周日
        /// </summary>
        public static string PreWeekSunday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 昨天
        /// </summary>
        public static string Yesterday
        {
            get { return DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd"); }
        }
        /// <summary>
        /// 今天
        /// </summary>
        public static string Today
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本周一
        /// </summary>
        public static string ThisWeekMonday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本周日
        /// </summary>
        public static string ThisWeekSunday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(6).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本月第一天
        /// </summary>
        public static string ThisMonthFirstDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本月最后一天
        /// </summary>
        public static string ThisMonthLastDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 长日期转换为yyyy-mm-dd形式显示
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertDateToYYYYMMDD(object date)
        {
            if (date.IsNullOrEmpty())
            {
                return "";
            }
            return System.Convert.ToDateTime(date).ToString("yyyy-MM-dd");
        }
        #endregion

        public static long DateDiff(DateInterval interval, DateTime date1, DateTime date2)
        {

            TimeSpan ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;
                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
                case DateInterval.Weekday:
                    return Fix(ts.TotalDays) / 7;
                case DateInterval.Day:
                    return Fix(ts.TotalDays);
                case DateInterval.Hour:
                    return Fix(ts.TotalHours);
                case DateInterval.Minute:
                    return Fix(ts.TotalMinutes);
                default:
                    return Fix(ts.TotalSeconds);
            }
        }

        private static long Fix(double number)
        {
            if (number >= 0)
            {
                return (long)Math.Floor(number);
            }
            return (long)Math.Ceiling(number);
        }
    }

    public enum DateInterval
    {
        Year,
        Month,
        Weekday,
        Day,
        Hour,
        Minute,
        Second
    }
}
