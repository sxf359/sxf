using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;


namespace SXF.Kernel
{
    /// <summary>
    /// Form验证，Cookie有效期通过CheckTicket进行限制
    /// </summary>
    public class FormAuthentication
    {
        /// <summary>
        /// 检测票据
        /// </summary>
        public static void CheckTicket()
        {
            CheckTicket(false);
        }
        /// <summary>
        /// Session是否验证通过
        /// 在Cookie验证通过时,验证此值,能保证是从网站上登录,并且以Session方式验证通过
        /// </summary>
        public static bool SessionVerified
        {
            get
            {
                return HttpContext.Current.Session["SessionState"] != null;
            }
        }
        /// <summary>
        /// 检测票据
        /// </summary>
        /// <param name="expires">
        /// 是否过期,过期会自动把过期时间延长600分钟,会造成COOKIE重写,需和SetTicket参数一致
        /// 如果域名为全域名则为FALSE
        /// </param>
        public static void CheckTicket(bool expires)
        {
            if (!HttpContext.Current.Request.Path.ToLower().EndsWith(".aspx"))
            {
                return;
            }
            FormsAuthenticationTicket authTicket = null;
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (authCookie == null)
                return;
            if (authCookie.Value == "")
                return;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket == null)
                    return;
                if (authTicket.IssueDate.Date != DateTime.Now.Date)
                {
                    return;
                }
                string[] roles = authTicket.UserData.Split(new char[] { ',' });

                FormsIdentity id = new FormsIdentity(authTicket);

                System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, roles);
                HttpContext.Current.User = principal;

                if (expires)
                {
                    string domain = authCookie.Domain;
                    //如果不是主域COOKIE
                    if (domain == null || domain.Substring(0, 1) != ".")
                    {
                        //让20分钟后过期
                        authCookie.Expires = DateTime.Now.AddMinutes(600);
                        HttpContext.Current.Response.Cookies.Add(authCookie);
                    }
                }
                else
                {
                    string domain = authCookie.Domain;
                    //如果不是主域COOKIE
                    if (domain == null || domain.Substring(0, 1) != ".")
                    {
                        //让120分钟后过期
                        authCookie.Expires = DateTime.Now.AddMinutes(600);
                        HttpContext.Current.Response.Cookies.Add(authCookie);
                    }
                }
            }
            catch (Exception ero)
            {
                HttpContext.Current.Response.Write("检测票据出现错误:" + ero);
            }
        }

        /// <summary>
        /// 设置票据
        /// </summary>
        /// <param name="ticketName"></param>
        /// <param name="rules"></param>
        /// <param name="expires"></param>
        public static void SetTicket(string ticketName, string rules, bool expires)
        {
            SetTicket(ticketName, rules, expires, null);
        }
        /// <summary>
        /// 设置票据
        /// </summary>
        /// <param name="userName">登录名</param>
        /// <param name="rules">组</param>
        /// <param name="expires">是否过期,如果设置了域,则为false</param>
        /// <param name="domain">域</param>
        public static void SetTicket(string ticketName, string rules, bool expires, string domain)
        {
            //暂时为永不过期
            //expires = false;
            HttpContext context = HttpContext.Current;
            string userDate = rules;
            DateTime expiration = DateTime.Now.AddMinutes(120);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,  // 版本号
                        ticketName,  // 票据名
                        DateTime.Now,  // 票据发出时间
                        DateTime.Now.AddDays(2),  // 过期时间2天
                        true,  // 是否持久
                         userDate// 存储在 Cookie 中的用户定义数据,这里用来存取用户组
                        );
            // Encrypt the ticket
            string cookieStr = FormsAuthentication.Encrypt(ticket);
            //context.Response.Cookies[FormsAuthentication.FormsCookieName].Value = cookieStr;
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieStr);

            if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Path = "/";
            }
            else
            {
                //如果过期
                if (expires)
                {
                    cookie.Expires = expiration;
                }
            }
            context.Response.Cookies.Add(cookie);
            context.Session["SessionState"] = true;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public static void LoginOut()
        {
            LoginOut(null);
        }
        public static void LoginOut(string returnUrl)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                return;
            }
            FormsAuthentication.SignOut();
            HttpCookie c = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (c == null)
            {
                return;
            }
            c.Expires = DateTime.Now.AddYears(-100);
            context.Response.SetCookie(c);

            c.Domain = FormsAuthentication.CookieDomain;
            context.Response.SetCookie(c);
            context.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-100);

            context.Session.Clear();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                context.Response.Redirect(returnUrl);
            }
            return;


        }
    }
}
