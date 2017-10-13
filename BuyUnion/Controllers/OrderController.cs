using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using System.Data.Entity;
namespace BuyUnion.Controllers
{
    public class OrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

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
            var model = new SubmitOrderViewModel()
            {
                ID = order.ID,
                Address = order.Address,
                Amount = order.Amount,
                State = order.State,
                Consignee = order.Consignee,
                Type = order.Type,
                PayType = order.PayType,
                PaidAmount = order.PaidAmount,
                Free = order.Free,
                Code = order.Code,
                PhoneNumber = order.PhoneNumber,
            };
            var pids = order.Details.Select(s => s.ProductID).Distinct();
            var ps = db.Products.Where(s => pids.Contains(s.ID)).ToList();
            model.Details = order.Details.Select(s => new OrderDetailViewModel()
            {
                Count = s.Count,
                ID = s.ID,
                Image = ps.FirstOrDefault(x => x.ID == s.ProductID).Image,
                Name = s.Name,
                Price = s.Price,
                ProductID = s.ProductID
            }).ToList();
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

        ///// <summary>
        ///// 混蛋企鹅请求后返回用的Ajax导致微信不能识别页面地址,必须让做两个请求让支付页面分开
        ///// </summary>
        ///// <returns></returns>
        //[AllowCrossSiteJson]
        //public ActionResult PayOnWeiXinTemp(string orderCode, string code)
        //{

        //    var model = new SubmitOrderViewModel(orderCode);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    WechatPay pay = new WechatPay(Request, Response);
        //    pay.GetOpenidAndAccessToken();
        //    return RedirectToAction("PayOnWeiXin", new { OrderCode = orderCode, OpenID = pay.OpenID });
        //}


        //public ActionResult PayOnWeiXin(string orderCode, string openid)
        //{
        //    var model = new OrderSubmitWaitPayViewModel(orderCode);
        //    WechatPay pay = new WechatPay(Request, Response);
        //    pay.OpenID = openid;
        //    pay.GetOpenidAndAccessToken();
        //    //pay.GetOpenidAndAccessToken();
        //    pay.OrderCode = model.Code;
        //    pay.TotalFee = Convert.ToInt32(model.TotalPay * 100);
        //    pay.Body = $"【六沐商城】购物单";
        //    pay.Attach = "";
        //    //pay.GoodsTag = string.Join(",", model.Details.Select(s => s.ModularProduct.Title));
        //    WxPayData unifiedOrderResult = pay.GetUnifiedOrderResult();
        //    string wxJsApiParam = pay.GetJsApiParameters();
        //    WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
        //    ViewBag.wxJsApiParam = wxJsApiParam;
        //    return View(model);
        //}


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