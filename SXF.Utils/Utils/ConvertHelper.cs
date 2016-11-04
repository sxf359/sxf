using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Utils
{
    public class ConvertHelper
    {
        /// <summary>
        /// 把object类型转换为整型(32 bit)
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回整型值</returns>
        public static int ToInt(object obj)
        {
            if (Convert.IsDBNull(obj))
                return 0;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把object类型转换为字符串
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回字符串</returns>
        public static string ToString(object obj)
        {
            if (Convert.IsDBNull(obj))
                return "";
            else
                return Convert.ToString(obj);
        }
        /// <summary>
        /// 把object类型转换为浮点型
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回浮点型值</returns>
        public static Single ToSingle(object obj)
        {
            if (Convert.IsDBNull(obj))
                return 0;
            try
            {
                return Convert.ToSingle(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把object类型转换为double
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回double值</returns>
        public static double ToDouble(object obj)
        {
            if (Convert.IsDBNull(obj))
                return 0;
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把object类型转换为decimail
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回double值</returns>
        public static decimal ToDecimal(object obj)
        {
            if (Convert.IsDBNull(obj))
                return 0;
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把object类型转换为日期型
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回日期型的值，如果出现转换异常，将返回1990-01-01</returns>
        public static DateTime ToDateTime(object obj)
        {
            if (Convert.IsDBNull(obj))
            {
                return new DateTime(1900, 1, 1);
            }
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }
        /// <summary>
        /// 把object类型转换为布尔型
        /// </summary>
        /// <param name="obj">要转换的值</param>
        /// <returns>返回布尔型的值，如果出现转换异常，将返回true</returns>
        public static bool ToBoolean(object obj)
        {
            if (Convert.IsDBNull(obj))
            {
                return true;
            }
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return true;
            }
        }
    }
}
