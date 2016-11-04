using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SXF.Utils
{
    public partial class StringHelper
    {
        #region 密码加密
        /// <summary>
        /// 对字符串进行MD5或SHA1加密操作
        /// </summary>
        /// <param name="cleanString">明文字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SysEncrypt(string cleanString, string strPasswordFormat)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(cleanString, strPasswordFormat);
        }

        #endregion

        /// <summary>
        /// 中英文加密
        /// </summary>
        /// <param name="instr">要加密的字体串</param>
        /// <returns></returns>
        public static string EncryptMD5(string instr)
        {
            string result;
            try
            {
                byte[] toByte = Encoding.Default.GetBytes(instr);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                toByte = md5.ComputeHash(toByte);
                result = BitConverter.ToString(toByte).Replace("-", "");
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 指定字节流编码计算MD5哈希值，可解决不同系统中文编码差异的问题。
        /// </summary>
        /// <param name="source">要进行哈希的字符串</param>
        /// <param name="bytesEncoding">获取字符串字节流的编码，如果是中文，不同系统之间务必请使用相同编码</param>
        /// <returns>32位大写MD5哈希值</returns>
        public static string ComputeMD5(string source, Encoding bytesEncoding)
        {
            byte[] sourceBytes = bytesEncoding.GetBytes(source);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] hashedBytes = md5.ComputeHash(sourceBytes);

            StringBuilder buffer = new StringBuilder(hashedBytes.Length);
            foreach (byte item in hashedBytes)
            {
                buffer.AppendFormat("{0:X2}", item);
            }

            return buffer.ToString();
        }

        #region 加密解密，可以设置密钥
        /// <summary>
        /// 可以设置密钥的加密方式
        /// </summary>
        /// <param name="sourceData">原文</param>
        /// <param name="key">密钥，8位数字，字符串方式</param>
        /// <returns></returns>
        public static string Encrypt(string sourceData, string key)
        {
            #region 检查密钥是否符合规定
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            #endregion

            Byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            Byte[] keys = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, key.Length));
            try
            {
                //convert data to byte array
                Byte[] sourceDataBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(sourceData);
                //get target memory stream
                MemoryStream tempStream = new MemoryStream();
                //get encryptor and encryption stream
                DESCryptoServiceProvider encryptor = new DESCryptoServiceProvider();
                CryptoStream encryptionStream = new CryptoStream(tempStream, encryptor.CreateEncryptor(keys, iv), CryptoStreamMode.Write);

                //encrypt data
                encryptionStream.Write(sourceDataBytes, 0, sourceDataBytes.Length);
                encryptionStream.FlushFinalBlock();

                //put data into byte array
                Byte[] encryptedDataBytes = tempStream.GetBuffer();
                //convert encrypted data into string
                return System.Convert.ToBase64String(encryptedDataBytes, 0, (int)tempStream.Length);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// 根据知道的密钥解密
        /// </summary>
        /// <param name="sourceData">密文</param>
        /// <param name="key">密钥，8位数字，字符串方式</param>
        /// <returns></returns>
        public static string Decrypt(string sourceData, string key)
        {
            #region 检查密钥是否符合规定
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            #endregion


            Byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            Byte[] keys = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, key.Length));
            try
            {
                Byte[] encryptedDataBytes = System.Convert.FromBase64String(sourceData);
                MemoryStream tempStream = new MemoryStream(encryptedDataBytes, 0, encryptedDataBytes.Length);
                DESCryptoServiceProvider decryptor = new DESCryptoServiceProvider();
                CryptoStream decryptionStream = new CryptoStream(tempStream, decryptor.CreateDecryptor(keys, iv), CryptoStreamMode.Read);
                StreamReader allDataReader = new StreamReader(decryptionStream);
                return allDataReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion


        [Obsolete("方法名写错了，请用ASP16MD5", false)]
        public static string ASP18MD5(string instr)
        {
            return EncryptMD5(instr).Substring(8, 16).ToLower();
        }

        /// <summary>
        /// 16位md5加密，返回小写
        /// </summary>
        /// <param name="instr">要加密的字体串</param>
        /// <returns></returns>
        public static string ASP16MD5(string instr)
        {
            return EncryptMD5(instr).Substring(8, 16).ToLower();
        }
    }
}
