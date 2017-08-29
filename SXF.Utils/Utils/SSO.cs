using System.Collections;
using System.Web;
using System.Web.Mvc;
using System;

namespace SXF.Utils
{
    /// <summary>
    /// 单点登录相关处理
    /// </summary>
    public static class SSO
    {
        /// <summary>
        /// 注销旧会员，存新会员
        /// </summary> 
        public static void LogOffOldUserNameAndSaveNewUserName()
        {

            if (Cookie.GetCookie("guid").IsNullOrEmpty() || Cookie.GetCookie("username").IsNullOrEmpty())
            {
                return;
            }
            Hashtable hOnline = (Hashtable)HttpContext.Current.Application["Online"];
            if (hOnline != null)
            {
                int i = 0;
                while (i < hOnline.Count) //因小BUG所以增加此判断，强制查询到底 
                {
                    //EventLog.WriteLog("login1");
                    IDictionaryEnumerator idE = hOnline.GetEnumerator();
                    string strKey = "";
                    while (idE.MoveNext())
                    {
                        if (idE.Value != null && idE.Value.ToString().Equals(Cookie.GetCookie("username")))
                        {
                            //EventLog.WriteLog("zhuxiao:" + idE.Key.ToString());
                            //已经登录，则注销哈希表中的该键值对  
                            strKey = idE.Key.ToString();
                            hOnline[strKey] = "XXXXXX";
                            break;
                        }
                    }
                    i++;
                }

            }
            else
            {
                //EventLog.WriteLog("login2");
                hOnline = new Hashtable();
            }
            //EventLog.WriteLog("save:" + HttpContext.Current.Session.SessionID);
            //重新存新的会员键值对
            hOnline[Cookie.GetCookie("guid")] = Cookie.GetCookie("username");
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["Online"] = hOnline;
            HttpContext.Current.Application.UnLock();
            //EventLog.WriteLog("login3");


        }
        
        /// <summary>
        /// cookie过期注销application在线状态，并注销相应cookie
        /// </summary>
        public static void CancelOnline()
        {
            if(Cookie.GetCookie("username").IsNullOrEmpty())  //cookie过期
            {
                //Session过期时清除application中的相应会员信息或者退出系统时释放资源
                Hashtable hOnline = (Hashtable)HttpContext.Current.Application["Online"];
                if (hOnline == null || Cookie.GetCookie("guid") == null)
                {
                    return;
                }

                //EventLog.WriteLog("hOnline:" + (hOnline == null));
                if (hOnline[Cookie.GetCookie("guid")] != null)
                {
                    hOnline.Remove(Cookie.GetCookie("guid"));
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["Online"] = hOnline;
                    HttpContext.Current.Application.UnLock();
                    Cookie.Delete("guid");
                }
            }
           
        }
        /// <summary>
        /// 验证会员是否在线
        /// </summary>
        /// <returns></returns>
        public static int Online()
        {
            //EventLog.WriteLog("start");
            Hashtable hOnline = (Hashtable)HttpContext.Current.Application["Online"];
            if (hOnline != null)
            {
                if (Cookie.GetCookie("username").IsNullOrEmpty() && hOnline[Cookie.GetCookie("guid")] != null)
                {
                    hOnline.Remove(Cookie.GetCookie("guid"));
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["Online"] = hOnline;
                    HttpContext.Current.Application.UnLock();
                    Cookie.Delete("guid");
                    return -1;
                }
                IDictionaryEnumerator idE = hOnline.GetEnumerator();                
                while (idE.MoveNext())
                {
                    if (idE.Key != null && Cookie.GetCookie("guid") != null && idE.Key.ToString().Equals(Cookie.GetCookie("guid")))
                    {
                        //EventLog.WriteLog("start1" + i);
                        //already login  
                        if (idE.Value != null && "XXXXXX".Equals(idE.Value.ToString()))
                        {
                            if(Cookie.GetCookie("guid")!=null)
                            {
                                hOnline.Remove(Cookie.GetCookie("guid"));
                                HttpContext.Current.Application.Lock();
                                HttpContext.Current.Application["Online"] = hOnline;
                                HttpContext.Current.Application.UnLock();
                            }
                            
                            CancelOnline();
                            return 0;
                        }
                        else
                        {
                            //EventLog.WriteLog("start3" + i);
                            return 1;
                        }

                    }

                }
            }
            CancelOnline();
            return -1;
        }

        /// <summary>
        /// 验证会员是否在线
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public static int Online(AuthorizationContext filterContext)
        {
            //EventLog.WriteLog("start");
            Hashtable hOnline = (Hashtable)filterContext.HttpContext.Application["Online"];
            if (hOnline != null)
            {
                if (Cookie.GetCookie("username").IsNullOrEmpty() && hOnline[Cookie.GetCookie("guid")] != null)
                {
                    hOnline.Remove(Cookie.GetCookie("guid"));
                    filterContext.HttpContext.Application.Lock();
                    filterContext.HttpContext.Application["Online"] = hOnline;
                    filterContext.HttpContext.Application.UnLock();
                    Cookie.Delete("guid");
                    return -1;
                }
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                int online = -1;
                while (idE.MoveNext())
                {
                    if (idE.Key != null)
                    {
                        if (Cookie.GetCookie("guid")!=null&&idE.Key.ToString().Equals(Cookie.GetCookie("guid")))
                        {

                            //already login  
                            if (idE.Value != null)
                            {
                                if ("XXXXXX".Equals(idE.Value.ToString()))
                                {
                                    //EventLog.WriteLog("start2");
                                    hOnline.Remove(Cookie.GetCookie("guid"));
                                    filterContext.HttpContext.Application.Lock();
                                    filterContext.HttpContext.Application["Online"] = hOnline;
                                    filterContext.HttpContext.Application.UnLock();
                                    CancelOnline();
                                    online = 0;
                                    return online;

                                }
                                else
                                {
                                    //EventLog.WriteLog("start3");
                                    online = 1;
                                    return online;

                                }

                            }



                        }

                    }



                }
                return online;
            }
            CancelOnline();
            return -1;
        }
    }
}
