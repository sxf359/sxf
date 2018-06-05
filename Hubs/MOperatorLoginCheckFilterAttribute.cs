using LMSoft.FrameWork.Identity;
using Microsoft.AspNet.Identity;
using SXF.Utils;
using System;
using System.Web;
using System.Web.Mvc; 


namespace LMSoft.Web.Hubs
{

    /// <summary>
    /// 话务员是否登录判断
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)]
    public class MOperatorLoginCheckFilterAttribute : AuthorizeAttribute
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
                    filterContext.Result = new RedirectResult("~/m/home/index?error=4", true);
                    return;

                }
                var user = new LMIdentityDbContext().Users.Find(filterContext.HttpContext.User.Identity.GetUserId());
                if (user == null || (user.Role != "admin" || user.RoleLevel != 0))
                {
                    //跳转到登陆页
                    filterContext.Result = new RedirectResult("~/m/home/index?error=1", true);
                    return;
                }


                if (Cookie.GetCookie("guid") != filterContext.HttpContext.User.Identity.GetUserId())
                {
                    //跳转到登陆页  超时过期
                    filterContext.Result = new RedirectResult("~/m/home/index?error=5", true);
                    return;
                }


            }



        }
    }
}
