using System;
using System.Text;
using System.IO;
using System.Net;

namespace SXF.Utils
{
    /// <summary>
    /// 手机短信发送
    /// </summary>
    public class Sms
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="token">用户身份标识</param>
        /// <param name="sPhone">要发送的手机号-暂不支持“,”分隔发送</param>
        /// <param name="sTempIndex">
        /// 模板：1-恭喜您购买成功，预定码666888，请您妥善保存该信息，凭预定码领取门票，祝您购物愉快！
        /// 2-恭喜您，注册成功，您的账号为：666888，感谢您对牛买网的支持！
        /// 3-您的验证码为666888，请妥善保管，使用后自动失效。
        /// 4-您好，您的预定码为666888的商品已经签收。
        /// 5-您的验证码为666888，请妥善保管，使用后自动过期。
        /// </param>
        /// <param name="sCode">要发送的验证码</param>
        /// <returns></returns>
        public static string sMsSend(string token, string sPhone, string sTempIndex, string sCode)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string postData = "token=" + token + "&sSphone=" + sPhone + "&sTempIndex=" + sTempIndex + "&sCode=" + sCode;
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://apix.nbbuy.com/sms/sms.ashx");
            myRequest.Method = "POST";
            myRequest.Timeout = 10000;
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            StreamReader sreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string read = sreader.ReadToEnd();
            return read;
        }
    }
}
