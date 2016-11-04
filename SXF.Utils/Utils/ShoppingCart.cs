using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Utils
{
    /// <summary>
    /// 购物车类
    /// </summary>
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //

        }

        /// <summary>
        /// 商品ID
        /// </summary>
        public int Id
        {
            get;
            set;
        }
      
      
        /// <summary>
        /// 商品价格
        /// </summary>
        public long Price
        {
            get;
            set;
        }
        

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number
        {
            get;
            set;
        }
    }
}
