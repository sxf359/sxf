using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.IO;
using System.Data;
using System.Reflection;
using System.Data.OleDb;

namespace SXF.Utils
{
    /// <summary>
    /// 导出操作类
    /// </summary>
    public class ExportHelper
    {

        private static string getTimeName()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }

        #region 控件导出Word
        /// <summary>
        /// 将Control里呈现的内容导出到Word文档
        /// </summary>
        /// <param name="control">要导出内容的控件</param>
        public static void ToWord(Control control)
        {
            HttpContext curContext = System.Web.HttpContext.Current;
            StringWriter strWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(strWriter);
            curContext.Response.ClearContent();
            curContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.doc", getTimeName()));
            curContext.Response.ContentType = "application/vnd.ms-word";
            curContext.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
            curContext.Response.Charset = "GB2312";
            control.RenderControl(htmlWriter);
            curContext.Response.Write(strWriter.ToString());
            curContext.Response.End();
        }
        #endregion

        #region 控件导出Excel
        /// <summary>
        /// 将Control里呈现的内容导出到Excel表格
        /// </summary>
        /// <param name="control">要导出内容的控件</param>
        public static void ToExcel(Control control)
        {
            ToExcel(control, getTimeName());
        }
        #endregion

        #region 控件导出Excel
        /// <summary>
        /// 将Control里呈现的内容导出到Excel表格
        /// </summary>
        /// <param name="control">要导出内容的控件</param>
        /// <param name="fileName">文件名</param>
        public static void ToExcel(Control control, string fileName)
        {
            if (fileName.IsNullOrEmpty())
            {
                fileName = getTimeName();
            }
            fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
            HttpContext curContext = System.Web.HttpContext.Current;
            StringWriter strWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(strWriter);
            curContext.Response.ClearContent();
            curContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.xls", fileName));
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
            curContext.Response.Charset = "GB2312";
            control.RenderControl(htmlWriter);
            curContext.Response.Write(strWriter.ToString());
            curContext.Response.End();
        }
        #endregion


        #region DataTable导出Excel
        /// <summary>
        /// 将DataTable里的数据呈现到Excel
        /// </summary>
        /// <param name="dt"></param>
        public static void ToExcel(DataTable dt)
        {
            ToExcel(dt, getTimeName());
        }

