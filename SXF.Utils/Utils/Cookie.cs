using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace SXF.Utils
{
    /// <summary>
    /// Cookie操作类
    /// </summary>
    public class Cookie
    {
        #region 静态方法



        #region 创建或修改COOKIE对象并赋Value值
        /// <summary>
        /// 创建或修改COOKIE对象并赋Value值
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        /// <remarks>
        /// 对COOKIE修改必须重新设Expires
        /// </remarks>
        public static void SetCookie(string strCookieName, string strValue)
        {
            SetCookie(strCookieName, 1, strValue);
        }

        /// <summary>
        /// 创建或修改COOKIE对象并赋Value值
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <param name="strValue">COOKIE对象Value值</param>
        /// <remarks>
        /// 对COOKIE修改必须重新设Expires
        /// </remarks>
        public static void SetCookie(string strCookieName, int iExpires, string strValue)
        {
            HttpCookie objCookie;
            if (HttpContext.Current.Request.Cookies[strCookieName.Trim()] == null)  //若不存在则创建
            {
                objCookie = new HttpCookie(strCookieName.Trim());
            }
            else    //否则读取
            {
                objCookie = HttpContext.Current.Request.Cookies[strCookieName.Trim()];
            }

            objCookie.Value = HttpContext.Current.Server.UrlEncode(strValue.Trim());
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <param name="KeyValue">键/值对集合</param>
        public static void SetCookie(string strCookieName, int iExpires, NameValueCollection KeyValue)
        {
            HttpCookie objCookie;
            if (HttpContext.Current.Request.Cookies[strCookieName.Trim()] == null)  //若不存在则创建
            {
                objCookie = new HttpCookie(strCookieName.Trim());
            }
            else    //否则读取
            {
                objCookie = HttpContext.Current.Request.Cookies[strCookieName.Trim()];
            }

            foreach (string key in KeyValue.AllKeys)
            {
                objCookie[key] = HttpContext.Current.Server.UrlEncode(KeyValue[key].Trim());
            }
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 创建或修改COOKIE对象并赋Value值
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <param name="strDomain">作用域</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        /// <remarks>
        /// 对COOKIE修改必须重新设Expires
        /// </remarks>
        public static void SetCookie(string strCookieName, int iExpires, string strValue, string strDomain)
        {
            HttpCookie objCookie;
            if (HttpContext.Current.Request.Cookies[strCookieName.Trim()] == null)  //若不存在则创建
            {
                objCookie = new HttpCookie(strCookieName.Trim());
            }
            else    //否则读取
            {
                objCookie = HttpContext.Current.Request.Cookies[strCookieName.Trim()];
            }

            objCookie.Value = HttpContext.Current.Server.UrlEncode(strValue.Trim());
            objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }


        

        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <param name="strDomain">作用域</param>
        /// <param name="KeyValue">键/值对集合</param>
        public static void SetCookie(string strCookieName, int iExpires, NameValueCollection KeyValue, string strDomain)
        {
            HttpCookie objCookie;
            if (HttpContext.Current.Request.Cookies[strCookieName.Trim()] == null)  //若不存在则创建
            {
                objCookie = new HttpCookie(strCookieName.Trim());
            }
            else    //否则读取
            {
                objCookie = HttpContext.Current.Request.Cookies[strCookieName.Trim()];
            }

            foreach (string key in KeyValue.AllKeys)
            {
                objCookie[key] = HttpContext.Current.Server.UrlEncode(KeyValue[key].Trim());
            }
            objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        #endregion

        #region 读取cookie操作

        /// <summary>
        /// 读取Cookie某个对象的Value值，返回Value值
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <returns>Value值，如果对象本就不存在，则返回 null</returns>
        public static string GetCookie(string strCookieName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies[strCookieName].Value);
            }
        }

        /// <summary>
        /// 读取Cookie某个对象的某个Key键的键值
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <returns>Key键值，如果对象或键值就不存在，则返回 null</returns>
        public static string GetCookie(string strCookieName, string strKeyName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            else
            {
                string strObjValue = HttpContext.Current.Request.Cookies[strCookieName].Value;
                string strKeyName2 = strKeyName + "=";
                if (strObjValue.IndexOf(strKeyName2) == -1)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies[strCookieName][strKeyName]);
                }
            }
        }
        #endregion 


        #region 修改cookie值操作
        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值或给某个COOKIE对象添加Key键
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="KeyValue">Key键值</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <returns>如果对象本就不存在，则返回 false</returns>
        public static bool Edit(string strCookieName, string strKeyName, string KeyValue, int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return false;
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie[strKeyName] = HttpContext.Current.Server.UrlEncode(KeyValue.Trim());
                if (iExpires > 0)
                {
                    if (iExpires == 1)
                    {
                        objCookie.Expires = DateTime.MaxValue;
                    }
                    else
                    {
                        objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                    }
                }
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return true;
            }
        }


        /// <summary>
        /// 修改cookie对象的某个键值，设置cookie有效期为24小时
        /// </summary>
        /// <param name="strCookieName"></param>
        /// <param name="strKeyName"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public static bool Edit(string strCookieName, string strKeyName, string KeyValue)
        {
            return Edit(strCookieName, strKeyName, KeyValue, 24 * 60 * 60);
        }
        #endregion 


        #region 删除cookie操作
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        public static void Delete(string strCookieName)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 删除某个COOKIE对象某个Key子键
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="iExpires">
        /// COOKIE对象有效时间（秒数）
        /// 1表示永久有效，0和负数都表示不设有效时间
        /// 大于等于2表示具体有效秒数
        /// 86400秒 = 1天 = （60*60*24）
        /// 604800秒 = 1周 = （60*60*24*7）
        /// 2593000秒 = 1月 = （60*60*24*30）
        /// 31536000秒 = 1年 = （60*60*24*365）
        /// </param>
        /// <returns>如果对象本就不存在，则返回 false</returns>
        public static bool Delete(string strCookieName, string strKeyName, int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return false;
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie.Values.Remove(strKeyName);
                if (iExpires > 0)
                {
                    if (iExpires == 1)
                    {
                        objCookie.Expires = DateTime.MaxValue;
                    }
                    else
                    {
                        objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                    }
                }
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return true;
            }
        }

        /// <summary>
        /// 删除某个COOKIE对象某个Key子键，有效期设置为24小时
        /// </summary>
        /// <param name="strCookieName"></param>
        /// <param name="strKeyName"></param>
        /// <returns></returns>
        public static bool Delete(string strCookieName, string strKeyName)
        {
            return Delete(strCookieName, strKeyName, 24 * 60 * 60);
        }
        #endregion 



        #endregion
    }
}
