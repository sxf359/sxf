using System;
using System.Web.UI;
using System.Web;

namespace SXF.Utils
{
    public static class JSHelper
    {

        /// <summary>
        /// 在页面开始输出js脚本
        /// </summary>
        /// <param name="script">脚本内容</param>
        public static void WriteScript(string script)
        {
            HttpContext.Current.Response.Write(string.Format("<script>{0}</script>", script));
        }
        /// <summary>
        /// 在页面开始输出文本
        /// </summary>
        /// <param name="content">文本内容</param>
        public static void Write(string content)
        {
            HttpContext.Current.Response.Write(content);
        }
        /// <summary>
        /// 在页面开始输出js提示框
        /// </summary>
        /// <param name="msg">提示内容</param>
        public static void WriteAlert(string msg)
        {
            Write(string.Format("<script type=\"text/javascript\">alert(\"{0}\")</script>", msg));
        }
        /// <summary>
        /// 在页面开始输出js跳转
        /// </summary>
        /// <param name="url">跳转地址</param>
        public static void WriteGoTo(string url)
        {
            WriteScript("location.href=\"" + url + "\"");
        }
        /// <summary>
        /// 在页面开始输出js提示框和js跳转
        /// </summary>
        /// <param name="msg">提示内容</param>
        /// <param name="url">跳转地址</param>
        public static void WriteAlertAandGoTo(string msg, string url)
        {
            WriteScript(string.Format("alert(\"{0}\");location.href=\"{1}\"", msg, url));
        }

        /// <summary>
        /// 向页面中注册js脚本
        /// </summary>
        /// <param name="js">脚本内容</param>
        public static void RegisterScript(string js)
        {
            Page page = PageHelper.GetPage();
            string jsStr = string.Format("<script type=\"text/javascript\">{0}</script>", js);
           // SinoHelper2.EventLog.WriteLog(jsStr);
            if (page != null)
            {
                //SinoHelper2.EventLog.WriteLog(typeof(Page).ToString());
                //SinoHelper2.EventLog.WriteLog(Guid.NewGuid().ToString());
                page.ClientScript.RegisterStartupScript(typeof(Page), Guid.NewGuid().ToString(), jsStr);
            }
            else
            {
                //SinoHelper2.EventLog.WriteLog("<br>gefd");
                HttpContext.Current.Response.Write(jsStr);
            }
        }

        /// <summary>
        /// 以注册脚本形式弹出提示框
        /// </summary>
        /// <param name="msg">信息</param>
        public static void Alert(string msg)
        {
            //防止警告信息字符串中由于有双引号字符而导致的错误
            msg = msg.Replace("\"", "\\\"");
            RegisterScript(string.Format("alert(\"{0}\")", msg));
        }
        /// <summary>
        /// 以注册脚本形式跳转页面
        /// </summary>
        /// <param name="url">目标页面地址</param>
        public static void GoTo(string url)
        {
            RegisterScript(string.Format("location.href=\"{0}\"", url));
        }
        /// <summary>
        /// 以注册脚本形式弹出提示框并跳转页面
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="url">目标页面地址</param>
        public static void AlertAndGoTo(string msg, string url)
        {
            //防止警告信息字符串中由于有双引号字符而导致的错误
            msg = msg.Replace("\"", "\\\"");
            
            RegisterScript(string.Format("alert(\"{0}\");location.href=\"{1}\";", msg, url));
        }
    }
}
