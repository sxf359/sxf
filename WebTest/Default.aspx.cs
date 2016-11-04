using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SXF.Utils;
using ShopNum1.QRCode;
 
namespace WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           ChartImage chartImage = new ChartImage();
            //Response.Write(Request.Url);
            //Response.End();
            chartImage.CreateChartImage(Server.MapPath("/Upload/qrcode.png"), "http://www.baidu.com", 230, 230);
            Response.Write("二维码生成成功");
        }
    }
}
