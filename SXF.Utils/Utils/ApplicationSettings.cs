﻿namespace SXF.Utils
{
    using System.Configuration;
    /// <summary>
    /// 对ConfigurationSettings.AppSettings操作类
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// 获取web.config的配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            string str = ConfigurationManager.AppSettings[key];
            //EventLog.WriteLog("key="+key);
            if (str == null)
            {

                //throw new FrameworkExcption("sxf");
                throw new FrameworkExcption("WebConfigHasNotAddKey ApplicationSettings", new string[] { key });
            }
            return str;
        }
    }
}

