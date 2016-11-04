using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SXF.Utils
{
    public static partial class ExtensionMethod
    {
        #region 绑定控件
        public static void DataShow(this BaseDataBoundControl control, object datasource)
        {
            control.DataSource = datasource;
            control.DataBind();
        }

        /// <summary>
        /// Repeater绑定
        /// </summary>
        /// <param name="rpt"></param>
        /// <param name="datasource"></param>
        public static void DataShow(this Repeater rpt, object datasource)
        {
            rpt.DataSource = datasource;
            rpt.DataBind();
        }
        public static void DataShow(this BaseDataList bdl, object datasource)
        {
            bdl.DataSource = datasource;
            bdl.DataBind();
        }
        #endregion

        #region TextBox
        public static string TText(this TextBox tb)
        {
            return tb.Text.Trim();
        }
        public static bool IsEmpty(this TextBox tb)
        {
            return tb.TText().IsNullOrEmpty();
        }
        public static bool IsIDCard(this TextBox tb)
        {
            return tb.TText().IsIDCard();
        }
        public static bool IsInteger(this TextBox tb)
        {
            return tb.TText().IsInteger();
        }
        public static bool IsNumber(this TextBox tb)
        {
            return tb.TText().IsNumber();
        }
        public static bool IsNotEmpty(this TextBox tb)
        {
            return tb.TText().IsNotNullAndEmpty();
        }
        public static bool IsCellPhone(this TextBox tb)
        {
            return tb.TText().IsCellPhone();
        }
        public static bool IsEmail(this TextBox tb)
        {
            return tb.TText().IsEmail();
        }
        public static void Empty(this TextBox tb)
        {
            tb.Text = string.Empty;
        }
        public static bool IsPhone(this TextBox tb)
        {
            return tb.TText().IsPhone();
        }

        /// <summary>
        /// 去除两边空格，并去除输入串单引号
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static string TRText(this TextBox tb)
        {
            return tb.Text.Trim().Replace("'", "");
        }

        #endregion
    }
}
