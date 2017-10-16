using System;
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
            if (order.State != Enums.OrderState.WaitPad)
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
                order.Address = model.Address;
                order.PhoneNumber = model.PhoneNumber;
                order.Consignee = model.Consignee;
                order.Free = order.Details.Sum(s => s.Count) * 20;
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
                order.Free = order.Details.Sum(s => s.Count) * 20;
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




        /// <summary>
        /// 混蛋企鹅请求后返回用的Ajax导致微信不能识别页面地址,必须让做两个请求让支付页面分开
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult PayOnWeiXinTemp(string orderCode, string code)
        {
            var order = db.Orders.FirstOrDefault(s => s.Code == orderCode);

            if (order == null)
            {
                return this.ToError("错误", "订单不存在");
            }
            WechatPay pay = new WechatPay();
            pay.GetOpenidAndAccessToken();
            return RedirectToAction("PayOnWeiXin", new { OrderCode = orderCode, OpenID = pay.OpenID });
        }


        public ActionResult PayOnWeiXin(string orderCode, string openid)
        {
            var model = db.Orders.FirstOrDefault(s => s.Code == orderCode);
            WechatPay pay = new WechatPay();
            pay.OpenID = openid;
            pay.GetOpenidAndAccessToken();
            //pay.GetOpenidAndAccessToken();
            pay.OrderCode = model.Code;
            pay.TotalFee = Convert.ToInt32(model.Amount * 100);
            pay.Body = $"购物单";
            pay.Attach = "";
            //pay.GoodsTag = string.Join(",", model.Details.Select(s => s.ModularProduct.Title));
            WxPayData unifiedOrderResult = pay.GetUnifiedOrderResult();
            string wxJsApiParam = pay.GetJsApiParameters();
            WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
            ViewBag.wxJsApiParam = wxJsApiParam;
            return View(model);
        }


        public ActionResult Index()
        {
            
            return View();
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