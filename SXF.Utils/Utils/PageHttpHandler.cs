using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace SXF.Utils
{
    /// <summary>
    /// 页面执行时间实现类
    /// </summary>
    public class PageHttpHandler : IHttpHandler, IRequiresSessionState
    {
        
        public void ProcessRequest(HttpContext context)
        {
            //EventLog.WriteLog("page1");
            string rawUrl = context.Request.RawUrl;
            string file = context.Request.Path;
            DateTime startTime = DateTime.Now;
            string aspxPagePath = context.Request.Path;
            try
            {
                IHttpHandler handler = PageParser.GetCompiledPageInstance(aspxPagePath, null, context);//执行请求的页面
                // Process the page just like any other aspx page
                context.Handler = handler;
                handler.ProcessRequest(context);
                TimeSpan duration = DateTime.Now - startTime;
                //EventLog.WriteLog(""+duration);
                //如果页面执行时间超过1000毫秒，则日志中进行记录
                if (duration.TotalMilliseconds >= 1000)
                {
                    string message = Convert.ToUInt32(duration.TotalMilliseconds).ToString().PadRight(6) + "ms " + file;
                    EventLog.Log(message, "Page");
                    //EventLog.WriteLog("page3");
                }
            }
            catch (Exception ero)
            {
                EventLog.Error("PageHttpHandler:" + ero.Message);
                throw ero;
            }
           
        }

        
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
