using System;
using System.Text;
using System.Web;

namespace SXF.Utils
{
    public class ExceptionHelper
    {

        public static Exception GetInnerException(Exception exp)
        {
            if (exp.InnerException != null)
            {
                exp = exp.InnerException;
                return GetInnerException(exp);
            }
            return exp;
        }
        public static void WriteException()
        {
            WriteException(HttpContext.Current.Server.GetLastError(), "");
        }
        static long exceptionId = 0;
        static object lockObj = new object();
        /// <summary>
        /// 错误输出到页面
        /// </summary>
        /// <param name="ero"></param>
        /// <param name="errorCode"></param>
        static void WriteException(Exception ero, string errorCode)
        {
            //return;    //网上不再做特殊处理，直接显示错误信息
            string address = RequestHelper.GetServerIp();
            //本地不作处理
            if (address.Contains("192.168."))
            {
                return;
            }
            string html = Properties.Resources.erroHtml;
            HttpContext context = HttpContext.Current;
            ero = GetInnerException(ero);
            if (ero != null)
            {
                string erroDetail = ero.StackTrace;
                erroDetail = erroDetail.Replace("\r\n", "<br>");
                erroDetail = HttpContext.Current.Server.HtmlEncode(erroDetail);
                html = html.Replace("[TIME]", DateTime.Now.ToString());
                html = html.Replace("[ERRO_CODE]", errorCode);
                html = html.Replace("[URL]", HttpContext.Current.Request.Url.ToString());
                html = html.Replace("[ERRO_TITLE]", HttpContext.Current.Server.HtmlEncode(ero.Message));
                html = html.Replace("[ERRO_MESSAGE]", erroDetail);
                context.Response.Write(html);
                context.Response.End();
            }
        }

        /// <summary>
        /// 内部记录日志
        /// </summary>
        /// <param name="ero"></param>
        /// <returns></returns>
        static string InnerLogException(Exception ero)
        {
            string host = HttpContext.Current.Request.Url.Host.ToUpper();
            string errorCode = host.Replace(".", "_");
            lock (lockObj)
            {
                exceptionId += 1;
                errorCode += ":" + EventLog.GetSecondFolder() + ":" + exceptionId;
            }

            ero = GetInnerException(ero);
            EventLog.LogItem item = new EventLog.LogItem();
            item.Title = "页面发生错误,错误代码:" + errorCode;
            if (ero != null)
            {
                item.Detail = ero.Message + "\r\n" + ero.StackTrace + "\r\n";
            }
            EventLog.Log(item, "Error");
            if (host == "LOCALHOST")
            {
                return errorCode;
            }


            return errorCode;
        }

        /// <summary>
        /// 页面输出并写入错误日志
        /// </summary>
        /// <param name="ero"></param>
        public static void WriteException(Exception ero)
        {
            //EventLog.Log("start1", "error");
            bool logError = true;
            if (ero is HttpException)
            {
                HttpException he = ero as HttpException;
                int code = he.GetHttpCode();
                if (code == 404)
                {
                    logError = false;
                }
            }
            if (ero is HttpRequestValidationException)
            {
                //HttpContext.Current.Response.Write("请不要输入非法字符");
                //HttpContext.Current.Response.End();
                //return;
                logError = false;
            }
            //EventLog.Log("start2", "error");
            string errorCode = string.Empty;
            if (logError)
            {
                //EventLog.Log("start6", "error");
                errorCode = InnerLogException(ero);
            }
            if (Utility.IsLocal())
            {

                //第一种页面错误输出方式
                //WriteException(ero, errorCode);
                Show("");

            }
            else
            {
                //EventLog.Log("start4", "error");
                //第二种页面错误输出方式，不显示具体错误信息，只显示友好提示
                //WriteException(ero, errorCode);
                Show("");
            }
            //EventLog.Log("start5", "error");
            //第三种页面错误输出方式，和第一种只有显示样式的区别，其他大致相同
            //Show(ero);
        }

