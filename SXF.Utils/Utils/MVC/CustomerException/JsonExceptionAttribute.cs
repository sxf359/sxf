using System;
using System.Web.Mvc;
namespace SXF.Utils.MVC
{
    /// <summary>
    /// json异常处理，只能使用在action上。调用方法
    /// 在json的action上直接使用[JsonException]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class JsonExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                //返回异常JSON
                //filterContext.Result = new JsonResult
                //{
                //    Data = new { Success = false, Message = filterContext.Exception.Message }
                //};

                //{"Message":"尝试除以零。"}
                //filterContext.Result = new JsonResult { Data=new { filterContext.Exception.Message } };

                //{"str":"尝试除以零。"}
                //var str = filterContext.Exception.Message;
                //filterContext.Result = new JsonResult { Data = new { str } };

                var json = new JsonResult();
                json.Data = filterContext.Exception.Message;
                filterContext.Result = json;
            }
        }
    }
}
