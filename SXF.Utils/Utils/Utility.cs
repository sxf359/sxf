namespace SXF.Utils
{
    using Microsoft.JScript;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    /// <summary>
    /// 工具类,对常用方法重新进行封装，及获取一些常用环境变量
    /// </summary>
    public class Utility
    {
        //创建一个随机数实例
        private static Random ro = new Random();

        /// <summary>
        /// 程序集版本
        /// </summary>
        public const string ASSEMBLY_VERSION = "4.0.0";
        /// <summary>
        /// 把动态页面转换成静态页面并输出
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outPath"></param>
        public static void Aspx2XHtml(string path, string outPath)
        {
            FileStream stream;
            Page page = new Page();
            StringWriter writer = new StringWriter();
            page.Server.Execute(path, writer);
            if (System.IO.File.Exists(page.Server.MapPath(outPath)))
            {
                System.IO.File.Delete(page.Server.MapPath(outPath));
                stream = System.IO.File.Create(page.Server.MapPath(outPath));
            }
            else
            {
                stream = System.IO.File.Create(page.Server.MapPath(outPath));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(writer.ToString());
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }
        /// <summary>
        /// 清空客户端浏览器的缓存,设置页面不被缓存
        /// </summary>
        public static void ClearPageClientCache()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.Expires = 0;
                HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1.0);
                HttpContext.Current.Response.AddHeader("pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("cache-control", "private");
                HttpContext.Current.Response.CacheControl = "no-cache";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(true);
                HttpContext.Current.Response.Cookies.Clear();
            }
        }
        /// <summary>
        /// 设置页面不被缓存
        /// </summary>
        public static void SetPageNoCache()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
                HttpContext.Current.Response.Expires = 0;
                HttpContext.Current.Response.CacheControl = "no-cache";
                HttpContext.Current.Response.AddHeader("Pragma", "No-Cache");
            }
        }
        /// <summary>
        /// 字符转数字版本
        /// </summary>
        /// <param name="strVersion"></param>
        /// <returns></returns>
        public static int ConvertVersionStr2Int(string strVersion)
        {
            if (!Validate.IsIP(strVersion))
            {
                return 0;
            }
            string[] strArray = strVersion.Split(new char[] { '.' });
            return ((((System.Convert.ToInt32(strArray[0]) << 0x18) | (System.Convert.ToInt32(strArray[1]) << 0x10)) | (System.Convert.ToInt32(strArray[2]) << 8)) | System.Convert.ToInt32(strArray[3]));
        }
        /// <summary>
        ///  将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static StringBuilder DataTableToJSON(DataTable dt)
        {
            return DataTableToJson(dt, true);
        }
        /// <summary>
        /// 将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt">要转换的数据表</param>
        /// <param name="dtDispose">数据表转换结束后是否dispose掉</param>
        /// <returns></returns>
        public static StringBuilder DataTableToJson(DataTable dt, bool dtDispose)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[\r\n");
            string[] strArray = new string[dt.Columns.Count];
            int index = 0;
            string format = "{{";
            string str2 = "";
            foreach (DataColumn column in dt.Columns)
            {
                object obj2;
                strArray[index] = column.Caption.ToLower().Trim();
                format = format + "'" + column.Caption.ToLower().Trim() + "':";
                str2 = column.DataType.ToString().Trim().ToLower();
                if ((((str2.IndexOf("int") > 0) || (str2.IndexOf("deci") > 0)) || ((str2.IndexOf("floa") > 0) || (str2.IndexOf("doub") > 0))) || (str2.IndexOf("bool") > 0))
                {
                    obj2 = format;
                    format = string.Concat(new object[] { obj2, "{", index, "}" });
                }
                else
                {
                    obj2 = format;
                    format = string.Concat(new object[] { obj2, "'{", index, "}'" });
                }
                format = format + ",";
                index++;
            }
            if (format.EndsWith(","))
            {
                format = format.Substring(0, format.Length - 1);
            }
            format = format + "}},";
            index = 0;
            object[] args = new object[strArray.Length];
            foreach (DataRow row in dt.Rows)
            {
                foreach (string str3 in strArray)
                {
                    args[index] = row[strArray[index]].ToString().Trim().Replace(@"\", @"\\").Replace("'", @"\'");
                    string str4 = args[index].ToString();
                    if (str4 != null)
                    {
                        if (!(str4 == "True"))
                        {
                            if (str4 == "False")
                            {
                                goto Label_028E;
                            }
                        }
                        else
                        {
                            args[index] = "true";
                        }
                    }
                    goto Label_029C;
                Label_028E:
                    args[index] = "false";
                Label_029C:
                    index++;
                }
                index = 0;
                builder.Append(string.Format(format, args));
            }
            if (builder.ToString().EndsWith(","))
            {
                builder.Remove(builder.Length - 1, 1);
            }
            if (dtDispose)
            {
                dt.Dispose();
            }
            return builder.Append("\r\n];");
        }


        public static string escape(string str)
        {
            return GlobalObject.escape(str);
        }
        /*
        /// <summary>
        /// 以 32 位 MD5 加密加CookieToken后缀的形式产生 cookie 密文
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GenerateToken(string s)
        {
            if ((s == null) || (0 == s.Length))
            {
                s = string.Empty;
            }
            return MD5(s + ApplicationSettings.Get("CookieToken"));
        }*/
        /// <summary>
        ///  32 位 MD5 加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(string s)
        {
            return TextEncrypt.MD5EncryptPassword(s);
        }
        /// <summary>
        /// 获取web.config的配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return ApplicationSettings.Get(key);
        }

        /*public static string GetAssemblyCopyright()
        {
            return ApplicationEnvironment.GetAssemblyCopyright();
        }

        public static string GetAssemblyProductName()
        {
            return ApplicationEnvironment.GetAssemblyProductName();
        }

        public static string GetAssemblyVersion()
        {
            return ApplicationEnvironment.GetAssemblyVersion();
        }
        */
        /*
         /// <summary>
         /// 取CPU编号
         /// </summary>
         /// <returns></returns>
         public static string GetCpuID()
         {
             ManagementClass class2 = new ManagementClass("Win32_Processor");
             string s = "";
             try
             {
                 ManagementObjectCollection instances = class2.GetInstances();
                 foreach (ManagementObject obj2 in instances)
                 {
                     s = obj2.Properties["ProcessorId"].Value.ToString();
                     goto Label_008F;
                 }
             }
             catch
             {
                 s = "";
             }
             finally
             {
                 class2.Dispose();
             }
         Label_008F:
             return BitConverter.ToString(Encoding.GetEncoding("GB2312").GetBytes(s)).Replace("-", "");
         }
         /// <summary>
         /// 取第一块硬盘编号
         /// </summary>
         /// <returns></returns>
         public static string GetHardDiskID()
         {
             string s = "";
             ManagementClass class2 = new ManagementClass("Win32_DiskDrive");
             ManagementObjectCollection instances = class2.GetInstances();
             foreach (ManagementObject obj2 in instances)
             {
                 s = obj2.Properties["Model"].Value.ToString();
                 obj2.Dispose();
             }
             class2.Dispose();
             instances.Dispose();
             return BitConverter.ToString(Encoding.GetEncoding("GB2312").GetBytes(s)).Replace("-", "");
         }
         */
        /// <summary>
        /// 取机器名
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
        /*
        public static string GetIPAddressByMac(string macAddress)
        {
            List<LocalIPAndMac> localIPAndMac = GetLocalIPAndMac();
            foreach (LocalIPAndMac mac in localIPAndMac)
            {
                if (string.Compare(mac.MACAddress, macAddress, true) == 0)
                {
                    return mac.IPAddress;
                }
            }
            return "";
        }

        public static List<LocalIPAndMac> GetLocalIPAndMac()
        {
            List<LocalIPAndMac> list = new List<LocalIPAndMac>();
            ManagementObjectCollection instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
            foreach (ManagementObject obj2 in instances)
            {
                try
                {
                    if ((bool) obj2["IPEnabled"])
                    {
                        string mac = obj2["MacAddress"].ToString().Replace(':', '-');
                        Array array = (Array) obj2.Properties["IpAddress"].Value;
                        string ip = array.GetValue(0).ToString();
                        list.Add(new LocalIPAndMac(ip, mac));
                    }
                }
                catch
                {
                }
                obj2.Dispose();
            }
            return list;
        }

        public static string GetMACAddress()
        {
            string str = " ";
            ManagementClass class2 = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection instances = class2.GetInstances();
            foreach (ManagementObject obj2 in instances)
            {
                if ((bool) obj2["IPEnabled"])
                {
                    str = obj2["MacAddress"].ToString();
                }
                obj2.Dispose();
            }
            class2.Dispose();
            instances.Dispose();
            return str.Replace(":", "");
        }

        public static string GetMACByIPAddress(string ipAddress)
        {
            List<LocalIPAndMac> localIPAndMac = GetLocalIPAndMac();
            foreach (LocalIPAndMac mac in localIPAndMac)
            {
                if (string.Compare(mac.IPAddress, ipAddress, true) == 0)
                {
                    return mac.MACAddress;
                }
            }
            return "";
        }

        public static string GetMachineSerial()
        {
            return GetMACAddress();
        }
        */
        /// <summary>
        /// 获取操作系统版本
        /// </summary>
        /// <returns></returns>
        public static string GetOSVersion()
        {
            string s = Environment.OSVersion.Version.ToString();
            return BitConverter.ToString(Encoding.GetEncoding("GB2312").GetBytes(s)).Replace("-", "");
        }
        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            Regex regex = new Regex("</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);
            return regex.Replace(HTML, "");
        }
        /// <summary>
        /// 长整型转换成IP地址字符串形式
        /// </summary>
        /// <param name="ipNumber"></param>
        /// <returns></returns>
        public static string Int2IP(long ipNumber)
        {
            IPAddress address = new IPAddress(ipNumber);
            return address.ToString();
        }
        /// <summary>
        /// IP 地址字符串形式转换成长整型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long IP2Int(string ip)
        {
            if (!Validate.IsIP(ip))
            {
                return -1L;
            }
            string[] strArray = ip.Split(new char[] { '.' });
            long num = long.Parse(strArray[3]) * 0x1000000L;
            num += int.Parse(strArray[2]) * 0x10000;
            num += int.Parse(strArray[1]) * 0x100;
            return (num + int.Parse(strArray[0]));
        }
        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            return TypeParse.IsNumericArray(strNumber);
        }



        /*
        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="cipHerBuffer"></param>
        /// <param name="cipherKey"></param>
        /// <returns></returns>
        public static object ReceiveDataLaunch(byte[] cipHerBuffer, string cipherKey)
        {
            return SerializationHelper.Deserialize(CompressHelper.DeflateDecompress(AES.DecryptBuffer(cipHerBuffer, cipherKey)));
        }
        /// <summary>
        /// 发送数据处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="cipherKey"></param>
        /// <returns></returns>
        public static byte[] SendDataLaunch<T>(T t, string cipherKey)
        {
            return AES.EncryptBuffer(CompressHelper.DeflateCompress(SerializationHelper.Serialize<T>(t)), cipherKey);
        }
        */
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="url"></param>
        public static void Redirect(string url)
        {
            if ((HttpContext.Current != null) && !string.IsNullOrEmpty(url))
            {
                HttpContext.Current.Response.Redirect(url);
                HttpContext.Current.Response.StatusCode = 0x12d;
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 以指定的ContentType输出指定文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">输出的文件名</param>
        /// <param name="filetype">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filepath, string filename, string filetype)
        {
            if (HttpContext.Current != null)
            {
                Stream stream = null;
                byte[] buffer = new byte[0x2710];
                try
                {
                    stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    long length = stream.Length;
                    HttpContext.Current.Response.ContentType = filetype;
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + UrlEncode(filename.Trim()).Replace("+", " "));
                    while (length > 0L)
                    {
                        if (HttpContext.Current.Response.IsClientConnected)
                        {
                            int count = stream.Read(buffer, 0, 0x2710);
                            HttpContext.Current.Response.OutputStream.Write(buffer, 0, count);
                            HttpContext.Current.Response.Flush();
                            buffer = new byte[0x2710];
                            length -= count;
                        }
                        else
                        {
                            length = -1L;
                        }
                    }
                }
                catch (Exception exception)
                {
                    HttpContext.Current.Response.Write("Error : " + exception.Message);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                HttpContext.Current.Response.End();
            }
        }
        /// <summary>
        /// 查找非 UTF8 编码的文件
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string[] SearchUTF8File(string directory)
        {
            StringBuilder builder = new StringBuilder();
            FileInfo[] files = new DirectoryInfo(directory).GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension.ToLower().Equals(".htm"))
                {
                    FileStream sbInputStream = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read);
                    bool flag = IsUTF8(sbInputStream);
                    sbInputStream.Close();
                    if (!flag)
                    {
                        builder.Append(files[i].FullName);
                        builder.Append("\r\n");
                    }
                }
            }
            return TextUtility.SplitStrArray(builder.ToString(), "\r\n");
        }
        /// <summary>
        /// 是否为 UTF8 编码
        /// </summary>
        /// <param name="sbInputStream">文件输入流</param>
        /// <returns>是返回 true,否则 false</returns>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            bool flag = true;
            long length = sbInputStream.Length;
            byte num2 = 0;
            for (int i = 0; i < length; i++)
            {
                byte num4 = (byte)sbInputStream.ReadByte();
                if ((num4 & 0x80) != 0)
                {
                    flag = false;
                }
                if (num2 == 0)
                {
                    if (num4 >= 0x80)
                    {
                        do
                        {
                            num4 = (byte)(num4 << 1);
                            num2 = (byte)(num2 + 1);
                        }
                        while ((num4 & 0x80) != 0);
                        num2 = (byte)(num2 - 1);
                        if (num2 == 0)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((num4 & 0xc0) != 0x80)
                    {
                        return false;
                    }
                    num2 = (byte)(num2 - 1);
                }
            }
            if (num2 > 0)
            {
                return false;
            }
            if (flag)
            {
                return false;
            }
            return true;
        }

        #region 类型转换
        public static bool StrToBool(object expression, bool defValue)
        {
            return TypeParse.StrToBool(expression, defValue);
        }

        public static bool StrToBool(string expression, bool defValue)
        {
            return TypeParse.StrToBool(expression, defValue);
        }

        public static float StrToFloat(object strValue, float defValue)
        {
            return TypeParse.StrToFloat(strValue, defValue);
        }

        public static float StrToFloat(string strValue, float defValue)
        {
            return TypeParse.StrToFloat(strValue, defValue);
        }

        public static int StrToInt(object expression, int defValue)
        {
            return TypeParse.StrToInt(expression, defValue);
        }

        public static int StrToInt(string expression, int defValue)
        {
            return TypeParse.StrToInt(expression, defValue);
        }
        #endregion
        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int num;
            int num2;
            char[] chArray;
            int blue = 0;
            color = color.TrimStart(new char[] { '#' });
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    chArray = color.ToCharArray();
                    num = System.Convert.ToInt32(chArray[0].ToString() + chArray[0].ToString(), 0x10);
                    num2 = System.Convert.ToInt32(chArray[1].ToString() + chArray[1].ToString(), 0x10);
                    blue = System.Convert.ToInt32(chArray[2].ToString() + chArray[2].ToString(), 0x10);
                    return Color.FromArgb(num, num2, blue);

                case 6:
                    chArray = color.ToCharArray();
                    num = System.Convert.ToInt32(chArray[0].ToString() + chArray[1].ToString(), 0x10);
                    num2 = System.Convert.ToInt32(chArray[2].ToString() + chArray[3].ToString(), 0x10);
                    blue = System.Convert.ToInt32(chArray[4].ToString() + chArray[5].ToString(), 0x10);
                    return Color.FromArgb(num, num2, blue);
            }
            return Color.FromName(color);
        }

        #region 跟踪调试
        /// <summary>
        /// 跟踪调试输出一个对象
        /// </summary>
        /// <param name="obj"></param>
        public static void Trace(object obj)
        {
            string format = "<div style='border:1px solid #96C2F1;background-color: #F7F7FF;font-size:14px;font-family:宋体;text-align:right;margin: 0px auto;margin-bottom:5px;margin-right:5px;float:left; text-align:left; line-height:25px; width:800px;'><h5 style='margin: 1px;background-color:#E2EAF8;height: 24px;'>跟踪信息：</h5>{0}</div>";
            HttpContext.Current.Response.Write(string.Format(format, obj.ToString()));
        }
        /// <summary>
        /// 跟踪调试输出一个对象,不加修饰
        /// </summary>
        /// <param name="obj"></param>
        public static void TraceWhite(object obj)
        {
            HttpContext.Current.Response.Write(obj.ToString());
        }
        #endregion

        public static string unescape(string str)
        {
            return GlobalObject.unescape(str);
        }

        public static string Url2HyperLink(string text)
        {
            string pattern = @"(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])";
            MatchCollection matchs = Regex.Matches(text, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match match in matchs)
            {
                text = text.Replace(match.ToString(), "<a target=\"_blank\" href=\"" + match.ToString() + "\">" + match.ToString() + "</a>");
            }
            return text;
        }
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        #region Cookie
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes((double)expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        public static string GetCookie(string strName)
        {
            if ((HttpContext.Current.Request.Cookies != null) && (HttpContext.Current.Request.Cookies[strName] != null))
            {
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }
            return "";
        }

        public static string GetCookie(string strName, string key)
        {
            if (((HttpContext.Current.Request.Cookies != null) && (HttpContext.Current.Request.Cookies[strName] != null)) && (HttpContext.Current.Request.Cookies[strName][key] != null))
            {
                return HttpContext.Current.Request.Cookies[strName][key].ToString();
            }
            return "";
        }
        #endregion

        public static string CurrentPath
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                string path = HttpContext.Current.Request.Path;
                path = path.Substring(0, path.LastIndexOf("/"));
                if (path == "/")
                {
                    return string.Empty;
                }
                return path;
            }
        }



        public static string GetAppLogDirectory
        {
            get
            {
                string fullPath = ConfigurationManager.AppSettings["AppLogDirectory"];
                if (string.IsNullOrEmpty(fullPath))
                {
                    fullPath = "AppLog";
                }
                fullPath = TextUtility.GetFullPath(fullPath);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                return fullPath;
            }
        }

        public static string GetIPDbFilePath
        {
            get
            {
                return ApplicationSettings.Get("IPDbFilePath");
            }
        }

        public static bool GetWriteAppLog
        {
            get
            {
                bool flag = false;
                string str = ConfigurationManager.AppSettings["WriteAppLog"];
                if (!string.IsNullOrEmpty(str))
                {
                    flag = System.Convert.ToBoolean(str);
                }
                return flag;
            }
        }



        /// <summary>
        /// right函数
        /// </summary>
        /// <param name="sSource"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static string Right(string sSource, int iLength)
        {
            if (iLength == -1)
            {
                return "";
            }
            return sSource.Substring(iLength > sSource.Length ? 0 : sSource.Length - iLength);
        }

        /// <summary>
        /// left函数
        /// </summary>
        /// <param name="sSource"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static string Left(string sSource, int iLength)
        {
            if (iLength == -1)
            {
                return "";
            }
            return sSource.Substring(0, iLength > sSource.Length ? sSource.Length : iLength);
        }

        #region 常用日期函数
        /// <summary>
        /// 上月第一天
        /// </summary>
        public static string PreMonthFirstDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 上月最后一天
        /// </summary>
        public static string PreMonthLastDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyy-MM-dd"); }
        }
        /// <summary>
        /// 上周一
        /// </summary>
        public static string PreWeekMonday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(-7).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 上周日
        /// </summary>
        public static string PreWeekSunday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 昨天
        /// </summary>
        public static string Yesterday
        {
            get { return DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd"); }
        }
        /// <summary>
        /// 今天
        /// </summary>
        public static string Today
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本周一
        /// </summary>
        public static string ThisWeekMonday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本周日
        /// </summary>
        public static string ThisWeekSunday
        {
            get { return DateTime.Now.AddDays(1 - System.Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(6).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本月第一天
        /// </summary>
        public static string ThisMonthFirstDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 本月最后一天
        /// </summary>
        public static string ThisMonthLastDay
        {
            get { return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 长日期转换为yyyy-mm-dd形式显示
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertDateToYYYYMMDD(object date)
        {
            if (date.IsNullOrEmpty())
            {
                return "";
            }
            return System.Convert.ToDateTime(date).ToString("yyyy-MM-dd");
        }
        #endregion


        /// <summary>
        /// 是否本地服务器
        /// </summary>
        /// <returns></returns>
        public static bool IsLocal()
        {
            string address = RequestHelper.GetServerIp();
            //本地不作处理
            if (address.Contains("192.168."))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据IP获取webconfig中的本地域名或者网站域名
        /// </summary>
        /// <returns></returns>
        public static string GetWebSiteDomain()
        {
            if (Utility.IsLocal())
            {
                return System.Configuration.ConfigurationManager.AppSettings["LocalDomain"].ToString();
            }
            else
            {
                return System.Configuration.ConfigurationManager.AppSettings["WebSiteDomain"].ToString();
            }
        }


        /// <summary>
        /// 时分秒+随机数字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNumber()
        {
            string orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            //获取一个大于1000小于9999的随机数
            string rnd = ro.Next(1000, 9999).ToString();
            orderNo += rnd;
            return orderNo;
        }
        /// <summary>
        /// 获取随机6位数
        /// </summary>
        /// <returns></returns>
        public static string GetRandomSix()
        {
            string rnd = ro.Next(100000, 999999).ToString();
            return rnd;
        }
        /// <summary>
        /// 根据url路径获取网页内容，url可能是外站url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetWebPageContent(string url)
        {

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                {
                    string xmlStr = reader.ReadToEnd();
                    return xmlStr;
                }


            }
        }
        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="iInterval">毫秒</param>
        public static void Delay(int iInterval)
        {
            DateTime now = DateTime.Now;
            while (now.AddMilliseconds(iInterval) > DateTime.Now)
            {
            }
            return;
        }
        /// <summary>
        /// 是否微信访问
        /// </summary>
        /// <returns></returns>
        public static bool IsMicroMessenger()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("MicroMessenger") >= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否android访问
        /// </summary>
        /// <returns></returns>
        public static bool IsAndroid()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("Android") >= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否iPhone访问
        /// </summary>
        /// <returns></returns>
        public static bool IsiPhone()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("iPhone") >= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        ///是否是android平板电脑
        /// </summary>
        /// <returns></returns>
        public static bool IsAndroidTablet()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("Android") >= 0 && userAgent.IndexOf("Mobile") == -1)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        ///是否是苹果平板电脑
        /// </summary>
        /// <returns></returns>
        public static bool IsiPad()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("iPad") >= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        ///是否是windows phone登录
        /// </summary>
        /// <returns></returns>
        public static bool IswindowsPhone()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("phone") >= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测是否手机端访问
        /// </summary>
        /// <returns></returns>
        public static bool IsPhoneEnd()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IsNullOrEmpty())
            {
                return false;
            }
            //EventLog.WriteLog(userAgent);
            //android手机访问
            if ((userAgent.IndexOf("Mobile") >= 0 && userAgent.IndexOf("Android") >= 0) || userAgent.IndexOf("iPhone") >= 0 || userAgent.IndexOf("Windows Phone") >= 0)
            {
                //EventLog.WriteLog("mobile access");
                return true;
            }
            return false;

        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 比较两个一维字符串数组是否相同，这个相同是指元素个数相等，并且值相同，但是元素位置可能不同
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static bool CompareArrayEqual(string[] arr1, string[] arr2)
        {
            if (arr1 == null || arr2 == null) return false;
            if (arr1.Length != arr2.Length) return false;
            Array.Sort(arr1);
            Array.Sort(arr2);
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}