        /// <summary>
        /// 获取错误信息JAVASCRIPT应用
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMessageByJavascript(Exception ex)
        {
            ex = GetInnerException(ex);
            return string.Format("{0}\n{1}\n{2}", ex.Message, ex.Source, ex.StackTrace);
        }
        /// <summary>
        /// 获取错误信息HTML的应用
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMessageByHTML(Exception ex)
        {
            ex = GetInnerException(ex);
            return string.Format("{0}<br/>{1}<br/>{2}", ex.Message, ex.Source, ex.StackTrace);
        }
        /// <summary>
        /// 获取错误信息，写入日志文件中使用
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMessageByLog(Exception ex)
        {
            ex = GetInnerException(ex);
            return string.Format("{0}<\r\n{1}\r\n{2}", ex.Message, ex.Source, ex.StackTrace);

        }

        /// <summary>
        /// 友好错误信息提示
        /// </summary>
        /// <param name="strMessage"></param>
        public static void Show(string strMessage)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body>");
            stringBuilder.Append("<h3 style=\"color:#6699cc;border-bottom:1px solid #8cb2d9;padding-left:20px;\">【 错误提示 】</h3>");
            stringBuilder.Append("<div style=\"padding-left:45px;line-height:35px;color:#666\">");
            stringBuilder.Append("很抱歉，您要访问的页面无法正确显示，可能是因为如下原因：<br />");
            stringBuilder.Append("1 . 系统过于繁忙，请点击浏览器的“刷新”按钮，或稍后再试。<br />");
            stringBuilder.Append("2 . 您输入的网址有误，请重新检查您输入的网址。 <br />");
            stringBuilder.Append("3 . 此页面已经删除，请访问其他页面。 <br />");
            stringBuilder.Append("4 . <a href=\"/\">返回首页</a> <br />");
            stringBuilder.Append(strMessage);
            stringBuilder.Append("</div>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");
            HttpContext.Current.Response.Write(stringBuilder.ToString());
            //EventLog.Log("show1", "error");
            HttpContext.Current.Response.End();
            return;

        }
        /// <summary>
        /// 错误信息显示
        /// </summary>
        /// <param name="source">错误源</param>
        /// <param name="message">信息码</param>
        /// <param name="stackTrace">堆栈信息</param>
        /// <param name="url">错误页面地址</param>
        /// <param name="servername">服务器名</param>
        public static void Show(string source, string message, string stackTrace, string url, string servername)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body>");
            stringBuilder.Append("<h3 style=\"color:#6699cc;border-bottom:1px solid #8cb2d9;padding-left:20px;\">【 错误提示 】</h3>");
            stringBuilder.Append("<div style=\"padding-left:45px;line-height:35px;color:#666\">");
            stringBuilder.AppendFormat("source:{0}<br/>", source);
            stringBuilder.AppendFormat("message:{0}<br/>", message);
            stringBuilder.AppendFormat("stackTrace:{0}<br/>", stackTrace);
            stringBuilder.AppendFormat("url:{0}<br/>", url);
            stringBuilder.AppendFormat("servername:{0}<br/>", servername);
            stringBuilder.Append("Time:" + DateTime.Now);
            stringBuilder.Append("</div>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");
            HttpContext.Current.Response.Write(stringBuilder.ToString());
            HttpContext.Current.Response.End();
            return;
        }
        /// <summary>
        /// 详细错误信息显示
        /// </summary>
        /// <param name="ex"></param>
        public static void Show(Exception ex)
        {
            ex = GetInnerException(ex);
            string erroDetail = ex.StackTrace;
            erroDetail = erroDetail.Replace("\r\n", "<br/>");
            //erroDetail = HttpContext.Current.Server.HtmlEncode(erroDetail);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body>");
            stringBuilder.Append("<h3 style=\"color:#6699cc;border-bottom:1px solid #8cb2d9;padding-left:20px;\">【 错误提示 】</h3>");
            stringBuilder.Append("<div style=\"padding-left:45px;line-height:35px;color:#666\">");
            stringBuilder.AppendFormat("source:{0}<br/>", ex.Source);
            stringBuilder.AppendFormat("message:{0}<br/>", ex.Message);
            stringBuilder.AppendFormat("stackTrace:{0}<br/>", erroDetail);
            stringBuilder.AppendFormat("url:{0}<br/>", HttpContext.Current.Request.Url);
            stringBuilder.AppendFormat("servername:{0}<br/>", HttpContext.Current.Request.Url.Host);
            stringBuilder.Append("Time:" + DateTime.Now);
            stringBuilder.Append("</div>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");
            HttpContext.Current.Response.Write(stringBuilder.ToString());
            HttpContext.Current.Response.End();
            return;
        }
    }
}
