using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;


namespace SXF.Utils
{
    /// <summary>
    /// 图片裁切模式
    /// </summary>
    public enum CutMode
    {
        /// <summary>
        /// 指定宽，高按比例   
        /// </summary>
        WIDTH = 0,
        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        HEIGHT = 1,
        /// <summary>
        /// 指定高宽缩放（可能变形）   
        /// </summary>
        WIDTH_HEIGHT = 3,
        /// <summary>
        /// 指定高宽裁减（不变形）
        /// </summary>
        CUT = 4,
        /// <summary>
        /// 自动
        /// </summary>
        AUTO = 5
    }

    /// <summary>
    /// 文件上传类
    /// </summary>
    public class UploadFile
    {
        #region 私有变量
        private static string watermarkpic = HttpContext.Current.Server.MapPath("/watermark/1.png");
        //允许的图片格式
        private static string allowExtentionName = ".gif|.jpg|.png";
        protected static string defpath = "/upload/a.linshi/";//添加水印时的临时图片存储路径
        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">原图片的PixelFormat</param>
        /// <returns></returns>
        private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断图片格式是否允许上传
        /// </summary>
        /// <param name="extentionname">图片扩展名</param>
        /// <returns>true:允许 false:不许</returns>
        private static bool allowPic(string extentionname)
        {
            return allowExtentionName.Contains(extentionname);
        }
        
        #endregion


        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="iswatermark">是否添加水印:0否 1是</param>
        /// <param name="fuUrl">上传控件</param>
        /// <param name="filePath">上传图片要存放的文件夹。格式为：/UploadImage/background/</param>
        /// <param name="msg">上传成功或失败的提示信息</param>
        /// <param name="imgName">上传成功后的文件名。若上传失败则为空</param>
        /// <returns>true:表示上传成功 false:上传失败</returns>
        public static bool UploadImage(int iswatermark, FileUpload fuUrl, string filePath, out string msg, out string imgName)
        {
            if (fuUrl.PostedFile.FileName.IsNullOrEmpty())
            {
                msg = "上传文件不能为空！";
                imgName = "";
                return false;
            }

            string filepath = fuUrl.PostedFile.FileName;                                //得到的是文件的完整路径,包括文件名
            string filename = filepath;                                 //得到上传的文件名20022775_m.jpg 


           
            string suffix = FileManager.GetFileExtension(filename);                                   //获得上传的图片的后缀名 

            if (!allowPic(suffix))
            {
                msg = "图片格式不正确，只能是jpg,gif,png格式";
                imgName = "";
                return false;
            }


            //上传的图片改名改名
            filename = Utility.GetRandomNumber() + suffix;
            string serverpath = HttpContext.Current.Server.MapPath(defpath + filename);//取得文件在服务器上保存的位置
            fuUrl.PostedFile.SaveAs(serverpath);//将上传的文件另存为 
            msg = "上传成功！";
            imgName = filename;
            //添加图片水印 
            string imageOldPath = HttpContext.Current.Server.MapPath(filePath + filename);
            if (iswatermark == 1 && File.Exists(watermarkpic))
            {
                AddShuiYinPic(serverpath, imageOldPath, watermarkpic);
            }

            return true;
        }

