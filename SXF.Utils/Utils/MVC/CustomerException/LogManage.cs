using System;
using System.Text;
using System.Web;


namespace SXF.Utils.MVC
{
    /// <summary>
    /// mvc日志管理
    /// </summary>
    public class LogManage
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

        static long exceptionId = 0;
        static object lockObj = new object();


        /// <summary>
        /// 内部记录日志
        /// </summary>
        /// <param name="ero"></param>
        /// <returns></returns>
        static string InnerLogException(Exception ero, EventLog.LogItem item)
        {
            string host = HttpContext.Current.Request.Url.Host.ToUpper();
            string errorCode = host.Replace(".", "_");
            lock (lockObj)
            {
                exceptionId += 1;
                errorCode += ":" + EventLog.GetSecondFolder() + ":" + exceptionId;
            }

            ero = GetInnerException(ero);


            item.Title = item.Title + ",错误代码:" + errorCode;
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
        public static void WriteException(Exception ero, EventLog.LogItem item)
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
                logError = false;
            }
            //EventLog.Log("start2", "error");
            string errorCode = string.Empty;
            if (logError)
            {
                //EventLog.Log("start6", "error");
                errorCode = InnerLogException(ero, item);
            }

        }



    }
}
