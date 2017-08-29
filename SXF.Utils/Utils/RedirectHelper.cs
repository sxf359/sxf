using System.Web;

namespace SXF.Utils
{

    public class RedirectHelper
    {
        /// <summary>
        /// 解决服务器无法在已发送 HTTP 标头之后设置状态错误的跳转方法
        /// </summary>
        /// <param name="url"></param>
        public static void RedirectUrl(string url)
        {
            HttpContext.Current.Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            HttpContext.Current.Response.BufferOutput = true;//设置输出缓冲
            if (!HttpContext.Current.Response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                HttpContext.Current.Response.Redirect(url, true);
            }
        }
    }
}
