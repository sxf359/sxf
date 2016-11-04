using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SXF.Utils
{
    public static partial class ExtensionMethod
    {



        #region string判断扩展
        public static bool IsNullOrEmpty(this string str)
        {
            return StringHelper.IsNullOrEmpty(str);
        }

        public static bool IsNotNullAndEmpty(this string str)
        {
            return StringHelper.IsNotNullAndEmpty(str);
        }

        /// <summary>
        /// 判断是否是整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInteger(this string str)
        {
            return StringHelper.IsInteger(str);
        }

        /// <summary>
        /// 判断字符串是否是正整数，可以是无数位
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger(this string str)
        {
            return StringHelper.IsPositiveInteger(str);
        }
        /// <summary>
        /// 判断是否为decimal类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string str)
        {
            return StringHelper.IsDecimal(str);
        }

        public static bool IsNumber(this string str)
        {
            return StringHelper.IsNumber(str);
        }

        /// <summary>
        /// 判断是否为手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCellPhone(this string str)
        {
            return StringHelper.IsCellPhone(str);
        }
        /// <summary>
        /// 是否固定电话
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPhone(this string str)
        {
            return StringHelper.IsPhone(str);
        }
        /// <summary>
        /// 判断是否为邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(this string str)
        {
            return StringHelper.IsEmail(str);
        }
        /// <summary>
        /// 判断身份证号格式是否正确
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string str)
        {
            return StringHelper.IsIDCard(str);
        }
        #endregion


        #region object 判断扩展
        /// <summary>
        /// 是null或空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null ? true : StringHelper.IsNullOrEmpty(obj.ToString());
        }

        public static bool IsNotNullAndEmpty(this object obj)
        {
            return obj == null ? false : StringHelper.IsNotNullAndEmpty(obj.ToString());
        }
        public static bool IsInteger(this object obj)
        {
            return obj == null ? false : StringHelper.IsInteger(obj.ToString());
        }
        public static bool IsDecimal(this object obj)
        {
            return obj == null ? false : StringHelper.IsDecimal(obj.ToString());
        }


        public static bool IsNumber(this object obj)
        {
            return obj == null ? false : StringHelper.IsNumber(obj.ToString());
        }

        /// <summary>
        /// 对象转换为字符串，如果对象为null，则为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SConvertString(this object obj)
        {
            return StringHelper.SConvertString(obj);
        }

        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            var ds = new DataSet();
            var t = new DataTable();
            ds.Tables.Add(t);
            elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
            foreach (T item in list)
            {
                var row = t.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                t.Rows.Add(row);
            }
            return ds;
        }

        #endregion

        #region 转换扩展
        /// <summary>
        /// 转换为int32类型，若是非数值型字串，则变为默认值0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInteger(this object str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 转换为Double类型，若是非数值型字串，则变为默认值0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToNumber(this object str)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为Decimal类型，若是非数值型字串，则变为默认值0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object str)
        {
            
            try
            {
                return Convert.ToDecimal(str);
            }
            catch
            {
                return 0;
            }
         
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

        /// <summary>
        /// 得到大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string To32MD5(this string str)
        {
            return StringHelper.EncryptMD5(str);
        }
        /// <summary>
        /// 得到小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string To16MD5(this string str)
        {
            return StringHelper.ASP16MD5(str);
        }

        #endregion

        #region 拼音

        public static string ToChineseSpell(this string str)
        {
            return StringHelper.GetChineseSpell(str);
        }
        public static string ToChineseIndex(this string str)
        {
            return StringHelper.GetChineseIndex(str);
        }
        #endregion

    }
}