        /// <summary>
        /// 重载,将DataTable里的数据呈现到Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName">文件名</param>
        public static void ToExcel(DataTable dt, string fileName)
        {
            if (fileName.IsNullOrEmpty())
            {
                fileName = getTimeName();
            }
            fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
            string result = string.Empty;
            int readCnt = dt.Rows.Count;
            int colCount = dt.Columns.Count;
            int pagerecords = 5000;
            string strTitleRow = "";
            for (int j = 0; j < colCount; j++)
            {
                strTitleRow += dt.Columns[j].ColumnName + "\t";
            }
            strTitleRow += "\r\n";

            StringBuilder strRows = new StringBuilder();
            int cnt = 1;
            for (int i = 0; i < readCnt; i++)
            {
                //strRows.Append("");
                for (int j = 0; j < colCount; j++)
                {
                    if (dt.Columns[j].DataType.Name == "DateTime" || dt.Columns[j].DataType.Name == "SmallDateTime")
                    {
                        if (dt.Rows[i][j].ToString() != string.Empty)
                        {
                            strRows.Append(Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy年MM月dd日hh时mm分ss秒") + "\t");
                        }
                        else
                            strRows.Append("\t");
                    }
                    else
                    {
                        strRows.Append(dt.Rows[i][j].ToString().Trim() + "\t");
                    }
                }
                strRows.Append("\r\n");
                cnt++;
                if (cnt >= pagerecords)
                {
                    result += strRows.ToString();
                    strRows.Remove(0, strRows.Length);
                    cnt = 1;
                }
            }
            result = strTitleRow + result + strRows.ToString();
            HttpContext curContext = System.Web.HttpContext.Current;
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            curContext.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
            curContext.Response.Charset = "GB2312";
            curContext.Response.Write(result);
            curContext.Response.Flush();
            curContext.Response.End();
        }

        /// <summary>
        /// 将DataTable里的数据呈现到Word
        /// </summary>
        /// <param name="dt">DataTable</param>
        public static void ToWord(DataTable dt)
        {

            string result = string.Empty;
            int readCnt = dt.Rows.Count;
            int colCount = dt.Columns.Count;
            int pagerecords = 5000;
            string strTitleRow = "";
            for (int j = 0; j < colCount; j++)
            {
                strTitleRow += dt.Columns[j].ColumnName + "\t";
            }
            strTitleRow += "\r\n";

            StringBuilder strRows = new StringBuilder();
            int cnt = 1;
            for (int i = 0; i < readCnt; i++)
            {
                //strRows.Append("");
                for (int j = 0; j < colCount; j++)
                {
                    if (dt.Columns[j].DataType.Name == "DateTime" || dt.Columns[j].DataType.Name == "SmallDateTime")
                    {
                        if (dt.Rows[i][j].ToString() != string.Empty)
                        {
                            strRows.Append(Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy年MM月dd日hh时mm分ss秒") + "\t");
                        }
                        else
                            strRows.Append("\t");
                    }
                    else
                    {
                        strRows.Append(dt.Rows[i][j].ToString().Trim() + "\t");
                    }
                }
                strRows.Append("\r\n");
                cnt++;
                if (cnt >= pagerecords)
                {
                    result += strRows.ToString();
                    strRows.Remove(0, strRows.Length);
                    cnt = 1;
                }
            }
            result = strTitleRow + result + strRows.ToString();
            HttpContext curContext = System.Web.HttpContext.Current;
            curContext.Response.ContentType = "application/vnd.ms-word";
            curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            curContext.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.doc", getTimeName()));
            curContext.Response.Charset = "GB2312";
            curContext.Response.Write(result);
            curContext.Response.Flush();
            curContext.Response.End();
        }
        #endregion

        #region Excel数据操作

        /// <summary>
        /// 得到Excel文件的数据
        /// </summary>
        /// <param name="SavePath">文件路径</param>
        /// <returns></returns>
        public static DataTable GetExcelData(string SavePath)
        {
            DataSet myDs = new DataSet();
            if (string.IsNullOrEmpty(SavePath))
            {
                throw new Exception("excel路径不能为空");
            }
            try
            {
                OleDbConnection cnnxls = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + SavePath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'");
                //new OleDbDataAdapter("select * from [Sheet1$]", cnnxls).Fill(myDs);
                new OleDbDataAdapter("select * from [newhd$]", cnnxls).Fill(myDs);
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
            return myDs.Tables[0];
        }

        /// <summary>
        /// 得到Excel文件的数据
        /// </summary>
        /// <param name="file">Excel文件路径</param>
        /// <param name="sheet">Excel工作表名</param>
        /// <param name="cols">工作表第一行的字段(全部为*)</param>
        /// <returns></returns>
        public static DataTable GetExcelData(string file, string sheet, string cols)
        {
            DataSet myDs = new DataSet();
            if (string.IsNullOrEmpty(file))
            {
                throw new Exception("excel路径不能为空");
            }
            try
            {
                OleDbConnection cnnxls = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'");
                new OleDbDataAdapter("select {0} from [{1}$]".FormatWith(cols, sheet), cnnxls).Fill(myDs);
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
            return myDs.Tables[0];
        }
        /// <summary>
        ///  得到Excel文件的工作表名(第三列为表名)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DataTable GetExcelTableName(string file)
        {
            DataTable dt = null;
            if (File.Exists(file))
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + file + ";Extended Properties='Excel 8.0;HDR=False;IMEX=1'"))
                {
                    conn.Open();
                    dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    conn.Close();
                }
            }
            return dt;
        }


        /// <summary>
        /// 得到Excel文件的数据
        /// </summary>
        /// <param name="file">Excel文件路径</param>
        /// <param name="sheet">Excel工作表名</param>
        /// <returns></returns>
        public static DataTable GetExcelData(string file, string sheet)
        {
            return GetExcelData(file, sheet, "*");
        }
        /// <summary>
        /// Excel数据导入数据库
        /// </summary>
        /// <param name="file">Excel文件</param>
        /// <param name="sheet">Excel工作表名</param>
        /// <param name="cols">工作表第一行的字段(全部为*)</param>
        /// <param name="table">数据库表名</param>
        /// <param name="fields">数据库表字段</param>
        /// <param name="staticFields">特定的列值，格式为：name:'小明',age:23，如果没有则为string.Empty</param>
        /// <returns></returns>
        public static string ExcelToSql(string file, string sheet, string cols, string table, string fields, string staticFields)
        {
            DataTable dt = GetExcelData(file, sheet, cols);
            if (dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string sfields = string.Empty;
                string svalues = string.Empty;
                if (staticFields.IsNotNullAndEmpty())
                {
                    string[] parts = staticFields.Split(',');
                    foreach (string part in parts)
                    {
                        string[] kv = part.Split(':');
                        sfields += "," + kv[0];
                        svalues += "," + kv[1];
                    }
                }
                foreach (DataRow dr in dt.Rows)
                {
                    sb.AppendFormat("insert into {0} ({1}{2}) values ", table, fields, sfields);
                    sb.Append("(");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i == dt.Columns.Count - 1)
                        {
                            sb.AppendFormat("'{0}'", dr[i]);
                        }
                        else
                        {
                            sb.AppendFormat("'{0}',", dr[i]);
                        }
                    }
                    sb.AppendFormat("{0});", svalues);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Excel数据导入数据库
        /// </summary>
        /// <param name="file">Excel文件</param>
        /// <param name="sheet">Excel工作表名</param>
        /// <param name="table">数据库表名</param>
        /// <param name="fields">数据库表字段</param>
        /// <param name="staticFields">特定的列值，格式为：name:'小明',age:23，如果没有则为string.Empty</param>
        /// <returns></returns>
        public static string ExcelToSql(string file, string sheet, string table, string fields, string staticFields)
        {
            return ExcelToSql(file, sheet, "*", table, fields, staticFields);
        }

        #endregion

        #region TXT数据操作

        /// <summary>
        /// Txt导出Datatable
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>datatable</returns>
        public static DataTable GetTxtData(string path)
        {
            Encoding code = Encoding.GetEncoding("gb2312");//编码格式
            StreamReader reader = new StreamReader(path, code);
            DataTable dt = new DataTable();
            DataColumn dc1 = dt.Columns.Add("mobile", Type.GetType("System.String"));
            DataColumn dc2 = dt.Columns.Add("name", Type.GetType("System.String"));
            while (reader.Peek() >= 0)
            {
                string s = reader.ReadLine();
                if (s.IndexOf(',') != -1)
                {
                    string[] str = s.Split(new char[] { ',' });
                    DataRow dr = dt.NewRow();
                    for (int x = 0; x < 2; x++)
                    {
                        dr[x] = str[x];
                    }
                    dt.Rows.Add(dr);
                }
            }
            reader.Dispose();
            reader.Close();
            return dt;
        }

        /// <summary>
        /// Txt导出Datatable
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>datatable</returns>
        public static DataTable GetTxtDataAll(string path)
        {
            return GetTxtDataAll(path, Encoding.GetEncoding("gb2312"), ',');
        }

        /// <summary>
        /// Txt导出Datatable
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>datatable</returns>
        public static DataTable GetTxtDataAll(string path, Encoding en, char c)
        {
            Encoding code = en;//编码格式
            StreamReader reader = new StreamReader(path, code);
            DataTable dt = new DataTable();
            bool flag = true;
            while (reader.Peek() >= 0)
            {
                string s = reader.ReadLine();
                if (s.IndexOf(c) != -1)
                {
                    string[] str = s.Split(new char[] { c });
                    DataRow dr = dt.NewRow();
                    for (int x = 0; x < str.Length; x++)
                    {
                        if (flag)
                        {
                            DataColumn dc = dt.Columns.Add();
                        }
                        dr[x] = str[x];
                    }
                    dt.Rows.Add(dr);
                    flag = false;
                }
            }
            reader.Dispose();
            reader.Close();
            return dt;
        }


        #endregion
    }
}
