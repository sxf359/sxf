using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using System.Drawing;

namespace SXF.Utils.ThoughtWorks
{
    /// <summary>
    /// 二维码处理类
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Bitmap CreateQRCode(string str)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //设置编码测量度  
            qrCodeEncoder.QRCodeScale = 4;// (值越大生成的二维码图片像素越高)
                                          //设置编码版本  
            qrCodeEncoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            //设置编码错误纠正  
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M; //错误效验、错误更正(有4个等级：H-L-M-Q)
            Bitmap bmp = qrCodeEncoder.Encode(str, Encoding.UTF8);
            return bmp;
        }
        /// <summary>
        /// 获取二维码中的字符串信息
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string QRDecodeTxt(Image img)
        {

            string text = null;

            try

            {

                QRCodeDecoder decoder = new QRCodeDecoder();

                QRCodeImage qrimg = new QRCodeBitmapImage(new Bitmap(img));

                text = decoder.decode(qrimg, Encoding.UTF8);

            }

            catch

            {

                EventLog.WriteLog("无法识别到二维码");

            }

            return text;

        }
    }
}
