​using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace SXF.Utils
{
    /// <summary>
    /// 字符集类型
    /// </summary>
    public enum CharacterType
    {
        CharacterNumber=1,    //纯数字
        CharacterASCII=2      //ascii字符集
    }

    /// <summary>
    /// 生成图片验证码类
    /// </summary>
    public class VerifyCode
    {
        /// <summary>
        /// acsii验证码的字符集
        /// </summary>
        private static char[] character ={'1', '2', '3', '4', '5', '6', '8', '9','0', 'A', 'B', 'C', 'D', 'E',
'F', 'G', 'H','I', 'J', 'K', 'L', 'M', 'N','O', 'P','Q', 'R', 'S', 'T','U','V', 'W', 'X', 'Y','Z','a','b','c','d','e','f','g',
                              'h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
       
        /// <summary>
        /// 纯数字验证码的字符集
        /// </summary>
        private static char[] characterNumber = { '1', '2', '3', '4', '5', '6', '8', '9', '0' };

        #region  字符验证码
        /// <summary>
        /// 按默认的宽高，噪点等级，字体大小生成ASCII图片验证码
        /// </summary>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCode()
        {
            return MemoryVerifyCode(70, 25);
        }
        /// <summary>
        /// 按默认值的噪点等级，字体大小生成ASCII图片验证码
        /// </summary>
        /// <param name="imgWidth">图片宽度px</param>
        /// <param name="imgHeight">图片高度px</param>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCode(int imgWidth, int imgHeight)
        {
            return MemoryVerifyCode(imgWidth, imgHeight, 16);
        }

        /// <summary>
        /// 按默认值的噪点等级生成ASCII图片验证码
        /// </summary>
        /// <param name="imgWidth">图片宽度px</param>
        /// <param name="imgHeight">图片高度px</param>
        /// <param name="fontSize">字体大小px</param>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCode(int imgWidth, int imgHeight, float fontSize)
        {
            return MemoryVerifyCode(imgWidth, imgHeight, fontSize, 100,CharacterType.CharacterASCII);
        }
        #endregion

        #region  数字验证码
        /// <summary>
        /// 按默认的宽高，噪点等级，字体大小生成纯数字图片验证码
        /// </summary>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCodeByNumber()
        {
            return MemoryVerifyCodeByNumber(70, 25);
        }
        /// <summary>
        /// 按默认的噪点等级，字体大小生成纯数字图片验证码
        /// </summary>
        /// <param name="imgWidth">图片宽度px</param>
        /// <param name="imgHeight">图片高度px</param>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCodeByNumber(int imgWidth, int imgHeight)
        {
            return MemoryVerifyCodeByNumber(imgWidth, imgHeight, 16);
        }

        /// <summary>
        /// 按默认值噪点等级生成纯数字图片验证码
        /// </summary>
        /// <param name="imgWidth">图片宽度px</param>
        /// <param name="imgHeight">图片高度px</param>
        /// <param name="fontSize">字体大小px</param>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCodeByNumber(int imgWidth, int imgHeight, float fontSize)
        {
            return MemoryVerifyCode(imgWidth, imgHeight, fontSize, 100, CharacterType.CharacterNumber);
        }
        #endregion

        /// <summary>
        /// 以指定的宽高，指定的字体大小，及指定的噪点等级生成ASCII验证码图片
        /// </summary>
        /// <param name="imgWidth">图片宽度px</param>
        /// <param name="imgHeight">图片高度px</param>
        /// <param name="fontSize">字体大小px</param>
        /// <param name="noise">噪点</param>
        /// <param name="characterType">字符集类型</param>
        /// <returns></returns>
        public static MemoryStream MemoryVerifyCode(int imgWidth, int imgHeight, float fontSize, int noise, CharacterType characterType)
        {
            string chkCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点
            Color[] color ={ Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange,

Color.Brown, Color.DarkBlue };

            //字体列表，用于验证码

            string[] font ={ "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh",

"PMingLiU", "Impact" };


            Random rnd = new Random();
            //生成验证码字符串
            for (int i = 0; i < 4; i++)
            {
                if (CharacterType.CharacterASCII == characterType)
                {
                    chkCode += character[rnd.Next(character.Length)];
                }
                else
                {
                    chkCode += characterNumber[rnd.Next(characterNumber.Length)];
                }
            }
            //保存验证码的Session
            HttpContext.Current.Session["CheckCode"] = chkCode.ToLower();

            //将验证码图片写入内存流，并将其以"image/Png" 格式输出
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(70, 25))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.Clear(Color.White);

                    //画验证码字符串
                    for (int i = 0; i < chkCode.Length; i++)
                    {
                        string fnt = font[rnd.Next(font.Length)];

                        using (Font ft = new Font(fnt, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                        {
                            Color clr = color[rnd.Next(color.Length)];
                            graphics.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), i * fontSize, (float)1);
                        }

                    }
                }
                //画噪点
                for (int i = 0; i < noise; i++)
                {
                    int x = rnd.Next(bmp.Width);
                    int y = rnd.Next(bmp.Height);
                    Color clr = color[rnd.Next(color.Length)];
                    bmp.SetPixel(x, y, clr);

                }


                bmp.Save(ms, ImageFormat.Jpeg);
            }

            return ms;
        }
    }
}


