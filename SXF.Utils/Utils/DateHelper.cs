using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
