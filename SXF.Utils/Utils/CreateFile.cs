/*###########################################################
 * 功能：生成静态文件类
 * by:sxf
 * date:2013-06-18
############################################################# */

using System;
using System.IO;
using System.Web;

namespace SXF.Utils
{
    public class CreateFile
    {
        /// 生成静态文件
        /// </summary>
        /// <param name="OldFilePath">旧文件路径</param>
        /// <param name="NewFilePath">新文件路径</param>
        public void CreateHtml(string OldFilePath, string NewFilePath)
        {


            string newPageHtml = "";
            //虚拟路径
            string PageUrl = "";
            string ServerName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();

            if (ServerName == "localhost")
            {
                newPageHtml = "/" + NewFilePath;
                PageUrl = System.Configuration.ConfigurationManager.AppSettings["LocalDomain"].ToString() + OldFilePath;
            }
            else
            {
                newPageHtml = "/" + NewFilePath;
                PageUrl = System.Configuration.ConfigurationManager.AppSettings["WebSiteDomain"].ToString() + OldFilePath;
            }



            //打开文本流
            System.Net.WebClient wc = new System.Net.WebClient();
            //retrieve or set CredentialCache.
            wc.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream resStream = null;
            try
            {
                EventLog.WriteLog(PageUrl);
                //可能出现异常的代码，可以是多行
                resStream = wc.OpenRead(PageUrl);
            }
            catch (Exception e)
            {
                //捕获异常后的处理代码 
                EventLog.WriteLog(e.Source + "\n" + e.Message + "\n" + e.StackTrace);
                return;

            }

            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.GetEncoding("UTF-8"));
            //要生成的静态页面路径  
            string physicalPath = HttpContext.Current.Server.MapPath(newPageHtml);
            //如果文件不存在，则创建
            if (!File.Exists(physicalPath))
            {
                FileStream objfile = File.Create(physicalPath);
                objfile.Close();
            }

            StreamWriter sw = new StreamWriter(physicalPath, false, System.Text.Encoding.GetEncoding("UTF-8"));

            string tem = sr.ReadToEnd();

            try
            {
                sw.Write(tem);
                EventLog.WriteLog("生成静态文件成功！");
            }
            catch
            {
                EventLog.WriteLog("生成静态文件失败！");

            }
            finally
            {
                sw.Close();
                sr.Close();
                if (resStream != null)
                {
                    resStream.Close();
                }

            }



        }



        /// <summary>
        /// 从文件名中截取路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPathFromName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            return fileName.Substring(0, fileName.LastIndexOf("/") + 1);
        }
    }
}
