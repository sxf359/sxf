using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace LMSoft.Web.Hubs
{
    /// <summary>
    /// 自定义路由，实现二级域名
    /// </summary>
    public class DomainRoute : Route
    {
        private Regex domainRegex;
        private Regex pathRegex;

        public string Domain { get; set; }

        public DomainRoute(string domain, string url, RouteValueDictionary defaults)
            : base(url, defaults, new MvcRouteHandler())
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults, IRouteHandler routeHandler)
            : base(url, new RouteValueDictionary(defaults), routeHandler)
        {
            Domain = domain;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // 构造 regex
            domainRegex = CreateRegex(Domain);
            //EventLog.WriteLog("Domain:" + Domain);
            pathRegex = CreateRegex(Url);
            // 请求信息
            string requestDomain = httpContext.Request.Headers["host"];
            if (!string.IsNullOrEmpty(requestDomain))
            {
                if (requestDomain.IndexOf(":", StringComparison.Ordinal) > 0)
                {
                    requestDomain = requestDomain.Substring(0, requestDomain.IndexOf(":", StringComparison.Ordinal));
                }
            }
            else
            {
                if (httpContext.Request.Url != null) requestDomain = httpContext.Request.Url.Host;
            }
            string requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            //EventLog.WriteLog("requestDomain:" + requestDomain);
            //EventLog.WriteLog("requestPath:" + requestPath);

            // 匹配域名和路由
            Match domainMatch = domainRegex.Match(requestDomain);
            Match pathMatch = pathRegex.Match(requestPath);

            // 路由数据
            RouteData data = null;

            if (domainMatch.Success && pathMatch.Success)
            {
                //EventLog.WriteLog("success");
                data = new RouteData(this, RouteHandler);
                // 添加默认选项
                if (Defaults != null)
                {
                    foreach (KeyValuePair<string, object> item in Defaults)
                    {
                        data.Values[item.Key] = item.Value;
                        #region 此处将area及Namespaces写入DataTokens里
                        if (item.Key.Equals("area") || item.Key.Equals("Namespaces"))
                        {
                            data.DataTokens[item.Key] = item.Value;
                            //EventLog.WriteLog(data.DataTokens[item.Key].ToString());
                            data.DataTokens["UseNamespaceFallback"] = false;
                        }
                        #endregion
                    }
                }

                // 匹配域名路由
                for (int i = 1; i < domainMatch.Groups.Count; i++)
                {
                    Group group = domainMatch.Groups[i];
                    if (group.Success)
                    {
                        string key = domainRegex.GroupNameFromNumber(i);
                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                                #region 新增将area写入到DataTokens中
                                if (key.Equals("area"))
                                {
                                    data.DataTokens[key] = group.Value;
                                    //EventLog.WriteLog("domainMatch-" + key + ":" + data.DataTokens[key].ToString());
                                }
                                #endregion
                            }
                        }
                    }
                }

                // 匹配域名路径
                for (int i = 1; i < pathMatch.Groups.Count; i++)
                {
                    Group group = pathMatch.Groups[i];
                    if (group.Success)
                    {
                        string key = pathRegex.GroupNameFromNumber(i);

                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                                #region 新增将area写入到DataTokens中
                                if (key.Equals("area"))
                                {
                                    data.DataTokens[key] = group.Value;
                                    //EventLog.WriteLog("pathMatch-" + key + ":" + data.DataTokens[key].ToString());
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            //foreach (var item in data.DataTokens)
            //{
            //    EventLog.WriteLog(item.Key + ":" + item.Value);
            //}
            return data;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, RemoveDomainTokens(values));
        }

        public DomainData GetDomainData(RequestContext requestContext, RouteValueDictionary values)
        {
            // 获得主机名
            string hostname = Domain;
            foreach (KeyValuePair<string, object> pair in values)
            {
                hostname = hostname.Replace("{" + pair.Key + "}", pair.Value.ToString());
            }
            //EventLog.WriteLog("hostname:" + hostname);
            // Return 域名数据
            return new DomainData
            {
                Protocol = "http",
                HostName = hostname,
                Fragment = ""
            };
        }

        private Regex CreateRegex(string source)
        {
            // 替换
            source = source.Replace("/", @"\/?");
            source = source.Replace(".", @"\.?");
            source = source.Replace("-", @"\-?");
            source = source.Replace("{", @"(?<");
            source = source.Replace("}", @">([a-zA-Z0-9_\-]*))");
            //EventLog.WriteLog("source:^" + source + "$");
            return new Regex("^" + source + "$", RegexOptions.IgnoreCase);
        }

        private RouteValueDictionary RemoveDomainTokens(RouteValueDictionary values)
        {
            Regex tokenRegex = new Regex(@"({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?");
            Match tokenMatch = tokenRegex.Match(Domain);
            for (int i = 0; i < tokenMatch.Groups.Count; i++)
            {
                Group group = tokenMatch.Groups[i];
                if (group.Success)
                {
                    string key = group.Value.Replace("{", "").Replace("}", "");
                    if (values.ContainsKey(key))
                        values.Remove(key);
                }
            }

            return values;
        }
    }
    public class DomainData
    {
        public string Protocol { get; set; }
        public string HostName { get; set; }
        public string Fragment { get; set; }
    }
}