using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Utils
{
    public static partial class ExtensionMethod
    {



        #region string 扩展
        public static bool IsNullOrEmpty(this string str)
        {
            return StringHelper.IsNullOrEmpty(str);
        }
        public static bool IsNotNullAndEmpty(this string str)
        {
            return StringHelper.IsNotNullAndEmpty(str);
        }
        public static bool IsInteger(this string str)
        {
            return StringHelper.IsInteger(str);
        }
       
        
 

        public static bool IsNumber(this string str)
        {
            return StringHelper.IsNumber(str);
        }
       

       

        public static bool IsIDCard(this string str)
        {
            return StringHelper.IsIDCard(str);
        }
        #endregion

        #region 转换
        public static int ToInteger(this string str)
        {
            return int.Parse(str);
        }

        public static double ToNumber(this string str)
        {
            return double.Parse(str);
        }

        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }
        public static string FormatWith(this string str, params object[] objs)
        {
            return string.Format(str, objs);
        }
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// 
        public static string ToCamel(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            else
            {
                if (str.Length > 1)
                {
                    return str[0].ToString().ToLower() + str.Substring(1);
                }
                else
                {
                    return str.ToLower();
                }
            }
        }
        /// <summary>
        ///首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPascal(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            else
            {
                if (str.Length > 1)
                {
                    return str[0].ToString().ToUpper() + str.Substring(1);
                }
                else
                {
                    return str.ToLower();
                }
            }
        }

       

        #endregion

      

    }
}
