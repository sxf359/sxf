using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace SXF.Utils
{
    public static class PageHelper
    {
        public static Page GetPage()
        {
            if (HttpContext.Current != null)
            {
                return (HttpContext.Current.Handler as Page);
            }
            else
            {
                throw new Exception("HttpContext.Current对象为空，所以不能用此方法！");
            }
        }


        #region 请求有误时可以调用的方法

        /// <summary>
        /// 在页面开始输出信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="responseEnd">是否结束响应流的输出</param>
        public static void ErrorRequest(string msg, bool responseEnd)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Write(msg);
            if (responseEnd)
            {
                response.End();
            }
        }
        /// <summary>
        /// 在页面开始输出信息,结束响应流的输出
        /// </summary>
        /// <param name="msg">信息</param>
        public static void ErrorRequest(string msg)
        {
            ErrorRequest(msg, true);
        }

        /// <summary>
        /// 在页面开始输出信息‘您的请求有误!’，结束响应流的输出
        /// </summary>
        public static void ErrorRequest()
        {
            ErrorRequest("您的请求有误!");
        }
        /// <summary>
        /// 在页面开始输出信息‘您的请求有误!’
        /// </summary>
        /// <param name="responseEnd">是否结束响应流的输出</param>
        public static void ErrorRequest(bool responseEnd)
        {
            ErrorRequest("您的请求有误!", responseEnd);
        }

        #endregion

    

        #region 操作url

        /// <summary>
        /// 设置URL的参数
        /// 用Request.Url做为处理的url来处理,Request.Url的形式为绝对路径
        /// </summary>
        /// <param name="key">要设置的key</param>
        /// <param name="value">要赋的值</param>
        /// <returns></returns>
        public static string SetUrlParams(string key, string value)
        {
            return SetUrlParams(key, value, HttpContext.Current.Request.Url.ToString(), false);
        }

        /// <summary>
        /// 设置URL的参数,value为urlEncode编码
        /// 用Request.Url做为处理的url来处理,Request.Url的形式为绝对路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="urlEncode"></param>
        /// <returns></returns>
        public static string SetUrlParams(string key, string value, bool urlEncode)
        {
            return SetUrlParams(key, value, HttpContext.Current.Request.Url.ToString(), true);
        }

        /// <summary>
        /// 设置URL的参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static string SetUrlParams(string key, string value, string url)
        {
            return SetUrlParams(key, value, url, false);
        }
        /// <summary>
        /// 设置URL的参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="url"></param>
        /// <param name="urlEncode"></param>
        /// <returns></returns>
        public static string SetUrlParams(string key, string value, string url, bool urlEncode)
        {
            if (urlEncode)
            {
                if (HttpContext.Current == null)
                {
                    throw new Exception("只有要页面的上下文中才可以用UrlEncode!");
                }
                else
                {
                    value = HttpContext.Current.Server.UrlEncode(value);
                }
            }
            int fragPos = url.LastIndexOf("#");
            string fragment = string.Empty;
            if (fragPos > -1)
            {
                fragment = url.Substring(fragPos);
                url = url.Substring(0, fragPos);
            }
            int querystart = url.IndexOf("?");
            if (querystart < 0)
            {
                url += "?" + key + "=" + value;
            }
            else
            {
                Regex reg = new Regex(@"(?<=[&\?])" + key + @"=[^\s&#]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (reg.IsMatch(url))
                    url = reg.Replace(url, key + "=" + value);
                else
                    url += "&" + key + "=" + value;
            }
            return url + fragment;
        }

        /// <summary>
        /// 移除url的key参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RemoveUrlParams(string key, string url)
        {
            Regex reg = new Regex(@"[&\?]" + key + @"=[^\s&#]*&?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.Replace(url, new MatchEvaluator(PutAwayGarbageFromUrl));
        }

        private static string PutAwayGarbageFromUrl(Match match)
        {
            string value = match.Value;
            if (value.EndsWith("&"))
                return value.Substring(0, 1);
            else
                return string.Empty;
        }

        #endregion

    }
}
