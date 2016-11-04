using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace SXF.Utils
{
    public class PageHttpHandler : IHttpHandler, IRequiresSessionState
    {
        
        public void ProcessRequest(HttpContext context)
        {
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
                if (duration.TotalMilliseconds > 0)
                {
                    string message = Convert.ToUInt32(duration.TotalMilliseconds).ToString().PadRight(6) + "ms " + file;
                    EventLog.Log(message, "Page");
                }
            }
            catch (Exception ero)
            {
                EventLog.Error("PageHttpHandler:" + ero.Message);
                throw ero;
            }
            //context.Response.Write(String.Format("Request finshed. Total duration: {0} ms.", duration.Milliseconds));
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
