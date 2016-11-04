using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SXF.Utils;

namespace SXF.Kernel
{
    public class Mapping : BaseDataProvider
    {
        private static Dictionary<string, string> _identity = new Dictionary<string, string>();
        private static Dictionary<string, object> _defaultValue;
        private static DbHelper _db;

        static Mapping()
        {


        }
        public Mapping(string connstring)
            : base(connstring)
        {
            _db = base.Database;
        }
        public static void Init()
        {
            //初始化自增字段
            InitIdentity();
        }
        private static void InitIdentity()
        {

            IDataReader reader = _db.ExecuteReader(Sql.GET_IDENTITY);
            while (reader.Read())
            {
                if (_identity.ContainsKey(reader["Table_name"].ToString()))
                    _identity[reader["Table_name"].ToString()] = reader["Identity_Column_name"].ToString();
                else
                    _identity.Add(reader["Table_name"].ToString(), reader["Identity_Column_name"].ToString());
            }
            reader.Close();
        }
        /// <summary>
        /// 判断 DataReader 里面是否包含指定的列
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static bool ReaderExists(IDataReader dr, string columnName)
        {
            int count = dr.FieldCount;
            for (int i = 0; i < count; i++)
            {
                if (dr.GetName(i).Equals(columnName))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断字段是否是自增列
        /// </summary>
        /// <param name="field"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool FieldIsIndentity(string field, string tableName)
        {
            if (_identity.ContainsKey(tableName))
            {
                if (_identity[tableName] == field)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 判断字段是否是自增列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool FieldIsIndentity(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(typeof(IdentityAttribute), false);
            if (atts.Length > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取一个实体的自增列名称
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns></returns>
        public static string GetIdentity(object obj)
        {
            string identity = "";
            PropertyInfo[] infos = obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (FieldIsIndentity(info))
                {
                    identity = info.Name;
                    break;
                }
            }
            if (identity == "")
            {
                Init();
            }
            return identity;
        }

        /// <summary>
        /// 获取实体自增列的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetIdentityValue(object obj)
        {
            int identityValue = 0;
            PropertyInfo[] infos = obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (FieldIsIndentity(info))
                {
                    identityValue = info.GetValue(obj, null).ToInteger();
                    break;
                }
            }

            return identityValue;
        }

        /// <summary>
        /// 获取一个泛类的自增列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetIdentity<T>()
        {
            string identity = "";
            Type type = typeof(T);
            PropertyInfo[] infos = type.GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (FieldIsIndentity(info))
                {
                    identity = info.Name;
                    break;
                }
            }
            if (identity == "")
            {
                Init();
                return _identity[type.Name];
            }
            return identity;
        }
        /// <summary>
        /// 判断一列是否为唯一性属性
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool FieldIsUnique(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(typeof(UniqueAttribute), false);
            if (atts.Length > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取一个属性的特性
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static PropertyAttribute GetProperty(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(typeof(PropertyAttribute), false);
            foreach (object obj in atts)
            {
                return (PropertyAttribute)obj;
            }
            return new PropertyAttribute(PropertyType.Property, info.Name);
        }
        /// <summary>
        /// 获取一个属性的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static T GetProperty<T>(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(typeof(T), false);
            foreach (object obj in atts)
            {
                return (T)obj;
            }
            return default(T);
        }
        /// <summary>
        /// 获取一个对象的特性
        /// </summary>
        /// <param name="info">属性</param>
        /// <returns></returns>
        public static PropertyType GetPrimaryKeyProperty(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(typeof(PropertyAttribute), false);
            foreach (object obj in atts)
            {
                return ((PropertyAttribute)obj).PropertyType;
            }
            return PropertyType.Property;
        }
        /// <summary>
        /// 获取一个实体类型的所有外键实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetForeignKeyType<T>()
        {
            Type type = typeof(T);
            PropertyInfo[] infos = type.GetProperties();
            List<string> list = new List<string>();
            foreach (PropertyInfo info in infos)
            {
                PropertyAttribute property = GetProperty(info);
                if (property.PropertyType == PropertyType.ForeignKey)
                    list.Add(info.Name);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获取一个实体对象的主键
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string[] GetPrimaryKey(object obj)
        {
            IList<string> list = new List<string>();
            PropertyInfo[] infos = obj.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {
                if (GetPrimaryKeyProperty(info) == PropertyType.PrimaryKey)
                    list.Add(info.Name);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获取一个类型的默认值
        /// </summary>
        /// <param name="targetType">类型</param>
        /// <returns>返回该类型的默认值</returns>
        public static object DefaultForType(Type targetType)
        {
            object objValue = null;
            string typeName = targetType.Name;
            if (_defaultValue == null)
            {
                _defaultValue = new Dictionary<string, object>();
            }
            if (!_defaultValue.ContainsKey(typeName))
            {
                if (targetType.IsValueType)
                    objValue = Activator.CreateInstance(targetType);
                _defaultValue.Add(typeName, objValue);
            }
            return _defaultValue[typeName];
        }
        /// <summary>
        /// 从数据集读取数据赋值到实体对象中
        /// </summary>
        /// <param name="t">要赋值的实体对象</param>
        /// <param name="idr">数据集</param>
        public static void ToObject(Object t, DataTable tb)
        {
            ToObject(t, tb, false);
        }
        /// <summary>
        /// 从数据集读取数据赋值到实体对象中
        /// </summary>
        /// <param name="t">要赋值的实体对象</param>
        /// <param name="tb">数据集</param>
        /// <param name="IsLoad">是否加载包含的引用类型</param>
        public static void ToObject(Object t, DataTable tb, bool IsLoad)
        {
            PropertyInfo[] infos = t.GetType().GetProperties();
            int count = infos.Length;

            if (tb.Rows.Count > 0)
            {
                DataRow idr = tb.Rows[0];
                foreach (PropertyInfo info in infos)
                {
                    Type type = info.PropertyType;

                    if (type.GetInterface("System.Collections.ICollection") != null)
                        continue;

                    if (type.IsClass && type != Type.GetType("System.String"))
                    {
                        if (IsLoad)
                        {
                            object obj = Activator.CreateInstance(type);
                            PropertyInfo[] classInfos = type.GetProperties();
                            foreach (PropertyInfo classinfo in classInfos)
                            {
                                Type classType = classinfo.PropertyType;

                                if (classType.GetInterface("System.Collections.ICollection") != null)
                                    continue;

                                if (idr[classinfo.Name] is DBNull || idr[classinfo.Name] == null)
                                    continue;
                                if (tb.Columns.Contains(classinfo.Name))
                                {
                                    classinfo.SetValue(obj, idr[classinfo.Name], null);
                                }
                            }
                            if (obj != null)
                                info.SetValue(t, obj, null);
                        }
                    }
                    else
                    {
                        if (idr[info.Name] is DBNull || idr[info.Name] == null)
                            continue;
                        if (tb.Columns.Contains(info.Name))
                            info.SetValue(t, idr[info.Name], null);
                    }
                }
            }
        }
        /// <summary>
        /// 加载实体并返回指定包含的引用实体
        /// </summary>
        /// <typeparam name="T">要返回的引用的实体类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <param name="tb">数据集</param>
        /// <returns>返回指定包含的引用实体</returns>
        public static void ToObject(object t, DataTable tb, params string[] objs)
        {
            PropertyInfo[] infos = t.GetType().GetProperties();
            int count = infos.Length;

            if (tb.Rows.Count > 0)
            {
                DataRow idr = tb.Rows[0];
                foreach (PropertyInfo info in infos)
                {
                    Type type = info.PropertyType;

                    if (type.GetInterface("System.Collections.ICollection") != null)
                        continue;

                    if (type.IsClass && type != Type.GetType("System.String"))
                    {
                        if (objs.Contains(info.Name))
                        {
                            object obj = Activator.CreateInstance(type);
                            PropertyInfo[] classInfos = type.GetProperties();
                            foreach (PropertyInfo classinfo in classInfos)
                            {
                                Type classType = classinfo.PropertyType;
                                if (classType.GetInterface("System.Collections.ICollection") != null)
                                    continue;

                                if (classType.IsClass && classType != Type.GetType("System.String"))
                                {
                                    continue;
                                }
                                if (idr[classinfo.Name] is DBNull || idr[classinfo.Name] == null)
                                    continue;
                                if (tb.Columns.Contains(classinfo.Name))
                                {
                                    classinfo.SetValue(obj, idr[classinfo.Name], null);
                                }
                            }
                            if (obj != null)
                                info.SetValue(t, obj, null);
                        }
                    }
                    else
                    {
                        if (idr[info.Name] is DBNull || idr[info.Name] == null)
                            continue;
                        if (tb.Columns.Contains(info.Name))
                            info.SetValue(t, idr[info.Name], null);
                    }
                }
            }
        }
        /// <summary>
        /// 加载实体集
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="tb">数据集</param>
        /// <param name="IsLoad">是否加载关联实体</param>
        /// <returns>返回实体集</returns>
        public static List<T> ToList<T>(DataTable tb, bool IsLoad) where T : new()
        {
            List<T> list = new List<T>();
            string typeName = typeof(T).Name;
            int recordCount = tb.Rows.Count;
            for (int i = 0; i < recordCount; i++)
            {
                DataRow row = tb.Rows[i];
                T t = new T();
                PropertyInfo[] infos = typeof(T).GetProperties();
                int count = infos.Length;
                foreach (PropertyInfo info in infos)
                {
                    Type type = info.PropertyType;

                    if (type.IsClass && type != Type.GetType("System.String"))
                    {
                        if (IsLoad)
                        {
                            PropertyAttribute property = GetProperty(info);
                            object obj = Activator.CreateInstance(type);
                            PropertyInfo[] classInfos = type.GetProperties();
                            foreach (PropertyInfo classinfo in classInfos)
                            {
                                if (row.Table.Columns.Contains(classinfo.Name))
                                {
                                    if (row[classinfo.Name] is DBNull)
                                        continue;
                                    classinfo.SetValue(obj, row[classinfo.Name], null);
                                }
                            }
                            if (obj != null)
                                info.SetValue(t, obj, null);
                        }
                    }
                    else
                    {
                        if (row.Table.Columns.Contains(info.Name))
                        {
                            if (row[info.Name] is DBNull)
                                continue;
                            info.SetValue(t, row[info.Name], null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        public static List<T> ToList<T>(DataTable tb, params string[] objs) where T : new()
        {
            List<T> list = new List<T>();
            string typeName = typeof(T).Name;
            int recordCount = tb.Rows.Count;
            for (int i = 0; i < recordCount; i++)
            {
                DataRow row = tb.Rows[i];
                T t = new T();
                PropertyInfo[] infos = typeof(T).GetProperties();
                int count = infos.Length;
                string subColumnName = "";
                string columnName = "";
                foreach (PropertyInfo info in infos)
                {
                    Type type = info.PropertyType;

                    if (type.IsClass && type != Type.GetType("System.String"))
                    {
                        if (objs.Contains(info.Name))
                        {
                            PropertyAttribute property = GetProperty(info);
                            object obj = Activator.CreateInstance(type);
                            PropertyInfo[] classInfos = type.GetProperties();
                            foreach (PropertyInfo classinfo in classInfos)
                            {
                                subColumnName = info.Name + "_" + classinfo.Name;
                                if (row.Table.Columns.Contains(subColumnName))
                                {
                                    if (row[subColumnName] is DBNull)
                                        continue;
                                    classinfo.SetValue(obj, row[subColumnName], null);
                                }
                            }
                            if (obj != null)
                                info.SetValue(t, obj, null);

                        }
                    }
                    else
                    {
                        columnName = info.Name;
                        if (row.Table.Columns.Contains(columnName))
                        {
                            if (row[columnName] is DBNull)
                                continue;
                            info.SetValue(t, row[columnName], null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }
        public static List<T> ToList<T>(DataTable tb, Dictionary<string, string> dict) where T : new()
        {
            List<T> list = new List<T>();
            string typeName = typeof(T).Name;
            int recordCount = tb.Rows.Count;
            for (int i = 0; i < recordCount; i++)
            {
                DataRow row = tb.Rows[i];
                T t = new T();
                PropertyInfo[] infos = typeof(T).GetProperties();
                int count = infos.Length;
                int m = 1;
                string s = "";
                string subColumnName = "";
                foreach (PropertyInfo info in infos)
                {
                    Type type = info.PropertyType;
                    if (type.IsClass && type != Type.GetType("System.String"))
                    {
                        if (dict.ContainsKey(info.Name))
                        {
                            PropertyAttribute property = GetProperty(info);
                            object obj = Activator.CreateInstance(type);
                            PropertyInfo[] classInfos = type.GetProperties();
                            foreach (PropertyInfo classinfo in classInfos)
                            {
                                if (m == 0)
                                {
                                    s = "";
                                }
                                else
                                {
                                    s = m.ToString();
                                }
                                string[] nums = dict[info.Name].Split(new char[] { '.' });
                                subColumnName = classinfo.Name + nums[1];
                                if (row.Table.Columns.Contains(subColumnName))
                                {
                                    if (row[subColumnName] is DBNull)
                                        continue;
                                    classinfo.SetValue(obj, row[subColumnName], null);
                                }
                            }
                            if (obj != null)
                                info.SetValue(t, obj, null);
                            m++;

                        }
                    }
                    else
                    {
                        if (row.Table.Columns.Contains(info.Name))
                        {
                            if (row[info.Name] is DBNull)
                                continue;
                            info.SetValue(t, row[info.Name], null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }
        /// <summary>
        /// 加载实体集（不包含关联实体）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="tb">数据集</param>
        /// <returns>返回实体集</returns>
        public static List<T> ToList<T>(DataTable tb) where T : new()
        {
            return ToList<T>(tb, false);
        }
    }
}
