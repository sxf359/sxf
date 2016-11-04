namespace SXF.Utils
{
    using System.Web;
    /// <summary>
    /// 对Session操作进行封装
    /// </summary>
    public class SessionState
    {
        /// <summary>
        /// 从 Session 读取 键为 name 的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object Get(string name)
        {
            return HttpContext.Current.Session[name];
        }
        /// <summary>
        ///  从 Session 删除 键为 name session 项
        /// </summary>
        /// <param name="name"></param>
        public static void Remove(string name)
        {

            if (HttpContext.Current.Session[name] != null)
            {
                HttpContext.Current.Session.Remove(name);
            }
        }
        /// <summary>
        /// 删除所有 session 项
        /// </summary>
        public static void RemoveAll()
        {
            HttpContext.Current.Session.RemoveAll();
        }
        /// <summary>
        /// 向 Session 保存 键为 name 的， 值为 value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void Set(string name, object value)
        {

            HttpContext.Current.Session.Add(name, value);
        }
        /// <summary>
        /// 摧毁网站所有session
        /// </summary>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }
    }
}

