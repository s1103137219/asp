using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Models
{
    public class OrderSearchArg 
    {
        public string OrderID { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeID { get; set; }
        public string ShipperID { get; set; }
        public string Orderdate { get; set; }
        public string ShippedDate { get; set; }
        public string RequireDdate { get; set; }
        public string delbtn { get; set; }
        public string editbtn { get; set; }
        public string ProuductName { get; set; }
        public string UnitPrice { get; set; }

}
}