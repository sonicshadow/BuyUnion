﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using System.Data.Entity;
using BuyUnion.WeChatPay;
using WxPayAPI;

namespace BuyUnion.Controllers
{
    public class OrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        public string UserID
        {
            get
            {
                return this.Request.Cookies["User"].Value;
            }
        }

        // GET: Order
        public ActionResult Submit(string code)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.Code == code);
            if (order == null)
            {
                return this.ToError("错误", "订单有误");
            }
            HttpCookie cok = Request.Cookies["UserId"];
            if (cok != null)
            {
                cok.Value = order.UserID;
            }
            else
            {
                HttpCookie cookie = new HttpCookie("UserId");
                cookie.Value = order.UserID;
                Response.Cookies.Add(cookie);
            }
            if (order.State != Enums.OrderState.WaitPaid)
            {
                return this.ToError("错误", "订单已提交");
            }
            var pids = order.Details.Select(s => s.ProductID).Distinct();
            var products = db.Products.Where(s => pids.Contains(s.ID)).ToList();
            var model = new SubmitOrderViewModel(order, products);
            return View(model);
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditOrder(SubmitOrderViewModel model)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == model.ID);
            order.Type = model.Type;
            if (model.Type == Enums.OrderType.Express)
            {
                //运费
                decimal freight = 0;
                order.Address = model.Address;
                order.PhoneNumber = model.PhoneNumber;
                order.Consignee = model.Consignee;
                order.Free = order.Details.Sum(s => s.Count) * freight;
            }
            else
            {
                order.Free = 0;
            }
            order.PaidAmount = order.Amount + order.Free;
            order.UpdateDateTime = DateTime.Now;
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "修改成功", new
            {
                order.Amount,
                order.Free
            }));
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditDetails(int id, int count)
        {
            var details = db.OrderDetails.FirstOrDefault(s => s.ID == id);
            var order = db.Orders.Include(s => s.Details)
                .FirstOrDefault(s => s.ID == details.OrderID);
            details.Count = count;
            order.Amount = order.Details.Sum(s => (s.Count * s.Price));
            if (order.Type == Enums.OrderType.Express)
            {
                decimal freight = 0;
                order.Free = order.Details.Sum(s => s.Count) * freight;
            }
            else
            {
                order.Free = 0;
            }
            order.PaidAmount = order.Amount + order.Free;
            order.UpdateDateTime = DateTime.Now;
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "修改成功", new
            {
                order.Amount,
                order.Free
            }));
        }

        [HttpPost]
        public ActionResult Check(string code)
        {

            var order = db.Orders.FirstOrDefault(s => s.Code == code);
            if (order == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "订单不存在"));
            }
            if (string.IsNullOrWhiteSpace(order.Address))
            {
                return Json(Comm.ToJsonResult("Address", "未填写送货地址"));
            }
            if (order.State != Enums.OrderState.WaitPaid)
            {
                return Json(Comm.ToJsonResult("Error", "订单已提交"));
            }

            return Json(Comm.ToJsonResult("Success", "检测通过"));
        }

        [HttpGet]
        public ActionResult GetOrderState(string code)
        {
            var order = db.Orders.FirstOrDefault(s => s.Code == code);
            if (order == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "订单不存在"));
            }
            return Json(Comm.ToJsonResult("Success", "成功", new { order.State }));

        }

        public ActionResult Result()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Details(string code)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.Code == code);
            HttpCookie cok = Request.Cookies["UserId"];
            if (cok != null)
            {
                cok.Value = order.UserID;
            }
            else
            {
                HttpCookie cookie = new HttpCookie("UserId");
                cookie.Value = order.UserID;
                Response.Cookies.Add(cookie);
            }
            if (order.State == Enums.OrderState.WaitPaid)
            {
                return RedirectToAction("Submit", new { Code = code });
            }
            var productIds = order.Details.Select(s => s.ProductID).ToList();
            var products = db.Products.Where(s => productIds.Contains(s.ID)).ToList();
            var model = new SubmitOrderViewModel(order, products);
            return View(model);
        }

        public ActionResult Index()
        {
            var userId = Request.Cookies["UserId"] != null ? Request.Cookies["UserId"].Value : null;
            var orders = db.Orders.Include(s => s.Details).Where(s => s.UserID == userId).ToList();
            var productIds = new List<int>();
            foreach (var item in orders)
            {
                productIds.AddRange(item.Details.Select(s => s.ProductID).ToList());
            }
            productIds.Distinct();
            var products = db.Products.Where(s => productIds.Contains(s.ID)).ToList();
            var model = orders.Select(s =>
            {
                var pids = s.Details.Select(x => x.ProductID);
                var ps = products.Where(x => pids.Contains(x.ID));
                var item = new SubmitOrderViewModel(s, ps.ToList());
                return item;
            });
            return View(model);
        }

        [AllowAnonymous]
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult Get(string code)
        {
            var order = db.Orders.FirstOrDefault(s => s.Code == code);
            if (order == null)
            {
                return Json(Comm.ToJsonResult("Error", "订单有误"), JsonRequestBehavior.AllowGet);
            }
            return Json(Comm.ToJsonResult("Success", "成功", new
            {
                order.ID,
                order.Code,
                order.PayCode,
                order.Address,
                order.Amount,
                order.ChildProxyID,
                order.Consignee
            }), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}