        /// <summary>
        /// 图片上传，限制图片大小和宽高
        /// </summary>
        /// <param name="fuUrl">上传控件</param>
        /// <param name="filePath">上传图片要存放的文件夹。格式为：/UploadImage/background/</param>
        /// <param name="limitSize">上传图片最大值以字节为单位 0:表示不限制上传图片大小</param>
        /// <param name="limitWidth">上传图片最大宽度 0:表示不限制上传图片宽度</param>
        /// <param name="limitHeight">上传图片最大高度 0:表示不限制上传图片高度</param>
        /// <param name="msg">上传成功或失败的提示信息</param>
        /// <param name="imgName">上传成功后的文件名。若上传失败则为空</param>
        /// <returns>true:表示上传成功 false:上传失败</returns>
        public static bool UploadImage(int iswatermark, FileUpload fuUrl, string filePath, int limitSize, int limitWidth, int limitHeight, out string msg, out string imgName)
        {
            if (fuUrl.PostedFile.FileName.IsNullOrEmpty())
            {
                msg = "上传图片不能为空！";
                imgName = "";
                return false;
            }
            if (limitSize > 0)
            {
                if (fuUrl.PostedFile.ContentLength > limitSize)
                {
                    msg = "上传图片不能超过" + limitSize / 1024 + "M字节！";
                    imgName = "";
                    return false;
                }
            }

            string filepath = fuUrl.PostedFile.FileName;                                //得到的是文件的完整路径,包括文件名
            string filename = filepath;       //得到上传的文件名20022775_m.jpg 


            string suffix = FileManager.GetFileExtension(filename);                                   //获得上传的图片的后缀名 
            if (!allowPic(suffix))
            {
                msg = "图片格式不正确，只能是jpg,gif,png格式";
                imgName = "";
                return false;
            }
            if (limitWidth > 0 || limitHeight > 0)    //表示限制了宽高
            {
                using (Stream stream = fuUrl.PostedFile.InputStream)
                {
                    using (System.Drawing.Image img = System.Drawing.Image.FromStream(stream))
                    {
                        if (limitWidth > 0 && img.Width > limitWidth)
                        {
                            msg = "上传图片宽度不能超过" + limitWidth + "px！";
                            imgName = "";
                            return false;
                        }

                        if (limitHeight > 0 && img.Height > limitHeight)
                        {
                            msg = "上传图片高度不能超过" + limitHeight + "px！";
                            imgName = "";
                            return false;
                        }

                        stream.Close();
                    }
                }
            }
            //上传的图片改名改名
            filename = Utility.GetRandomNumber() + suffix;

            string serverpath = HttpContext.Current.Server.MapPath(filePath + filename);//取得文件在服务器上保存的位置
            fuUrl.PostedFile.SaveAs(serverpath);//将上传的文件另存为 
            msg = "上传成功！";
            imgName = filename; ;
            //添加图片水印 
            string imageOldPath = HttpContext.Current.Server.MapPath(filePath + filename);
            //EventLog.WriteLog(serverpath);
            //EventLog.WriteLog(imageOldPath);
            if (iswatermark == 1 && File.Exists(watermarkpic))
            {
                AddShuiYinPic(serverpath, imageOldPath, watermarkpic);
                //AddShuiYinWord(serverpath, imageOldPath);
            }

            return true;
        }

