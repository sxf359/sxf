using System;
using System.Web.Mvc;

namespace SXF.Utils.MVC
{
    /// <summary>
    /// 错误日志记录
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class LogExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                #region 记录错误日志
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                string actionName = (string)filterContext.RouteData.Values["action"];
                string msgTemplate = string.Format("在执行 controller[{0}] 的 action[{1}] 时产生异常", controllerName, actionName);
                EventLog.LogItem item = new EventLog.LogItem();
                item.Title = msgTemplate;
                LogManage.WriteException(filterContext.Exception, item);
                #endregion
                if (!(filterContext.Result is JsonResult))  //当非JsonResult并且需要自定义错误页面时
                {
                    if (!Utility.IsLocal())  //非本地，并且需要自定义错误时执行
                    {
                        //通知MVC框架，现在这个异常已经被我处理掉，你不需要将黄页显示给用户
                        filterContext.ExceptionHandled = true;
                        //跳转到错误提醒页面               
                        filterContext.Result = new ViewResult { ViewName = "error" };
                    }

                }

            }


            if (filterContext.Result is JsonResult)
            {
                //当结果为json时，设置异常已处理，此时要在action上调用JsonException属性
                filterContext.ExceptionHandled = true;
            }
            else
            {
                //否则调用原始设置
                base.OnException(filterContext);
            }
        }
    }
}
