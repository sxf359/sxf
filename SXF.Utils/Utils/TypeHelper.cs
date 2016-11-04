namespace SXF.Utils
{
    using System;
    /// <summary>
    /// 类型辅助类
    /// </summary>
    public class TypeHelper
    {
        /// <summary>
        /// 调整对象类型
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static object ChangeType(Type targetType, object val)
        {
            //如果值为空，则返回null
            if (val == null)
            {
                return null;
            }

            //如果model类型与值类型一致，则返回数据对象值
            if (targetType == val.GetType())
            {
                return val;
            }
            //如果model实例为可为空的datetime类型,返回获取的值
            if (targetType == typeof(DateTime?) && val.GetType() == typeof(DateTime))
            {
                return val;
            }
            //如果model实例为可为空的int类型，返回获取的值
            if (targetType == typeof(int?) && val.GetType() == typeof(int))
            {
                return val;
            }
            
            //如果model为bool型
            if (targetType == typeof(bool))
            {
                if (val.ToString() == "0")
                {
                    return false;
                }
                if (val.ToString() == "1")
                {
                    return true;
                }
            }
            //如果model为枚举类型
            if (targetType.IsEnum)
            {
                int result = 0;
                if (!int.TryParse(val.ToString(), out result))
                {
                    return Enum.Parse(targetType, val.ToString());
                }
                return val;
            }
            //如果model为system.Type类型
            if (targetType == typeof(Type))
            {
                return ReflectionHelper.GetType(val.ToString());
            }
            return Convert.ChangeType(val, targetType);
        }

        public static string GetClassSimpleName(Type t)
        {
            string[] strArray = t.ToString().Split(new char[] { '.' });
            return strArray[strArray.Length - 1].ToString();
        }

        public static string GetDefaultValue(Type destType)
        {
            if (IsNumbericType(destType))
            {
                return "0";
            }
            if (destType == typeof(string))
            {
                return "\"\"";
            }
            if (destType == typeof(bool))
            {
                return "false";
            }
            if (destType == typeof(DateTime))
            {
                return "DateTime.Now";
            }
            if (destType == typeof(Guid))
            {
                return "System.Guid.NewGuid()";
            }
            if (destType == typeof(TimeSpan))
            {
                return "System.TimeSpan.Zero";
            }
            return "null";
        }

        public static Type GetTypeByRegularName(string regularName)
        {
            return ReflectionHelper.GetType(regularName);
        }

        public static string GetTypeRegularName(Type destType)
        {
            string str = destType.Assembly.FullName.Split(new char[] { ',' })[0];
            return string.Format("{0},{1}", destType.ToString(), str);
        }

        public static string GetTypeRegularNameOf(object obj)
        {
            return GetTypeRegularName(obj.GetType());
        }

        public static bool IsFixLength(Type destDataType)
        {
            return (IsNumbericType(destDataType) || ((destDataType == typeof(byte[])) || ((destDataType == typeof(DateTime)) || (destDataType == typeof(bool)))));
        }

        public static bool IsNumbericType(Type destDataType)
        {
            return ((((((destDataType == typeof(int)) || (destDataType == typeof(uint))) || ((destDataType == typeof(double)) || (destDataType == typeof(short)))) || (((destDataType == typeof(ushort)) || (destDataType == typeof(decimal))) || ((destDataType == typeof(long)) || (destDataType == typeof(ulong))))) || ((destDataType == typeof(float)) || (destDataType == typeof(byte)))) || (destDataType == typeof(sbyte)));
        }

        public static bool IsSimpleType(Type t)
        {
            return (IsNumbericType(t) || ((t == typeof(char)) || ((t == typeof(string)) || ((t == typeof(bool)) || ((t == typeof(DateTime)) || ((t == typeof(Type)) || t.IsEnum))))));
        }
    }
}

