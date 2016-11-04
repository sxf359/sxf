using System;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SXF.Utils
{
    public class UrlFilterModule : IHttpModule
    {

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
            //application.EndRequest += (new EventHandler(this.Application_EndRequest));
            //application.
        }
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication Application = (HttpApplication)source;
            HttpResponse Response = Application.Context.Response;


            #region 判断域名是二级城市域名，如果是则重定向到相应目录
            string oldUrl = Application.Context.Request.Url.ToString().ToLower();
            //EventLog.WriteLog(oldUrl);
            //url中含有/city/sanmenxia，错误地址，不能直接访问，进行替换
            //oldUrl = oldUrl.Replace("/city/sanmenxia", "");

            //解决在分站访问当页面路径唯有文件名情况下。系统自动会加上/city/sanmenxia而导致的地址栏中实际显示的路径与访问的虚拟路径不同的问题
            if (oldUrl.Contains("/city/sanmenxia"))
            {
                //直接重定向
                Application.Context.Response.Redirect(oldUrl.Replace("/city/sanmenxia", ""));
            }

            if (UrlRewriteRules.OldCitySiteUrl.Contains("," + Application.Context.Request.Url.Host + ","))
            {
                //EventLog.WriteLog(oldUrl);
                string newUrl = oldUrl.Replace("http://" + Application.Context.Request.Url.Host, UrlRewriteRules.NewCitySiteUrl);
                //解决访问的链接地址没有文件名的情况下，httpmodule会处理两次的情况
                 if(FileManager.GetFileExtension(newUrl).IsNullOrEmpty())
                 {
                     if(!newUrl.EndsWith("/"))
                     {
                         newUrl += "/";
                     } 
                     newUrl += "index.aspx";
                 }

                //EventLog.WriteLog(newUrl);
                Application.Context.RewritePath(newUrl,false);
                return;


            }
            #endregion



            if (!FileManager.Exists(Application.Context.Request.PhysicalPath, FsoMethod.All))
            {
                //EventLog.WriteLog(Application.Context.Request.PhysicalPath);
                //获取请求的URL
                string path = Application.Context.Request.RawUrl;
                //EventLog.WriteLog(path);
                //去除路径中?后面的内容
                if (path.IndexOf("?") >= 0)
                {
                    path = Utility.Left(path, path.IndexOf("?"));
                }
                //EventLog.WriteLog(path);
                //EventLog.WriteLog(path.IndexOf("?").ToString());


                List<PathMapping> list = UrlRewriteRules.UrlRewrite();
                //EventLog.WriteLog("开始1");
                foreach (PathMapping pathMapping in list)
                {
                    //EventLog.WriteLog("开始2");
                    string pattern = @"^" + pathMapping.VirtualPath + "$";

                    string realPathRules = pathMapping.RealPath;
                    //匹配url规则
                    Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    //EventLog.WriteLog(regex.IsMatch(path).ToString());
                    if (regex.IsMatch(path))   //如果匹配成功
                    {
                        //获取匹配集合
                        Match mc = regex.Match(path);
                        //EventLog.WriteLog("mc.Groups.Count:" + mc.Groups.Count + "<br/>");
                        string realPath = "";
                        for (int i = 0; i < mc.Groups.Count; i++)
                        {

                            //EventLog.WriteLog("continue前");
                            if (i == 0)  //第一个匹配串是整串匹配，没用，丢弃
                            {
                                continue;
                            }
                            //EventLog.WriteLog("continue后");
                            //从第二个匹配字串起，替换$1,$2,$3这样url串中的参数
                            realPath = realPathRules.Replace("$" + i.ToString(), mc.Groups[i].Value);
                            //realPathRules值重置，因为如果是多个参数$1,$2,$3,不重置，则上行的realPathRules仍然是原来的$1,$2,$3,循环替换就没有成功
                            realPathRules = realPath;
                            //EventLog.WriteLog(realPath);

                        }
                        //EventLog.WriteLog(realPath);
                        //重写url
                        Application.Context.RewritePath(realPath);
                        return;
                    }
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Application_EndRequest(Object source, EventArgs e)
        {
            //HttpApplication application = (HttpApplication)source;
            //HttpResponse Response = application.Context.Response;
            //Response.Write("<h1>End of Request</h1><hr>");
        }
        public void Dispose()
        {
        }
    }
}
