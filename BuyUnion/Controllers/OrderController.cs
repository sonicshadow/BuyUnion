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
            //var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.Code == code);
            //if (order==null)
            //{
            //    return this.ToError("错误", "订单有误");
            //}
            var model = new SubmitOrderViewModel() { };
            model.Details = new List<OrderDetailViewModel>();
            model.Details.Add(new OrderDetailViewModel
            {
                Count = 1,
                ID = 1,
                Image = "~/Upload/201710111056228819.PNG",
                OrderID = 1,
                Price = 60,
                ProductID = 2
            });
            if (model.State != Enums.OrderState.WaitPad)
            {
                return this.ToError("错误", "订单已提交");
            }
            return View(model);

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