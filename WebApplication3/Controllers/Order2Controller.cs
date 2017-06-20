using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Controllers
{
    public class Order2Controller : Controller
    {
        Models.CodeService aa = new Models.CodeService();

        public ActionResult Index()
        {

            ViewBag.employeename = aa.GetEmployeeName();

            ViewBag.shippername = aa.GetShipperName();
            Models.OrderService orderService = new Models.OrderService();
            ViewBag.Data = orderService.GetOrderById("");

            return View();

        }

        [HttpPost()]
        public ActionResult Index(Models.OrderSearchArg data)
        {

            ViewBag.employeename = aa.GetEmployeeName();

            ViewBag.shippername = aa.GetShipperName();

            Models.OrderService orderService = new Models.OrderService();

            if (data.delbtn != null)
            {
                orderService.DeleteOrderDetailById(data.delbtn);
            }
            else if (data.editbtn != null)
            {
                ModelState.Clear();
                Models.Order order = orderService.GetOrderByIdForUpdate(data.editbtn);
                ViewBag.companyname = aa.GetCompanyName();
                ViewBag.employeename = aa.GetEmployeeName();
                ViewBag.shippername = aa.GetShipperName();
                ViewBag.productname = aa.GetProductName();
                order.Orderdate = Convert.ToDateTime(order.Orderdate).ToString("yyyy-MM-dd");
                order.RequireDdate = Convert.ToDateTime(order.RequireDdate).ToString("yyyy-MM-dd");
                order.ShippedDate = Convert.ToDateTime(order.ShippedDate).ToString("yyyy-MM-dd");
                return View("UpdateOrder", order);

            }
            else
            {
                ViewBag.Data = orderService.GetOrderByCondtioin(data);
            }

            return View("Index");

        }
        /// <summary>
        /// 新增訂單畫面
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult InsertOrder()
        {
            ViewBag.companyname = aa.GetCompanyName();
            ViewBag.employeename = aa.GetEmployeeName();
            ViewBag.shippername = aa.GetShipperName();
            ViewBag.productname = aa.GetProductName();

            return View(new Models.Order());
        }
        [HttpPost()]
        public ActionResult InsertOrder(Models.Order order)
        {
            Models.OrderService orderService = new Models.OrderService();
            orderService.InsertOrder(order);
            return RedirectToAction("Index", "Order");
        }
        /// <summary>
        /// 更新訂單(action)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateOrder(Models.Order order)
        {
            Models.OrderService orderService = new Models.OrderService();
            orderService.UpdateOrder(order);
            orderService.UpdateOrdertwo(order);
            return RedirectToAction("Index", "Order");
        }

    }





}