        /// <summary>
        /// 图片上传，按照限制的图片宽高剪切
        /// </summary>
        /// <param name="fuUrl">上传控件</param>
        /// <param name="filePath">上传图片要存放的文件夹。格式为：/UploadImage/background/</param>
        /// <param name="limitWidth">上传图片最大宽度 0:表示不限制上传图片宽度</param>
        /// <param name="limitHeight">上传图片最大高度 0:表示不限制上传图片高度</param>
        /// <param name="msg">上传成功或失败的提示信息</param>
        /// <param name="imgName">上传成功后的文件名。若上传失败则为空</param>
        /// <returns>true:表示上传成功 false:上传失败</returns>
        public static bool UploadImage(FileUpload fuUrl, string filePath, int limitWidth, int limitHeight, out string msg, out string imgName)
        {
            if (fuUrl.PostedFile.FileName.IsNullOrEmpty())
            {
                msg = "上传图片不能为空！";
                imgName = "";
                return false;
            }
           
            string filepath = fuUrl.PostedFile.FileName;                                //得到的是文件的完整路径,包括文件名
            string filename = filepath;       //得到上传的文件名20022775_m.jpg 


            string suffix = FileManager.GetFileExtension(filename);                                   //获得上传的图片的后缀名 
            if (!allowPic(suffix))
            {
                msg = "图片格式不正确，只能是jpg,gif,png格式";
                imgName = "";
                return false;
            }
           
            //上传的图片改名改名
            filename = Utility.GetRandomNumber() + suffix;
            string serverpath = HttpContext.Current.Server.MapPath(filePath + filename);//取得文件在服务器上保存的位置


            using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(fuUrl.PostedFile.InputStream))
            {
                //图片裁切
                using (System.Drawing.Image customerImage = UploadFile.CutImg(originalImage, limitWidth, limitHeight, CutMode.WIDTH, true))
                {
                    //EventLog.WriteLog(customerImage.Width.ToString());
                    //图片保存
                    customerImage.Save(serverpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

            }
            msg = "上传成功！";
            imgName = filePath + filename; 
            return true;
        }



        /// <summary>
        /// 生成缩略图，起作用的
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">设置缩略图宽度</param>
        /// <param name="height">设置缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath))
            {
                int towidth = width;
                int toheight = height;
                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;
                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "W"://指定宽，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }
                //新建一个bmp图片
                using (System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight))
                {
                    //新建一个画板
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        //设置高质量插值法
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        //设置高质量,低速度呈现平滑程度
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //清空画布并以透明背景色填充
                        g.Clear(System.Drawing.Color.Transparent);
                        //在指定位置并且按指定大小绘制原图片的指定部分
                        g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                            new System.Drawing.Rectangle(x, y, ow, oh),
                            System.Drawing.GraphicsUnit.Pixel);

                        //以jpg格式保存缩略图
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);


                    }
                }
            }
        }

        /// <summary>
        /// 生成定制宽高的图片
        /// </summary>
        /// <param name="originalImagePath">原始图片路径</param>
        /// <param name="thumbnailPath">定制图片路径</param>
        /// <param name="width">定制宽度</param>
        /// <param name="height">定制高度</param>
        /// <param name="mode">图片裁切模式</param>
        /// <param name="HightMode">图片质量</param>
        public static void MakeCutImg(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath))
            {
                //图片裁切
                using (System.Drawing.Image customerImage = CutImg(originalImage, width, height, mode))
                {
                    //EventLog.WriteLog(customerImage.Width.ToString());
                    //图片保存
                    customerImage.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

            }
        }
        /// <summary>
        /// 上传的图片按比例裁切,返回image对象
        /// </summary>
        /// <param name="originalImage">原始图片</param>
        /// <param name="width">将要达到的宽度</param>
        /// <param name="height">将要达到的高度</param>
        /// <param name="mode">裁切模式</param>
        /// <param name="HightMode">图片质量</param>
        /// <returns></returns>
        public static System.Drawing.Image CutImg(System.Drawing.Image originalImage, int width, int height, string mode)
        {
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(System.Drawing.Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                    new System.Drawing.Rectangle(x, y, ow, oh),
                    System.Drawing.GraphicsUnit.Pixel);


            }
            return bitmap;

        }

        /// <summary>
        /// 生成定制宽高的图片
        /// </summary>
        /// <param name="originalImagePath">原始图片路径</param>
        /// <param name="thumbnailPath">定制图片路径</param>
        /// <param name="width">定制宽度</param>
        /// <param name="height">定制高度</param>
        /// <param name="mode">图片裁切模式</param>
        /// <param name="HightMode">图片质量</param>
        public static void MakeCutImg(string originalImagePath, string thumbnailPath, int width, int height, CutMode mode, bool HightMode)
        {
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath))
            {
                //图片裁切
                using (System.Drawing.Image customerImage = CutImg(originalImage, width, height, mode, HightMode))
                {
                    //EventLog.WriteLog(customerImage.Width.ToString());
                    //图片保存
                    customerImage.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

            }
        }

        /// <summary>
        /// 上传的图片按比例裁切
        /// </summary>
        /// <param name="originalImage">原始图片</param>
        /// <param name="width">将要达到的宽度</param>
        /// <param name="height">将要达到的高度</param>
        /// <param name="mode">裁切模式</param>
        /// <param name="HightMode">图片质量</param>
        /// <returns></returns>
        public static System.Drawing.Image CutImg(System.Drawing.Image originalImage, int width, int height, CutMode mode, bool HightMode)
        {
            //将要达到的宽度
            int towidth = width;
            //将要达到的高度
            int toheight = height;

            int x = 0;
            int y = 0;
            //原始宽度
            int ow = originalImage.Width;
            //原始高度
            int oh = originalImage.Height;

            switch (mode)
            {
                case CutMode.WIDTH_HEIGHT://指定高宽缩放（可能变形）                
                    break;
                case CutMode.WIDTH://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case CutMode.HEIGHT://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case CutMode.CUT://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                case CutMode.AUTO://自动,在宽高内
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    else
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    break;
                default:
                    break;
            }
            //if (originalImage.Width < width)
            //{
            //    //towidth = originalImage.Width;
            //    ow = originalImage.Width;
            //}
            //if (originalImage.Height < height)
            //{
            //    //toheight = originalImage.Height;
            //    oh = originalImage.Height;
            //}
            ////达到的宽高大于原始图宽高
            //if (towidth > ow && toheight > oh)
            //{
            //    towidth = ow;
            //    toheight = oh;
            //}
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                if (HightMode)
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                }
                g.Clear(Color.White);
                //int a = (height - toheight) / 2;
                //int b = (width - towidth) / 2;
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);
            }
            return bitmap;

        }


        public static bool MakeThumbnail(FileUpload fup, string imgPath, int width, int height, string mode, out string msg, out string imgName)
        {
            if (fup.PostedFile.FileName.IsNullOrEmpty())
            {
                msg = "上传图片不能为空！";
                imgName = "";
                return false;
            }

            string filepath = fup.PostedFile.FileName;                                //得到的是文件的完整路径,包括文件名
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);       //得到上传的文件名20022775_m.jpg 

            int idx = filename.LastIndexOf(".");
            string suffix = filename.Substring(idx);                                    //获得上传的图片的后缀名 

            if (suffix.ToLower() != ".jpg" && suffix.ToLower() != ".gif" && suffix.ToLower() != ".png")
            {
                msg = "图片格式不正确，只能是jpg,gif,png格式";
                imgName = "";
                return false;
            }
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(fup.PostedFile.InputStream))
            {
                int towidth = width;
                int toheight = height;
                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;
                //System.Drawing.Imaging.ImageFormat imgType = originalImage.RawFormat;   //获取图片格式

                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "W"://指定宽，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }
                //新建一个bmp图片
                using (System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight))
                {
                    //新建一个画板
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        //设置高质量插值法
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        //设置高质量,低速度呈现平滑程度
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //清空画布并以透明背景色填充
                        g.Clear(System.Drawing.Color.Transparent);
                        //在指定位置并且按指定大小绘制原图片的指定部分
                        g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                            new System.Drawing.Rectangle(x, y, ow, oh),
                            System.Drawing.GraphicsUnit.Pixel);

                        //上传的图片改名改名
                        filename = Utility.GetRandomNumber() + suffix;

                        string serverpath = HttpContext.Current.Server.MapPath(imgPath + filename);//取得文件在服务器上保存的位置
                        //以jpg格式保存缩略图
                        bitmap.Save(serverpath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        msg = "上传成功！";
                        imgName = filename; ;
                        return true;
                    }
                }
            }
        }



        /**/
        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sy">生成的带文字水印的图片路径</param>
        protected static void AddShuiYinWord(string Path, string Path_sy)
        {
            string addText = "中原收藏商城";
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(Path))
            {
                using (Bitmap bmp = new Bitmap(image, image.Width, image.Height))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                        float fontSize = 16.0f;    //字体大小  
                        float textWidth = addText.Length * fontSize;  //文本的长度  
                        //下面定义一个矩形区域，以后在这个矩形里画上白底黑字   
                        float rectWidth = addText.Length * (fontSize + 8);
                        float rectHeight = fontSize + 8;
                        float rectX = bmp.Width - rectWidth;
                        float rectY = bmp.Height - rectHeight;
                        //声明矩形域  
                        RectangleF textArea = new RectangleF(rectX, rectY, rectWidth, rectHeight);
                        Font font = new Font("宋体", fontSize);   //定义字体  
                        Brush whiteBrush = new SolidBrush(Color.White);   //白笔刷，画文字用  
                        Brush blackBrush = new SolidBrush(Color.Black);   //黑笔刷，画背景用  
                        g.FillRectangle(blackBrush, rectX, rectY, rectWidth, rectHeight);
                        g.DrawString(addText, font, whiteBrush, textArea);
                    }
                    image.Dispose();   //显示释放对象,防止生成的带文字水印的图片路径与原图片的路径一致
                    bmp.Save(Path_sy, ImageFormat.Jpeg);
                }
            }
        }
        /**/
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        protected static void AddShuiYinPic(string Path, string Path_syp, string Path_sypf)
        {

            using (System.Drawing.Image img = System.Drawing.Image.FromFile(Path))
            {
                //如果原图片是索引像素格式之列的，则需要转换
                if (IsPixelFormatIndexed(img.PixelFormat))
                {

                    using (Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.DrawImage(img, 0, 0);
                        }
                        using (System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf))
                        {
                            using (System.Drawing.Graphics gs = System.Drawing.Graphics.FromImage(bmp))
                            {
                                gs.DrawImage(copyImage, new System.Drawing.Rectangle(bmp.Width - copyImage.Width, bmp.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
                            }
                        }
                        img.Dispose();    //此处必须释放
                        bmp.Save(Path_syp, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                }
                else //由于水印要添加在原图片上，不能直接对img对象进行操作，因为此对象创建后，被锁定，不能被更改，否则会报错：GDI+ 中发生一般性错误
                {
                    //先创建一bmp对象，把img对象通过指定宽高转移到bmp对象中
                    using (Bitmap bmp = new Bitmap(img, img.Width, img.Height))
                    {
                        //创建水印的System.Drawing.Image对象
                        using (System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf))
                        {
                            //创建gdi+绘图图画
                            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                            {
                                //通过画图把水印添加到bmp对象中
                                g.DrawImage(copyImage, new System.Drawing.Rectangle(bmp.Width - copyImage.Width, bmp.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
                            }
                        }
                        img.Dispose();  //释放img对象,此处必须释放，否则下一步存放时要对该图片进行操作
                        //EventLog.WriteLog(Path_syp);
                        //指定jpeg格式，否则图片大小过大
                        bmp.Save(Path_syp, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                }

            }
        }



    }
}
