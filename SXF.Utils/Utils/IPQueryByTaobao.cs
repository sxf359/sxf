using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SXF.Utils
{
    /// <summary>
    /// 通过淘宝提供的接口查询IP所属地区,ip地址库只能精确到市一级。县级的统一归到市一级
    /// </summary>
    public class IPQueryByTaobao
    {
        private string url = "http://ip.taobao.com/service/getIpInfo.php?ip=";

        public TaobaoData GetArea(string ip)
        {
            string areaJson;
            TaobaoData theData;
            try
            {
                WebRequest request = WebRequest.Create(url + ip);
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                    {
                        areaJson = reader.ReadToEnd();
                        theData = JSONHelper.JSONToObject<TaobaoData>(areaJson);
                        if (theData.code == "1")  //表示获取数据失败
                        {
                            return null;
                        }
                        return theData;
                    }

                }
            }
            catch
            {
                return null;
            }

        }

    }


    /// <summary>
    /// ip数据
    /// </summary>
    public class IPData
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        public string country_id { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string area { get; set; }
        public string area_id { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string region { get; set; }
        public string region_id { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }
        public string city_id { get; set; }
        /// <summary>
        /// 县
        /// </summary>
        public string county { get; set; }
        public string county_id { get; set; }
        /// <summary>
        /// isp
        /// </summary>
        public string isp { get; set; }
        public string isp_id { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string ip { get; set; }
    }
    /// <summary>
    /// 淘宝数据
    /// </summary>
    public class TaobaoData
    {
        public string code { get; set; }
        public IPData data { get; set; }
    }
}
