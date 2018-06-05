using LMSoft.FrameWork.Identity;
using Microsoft.AspNet.Identity;
using SXF.Utils;
using System;
using System.Web;
using System.Web.Mvc;
using LMSoft.CMS.BLL;
using LMSoft.CMS.Models;

namespace LMSoft.Web.Hubs
{
    /// <summary>
    /// 求职者是否登录判断
    /// </summary>
    public class PersonLoginCheckFilterAttribute : AuthorizeAttribute
    {


        /// <summary>
        /// 表示是否检查登录
        /// </summary>
        public bool IsCheck { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool hasRole = base.AuthorizeCore(httpContext);

            //如果为空，则表示尚未登录，为false
            //EventLog.WriteLog("hasRole1:" + hasRole.ToString());
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            return hasRole;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            //EventLog.WriteLog("start3" + IsCheck);
            if (IsCheck)
            {
                if (filterContext.HttpContext.User == null)
                {
                    return;
                }
                if (filterContext.HttpContext.User.Identity == null)
                {
                    return;
                }

                //校验用户是否已经登录
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    //跳转到登陆页
                    filterContext.Result = new RedirectResult("~/account/login?error=4", true);
                    return;

                }
                var user = new LMIdentityDbContext().Users.Find(filterContext.HttpContext.User.Identity.GetUserId());
                if (user == null || (user.Role != "科目一" && user.Role != "科目四"))
                {
                    //跳转到登陆页
                    filterContext.Result = new RedirectResult("~/account/login?error=1", true);
                    return;
                }
                if (user.Role == "科目一" || user.Role == "科目四")
                {

                    if(Cookie.GetCookie("guid")!= filterContext.HttpContext.User.Identity.GetUserId())
                    {
                        //跳转到登陆页  超时过期
                        filterContext.Result = new RedirectResult("~/account/login?error=5", true);
                        return;
                    }


                }


                if (user.LastLoginTime != null)
                {
                    //每5分钟，如果会员有操作，更新上次登录时间为现在
                    if (DateHelper.DateDiff(DateInterval.Minute, user.LastLoginTime.Value, DateTime.Now) > 5)
                    {
                        user.LastLoginTime = DateTime.Now;
                        using (var db = new LMIdentityDbContext())
                        {
                            string sql = string.Format("update aspnetusers set LastLoginTime='{0}' where id='{1}'", user.LastLoginTime, filterContext.HttpContext.User.Identity.GetUserId());
                            db.Database.ExecuteSqlCommand(sql);
                        }
                    }
                }


            }



        }
    }
}