﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace SXF.Utils
{
    /// <summary>
    /// 写日志
    /// 不想自动记录Context信息请调用Log(string message, string typeName, false)
    /// </summary>
    public class EventLog
    {
        /// <summary>
        /// 是否使用上下文信息写日志
        /// </summary>
        static bool UseContext = true;
        static string _thisDomain = "";


        #region LogItem
        [Serializable]
        public class LogItem
        {
            public DateTime Time
            {
                get;
                set;
            }
            public string Title
            {
                get;
                set;
            }
            public string Detail
            {
                get;
                set;
            }
            public string RequestUrl
            {
                get;
                set;
            }
            public string UrlReferrer
            {
                get;
                set;
            }
            public string HostIP
            {
                get;
                set;
            }
            public string UserAgent
            {
                get;
                set;
            }
            public override string ToString()
            {
                string s = Time.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(Title))
                {
                    Title = Detail;
                    Detail = "";
                }
                if (!string.IsNullOrEmpty(Title))
                {
                    s += "  " + Title;
                }
                if (!string.IsNullOrEmpty(RequestUrl))
                {
                    s += "\r\nUrl:" + RequestUrl;
                }
                if (!string.IsNullOrEmpty(UrlReferrer))
                {
                    s += "\r\nUrlReferrer:" + UrlReferrer;
                }
                if (!string.IsNullOrEmpty(HostIP))
                {
                    s += "\r\nHostIP:" + HostIP;
                }
                if (!string.IsNullOrEmpty(UserAgent))
                {
                    s += "\r\n" + UserAgent;
                }
                if (!string.IsNullOrEmpty(Detail))
                {
                    s += "\r\n" + Detail;
                }
                s += "\r\n";
                return s;
            }
        }
        #endregion


        static object lockObj = new object();
        /// <summary>
        /// 检查目录并建立
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFolder(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return;
            }
            string folder = "";
            string[] arry = path.Split('\\');
            for (int i = 0; i < arry.Length; i++)
            {
                folder += arry[i] + "\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }
        /// <summary>
        /// 自定义文件名前辍写入日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="typeName"></param>
        /// <param name="useContext"></param>
        /// <returns></returns>
        public static bool Log(string message, string typeName, bool useContext)
        {
            LogItem logItem = new LogItem();
            logItem.Detail = message;
            return Log(logItem, typeName, useContext);
        }
        public static bool Log(string message, string typeName)
        {
            return Log(message, typeName, true);
        }
        /// <summary>
        /// 指定日志类型名生成日志
        /// </summary>
        /// <param name="logItem"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool Log(LogItem logItem, string typeName)
        {
            return Log(logItem, typeName, true);
        }
        /// <summary>
        /// 指定日志类型名生成日志
        /// </summary>
        /// <param name="logItem"></param>
        /// <param name="typeName"></param>
        /// <param name="useContext">是否使用当前上下文信息</param>
        /// <returns></returns>
        public static bool Log(LogItem logItem, string typeName, bool useContext)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(typeName))
            {
                fileName += "." + typeName;
            }
            HttpContext context = HttpContext.Current;
            logItem.Time = DateTime.Now;

            if (context != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(_thisDomain))
                    {
                        _thisDomain = context.Request.Url.Host;
                    }
                    if (UseContext)
                    {
                        if (useContext)
                        {
                            logItem.HostIP = RequestHelper.GetCdnIP();

                            logItem.RequestUrl = context.Request.Url.ToString();
                            logItem.UserAgent = context.Request.UserAgent;
                            logItem.UrlReferrer = context.Request.UrlReferrer + "";
                        }
                    }
                }
                catch
                {
                }
            }
            return WriteLog(GetLogFolder(), logItem, fileName);
        }
        /// <summary>
        /// 生成日志,默认文件名
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sendToServer">是否发送到服务器</param>
        /// <returns></returns>
        public static bool Log(string message, bool sendToServer)
        {
            //if (sendToServer)
            //{
            //    SendToServer(message, "");
            //}
            return WriteLog(message);
        }
        /// <summary>
        /// 生成日志,默认文件名
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Log(string message)
        {
            return WriteLog(message);
        }
        /// <summary>
        /// 生成日志,文件名以Error开头
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Error(string message)
        {
            return Log(message, "Error");
        }
        /// <summary>
        /// 生成日志,文件名以Info开头
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Info(string message)
        {
            return Log(message, "Info");
        }
        /// <summary>
        /// 生成日志,文件名以Debug开头
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Debug(string message)
        {
            return Log(message, "Debug");
        }
        /// <summary>
        /// 在当前网站目录生成日志
        /// </summary>
        /// <param name="message"></param>
        public static bool WriteLog(string message)
        {
            return Log(message, "");
        }


        static bool Writing = false;
        //static DateTime lastWriteTime = DateTime.Now;
        static Dictionary<string, LogItemArry> logCaches = new Dictionary<string, LogItemArry>();
        static System.Timers.Timer timer;
        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Writing)
            {
                WriteLogFromCache();
            }

        }
        /// <summary>
        /// 指定路径,文件名,写入日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="logItem"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool WriteLog(string path, LogItem logItem, string fileName)//建立日志
        {
            try
            {
                if (timer == null)
                {
                    timer = new System.Timers.Timer();
                    timer.Interval = 2000;
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                    timer.Start();
                }
                if (!System.IO.Directory.Exists(path))
                    CreateFolder(path);
                string filePath = "";

                filePath = path + fileName + ".txt";

                if (!logCaches.ContainsKey(filePath))
                    logCaches.Add(filePath, new LogItemArry() { savePath = filePath });

                logCaches[filePath].Add(logItem);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string LastError;
        public static void WriteLogFromCache()
        {
            lock (lockObj)
            {
                Writing = true;
                //累加上次记录的日志
                if (logCaches.Count > 0)
                {
                    Dictionary<string, LogItemArry> list = new Dictionary<string, LogItemArry>(logCaches);
                    foreach (KeyValuePair<string, LogItemArry> entry in list)
                    {
                        LogItemArry logitemArry = entry.Value;
                        LastError = null;
                        try
                        {
                            WriteLine(logitemArry.ToString(), entry.Key);
                        }
                        catch (Exception ero)
                        {
                            LastError = ero.ToString();
                        }
                        logCaches.Remove(entry.Key);
                    }
                }
                //System.Threading.Thread.Sleep(6000);
                Writing = false;
            }
        }


        /// <summary>
        /// 写信息到指定文件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        private static void WriteLine(string message, string filePath)
        {

            using (FileStream fs = File.OpenWrite(filePath))
            {
                //根据上面创建的文件流创建写数据流
                StreamWriter w = new StreamWriter(fs, System.Text.Encoding.Default);
                //设置写数据流的起始位置为文件流的末尾
                w.BaseStream.Seek(0, SeekOrigin.End);
                //w.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                w.Write(message);
                w.Flush();
                w.Close();
            }
            Console.WriteLine(message);
        }
        static string secondFolder = null;
        /// <summary>
        /// 获取日志二级目录
        /// </summary>
        /// <returns></returns>
        public static string GetSecondFolder()
        {
            if (secondFolder == null)
            {
                string address = RequestHelper.GetServerIp();

                string[] arry = address.Split('.');
                secondFolder = arry[arry.Length - 1];
            }
            return secondFolder;
        }
        static string rootPath = null;
        /// <summary>
        /// 获取日志绝对目录
        /// </summary>
        /// <returns></returns>
        public static string GetLogFolder()
        {
            if (rootPath == null)
            {
                rootPath = System.Web.Hosting.HostingEnvironment.MapPath(@"\log\");
                if (string.IsNullOrEmpty(rootPath))
                {
                    rootPath = AppDomain.CurrentDomain.BaseDirectory + @"\log\";
                }
                rootPath += GetSecondFolder() + @"\";
                //如果节点有设置,则按节点设置读取
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                string settingPath = appSettings["EventLogFolder"];
                if (!string.IsNullOrEmpty(settingPath))
                {
                    rootPath = settingPath + @"\";
                }
            }
            return rootPath;
        }


        /// <summary>
        /// 项集合
        /// </summary>
        public class LogItemArry
        {
            public string savePath;
            List<LogItem> logs = new List<LogItem>();
            public void Add(LogItem log)
            {
                logs.Add(log);
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (LogItem item in logs)
                {
                    sb.Append(item.ToString());
                }
                return sb.ToString() + "\r\n";
            }
        }
    }
}
