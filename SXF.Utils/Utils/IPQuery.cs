using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace SXF.Utils
{
    // 任何语言都通用的IP地址转数字方法
    //a.b.c.d ==> a*256*256*256+b*256*256+c*256+d ===> 256*(c+256*(b+256* a))+d
    /// <summary>
    /// IP归属地查询类
    /// </summary>
    public sealed class IPQuery
    {
        /// <summary>
        /// ip地址转长整数
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long IpToLong(String ip)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(ip);
            byte[] bytes = ipaddress.GetAddressBytes();
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// 长整数转ip
        /// </summary>
        /// <param name="ipLong"></param>
        /// <returns></returns>
        public static String LongToIp(long ipLong)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(ipLong.ToString());
            return ipaddress.ToString();
        }

    }
}

