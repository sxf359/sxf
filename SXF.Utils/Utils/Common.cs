using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SXF.Utils
{
    public class Common
    {
        /// <summary>
        /// 获取一个字符串中某一字符或字符串的个数
        /// </summary>
        /// <param name="str">被搜索的字符串</param>
        /// <param name="regStr">要搜索的字符或字符串</param>
        /// <returns></returns>
        public static int MatcheCount(string str, string regStr)
        {
            Regex reg = new Regex(regStr, RegexOptions.Singleline);
            return reg.Matches(str).Count;
        }

        /// <summary>
        /// 从文件名中截取路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPathFromName(string fileName)
        {
            if (fileName == null || fileName == "")
            {
                return "";
            }
            return fileName.Substring(0, fileName.LastIndexOf("/") + 1);
        }

    }
}
