using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Models
{
    public class OrderDetails : Controller
    {   /// <summary>
        /// 商品名稱
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 單價
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// orderid
        /// </summary>
        public int OrderID { get; set; }
    }
}