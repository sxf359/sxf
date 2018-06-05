
using SXF.Utils;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;
using System.Threading;
using LMSoft.Web.Hubs;
using SXF.Utils.QuartzScheduler;
using System.Linq;

namespace LMSoft.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //create
        private static QuartzWorker _worker;
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterHandler(RouteTable.Routes);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // 定时工作任务
            _worker = new QuartzWorker();
            var recycleWork = new LMSoft.Web.Hubs.TheTask.RecycleCustomerWork();
            _worker.AddWork(recycleWork);
            //var testWork = new LMSoft.Web.Hubs.TheTask.TestWork();
            //_worker.AddWork(testWork);
            _worker.Start();


        }
        private void Session_End(object sender, EventArgs e)
        {

            //这里设置你的web地址，可以随便指向你的任意一个aspx页面甚至不存在的页面，目的是要激发Application_Start
            //使用您自己的URL
            //string url = "http://localhost";
            //System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            //System.Net.HttpWebResponse myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
            //System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流




            // 在会话结束时运行的代码。
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为 InProc 时，才会引发 Session_End 事件。
            // 如果会话模式设置为 StateServer 或 SQLServer，则不会引发该事件。
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //在应用程序关闭时运行的代码
            if (_worker != null)
            {
                _worker.Stop();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //if (Request.HttpMethod == "GET")
            //{
            //    //EventLog.WriteLog("a");
            //    //Response.Clear();
            //    Response.BufferOutput = true;
            //    //Response.Flush();
            //    //Response.End();
            //}
        }

    }
}




