using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SXF.Utils
{
    /// <summary>
    /// url验证规则
    /// </summary>
    public class UrlRewriteRules
    {

        public const string OldCitySiteUrl = ",sanmenxia.nbbuy.com,kaifeng.nbbuy.com,lingbao.nbbuy.com,mianchi.nbbuy.com,jiaozuowuzhi.nbbuy.com,";
        public const string NewCitySiteUrl = @"/city/sanmenxia";
        /// <summary>
        /// url验证规则列表
        /// </summary>
        /// <returns></returns>
        public static List<PathMapping> UrlRewrite()
        {
            List<PathMapping> list = new List<PathMapping>();
            PathMapping pm1 = null;
            //三门峡首页验证规则添加1
            pm1 = new PathMapping();
            pm1.VirtualPath = @"/sanmenxia.nbbuy.com";
            pm1.RealPath = @"/www.nbbuy.com/city/sanmenxia";
            list.Add(pm1);





            #region 新闻
            //pm1 = new PathMapping();
            //pm1.VirtualPath = @"/news/info\-([\d]+)\.aspx";
            //pm1.RealPath = @"/news/info.aspx?newsid=$1";
            //list.Add(pm1);

            //pm1 = new PathMapping();
            //pm1.VirtualPath = @"/news/list\-([\d]+)-([\d]+)\.aspx";
            //pm1.RealPath = @"/news/list.aspx?classid=$1&page=$2";
            //list.Add(pm1);

            //pm1 = new PathMapping();
            //pm1.VirtualPath = @"/card/infor\-([\d]+)\.aspx";
            //pm1.RealPath = @"/card/infor.aspx?cardid=$1";
            //list.Add(pm1);

            //pm1 = new PathMapping();
            //pm1.VirtualPath = @"/card/list\-([\d]+)\-([\d]+)\.aspx";
            //pm1.RealPath = @"/card/list.aspx?classid=$1&page=$2";
            //list.Add(pm1);

            #endregion



            return list;
        }
    }
    /// <summary>
    /// 路径镜像实体类
    /// </summary>
    public class PathMapping
    {
        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string VirtualPath { set; get; }
        /// <summary>
        /// 真实路径
        /// </summary>
        public string RealPath { set; get; }
    }
}
