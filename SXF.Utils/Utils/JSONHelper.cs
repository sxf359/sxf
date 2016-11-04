using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;
using System.Text.RegularExpressions;

namespace SXF.Utils
{
    /// <summary> 
    /// JSON帮助类 
    /// </summary> 
    public class JSONHelper
    {
        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                string str = jss.Serialize(obj);
                str = Regex.Replace(str, @"\\/Date\((\-{0,1}\d+)\)\\/", match =>
                {
                    //DateTime dt = new DateTime(1970, 1, 1);
                    //dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    //dt = dt.ToLocalTime();
                    //return dt.ToString("yyyy-MM-dd HH:mm:ss");
                    return match.Groups[1].Value.ToString();
                });
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
            }
        }
        /// <summary> 
        /// 数据表转键值对集合 
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                list.Add(dic);
            }
            return list;
        }

        /// <summary>
        /// 数据表转键值对集合，只转第一行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DataTableToDictionary(DataTable dt)
        {
            DataRow dr = dt.Rows[0];
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (DataColumn dc in dt.Columns)
            {
                dic.Add(dc.ColumnName, dr[dc.ColumnName]);
            }
            return dic;
        }

        /// <summary> 
        /// 数据集转键值对数组字典 
        /// </summary> 
        /// <param name="dataSet">数据集</param> 
        /// <returns>键值对数组字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            Dictionary<string, List<Dictionary<string, object>>> result = new Dictionary<string, List<Dictionary<string, object>>>();
            foreach (DataTable dt in ds.Tables)
                result.Add(dt.TableName, DataTableToList(dt));
            return result;
        }
        /// <summary> 
        /// 数据表转JSON，datatable最多返回一条数据
        /// </summary> 
        /// <param name="dataTable">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTableToJSONByOne(DataTable dt)
        {

            return ObjectToJSON(DataTableToDictionary(dt));

        }

        /// <summary>
        /// 数据表转json,datatable会返回多条数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>字符串</returns>
        public static string DataTableToJSON(DataTable dt)
        {

            return ObjectToJSON(DataTableToList(dt));

        }
        /// <summary> 
        /// JSON文本转对象,泛型方法 
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JSONToObject<T>(string jsonText)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
            }
        }
        /// <summary> 
        /// 将JSON文本转换为数据表数据 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据表字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }
        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns> 
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, object>>(jsonText);
        }
    }
}
