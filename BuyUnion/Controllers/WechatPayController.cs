using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;
using BuyUnion.Models;
using System.Text;
using BuyUnion.Enums;
using BuyUnion.WeChatPay;

namespace BuyUnion.Controllers
{
    public class WechatPayController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();




        /// <summary>
        /// 混蛋企鹅请求后返回用的Ajax导致微信不能识别页面地址,必须让做两个请求让支付页面分开
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult PayTemp(string orderCode, string code)
        {
            var order = db.Orders.FirstOrDefault(s => s.Code == orderCode);

            if (order == null)
            {
                return this.ToError("错误", "订单不存在");
            }
            if (Comm.IsWeChat)
            {
                WechatPay pay = new WechatPay();
                pay.GetOpenidAndAccessToken();
                return RedirectToAction("Pay", new { OrderCode = orderCode, OpenID = pay.OpenID });
            }
            return RedirectToAction("Pay", new { OrderCode = orderCode, OpenID = "" });
        }


        public ActionResult Pay(string orderCode, string openid)
        {
            var model = db.Orders.FirstOrDefault(s => s.Code == orderCode);
            HttpCookie cok = Request.Cookies["UserId"];
            if (cok != null)
            {
                cok.Value = model.UserID;
            }
            else
            {
                HttpCookie cookie = new HttpCookie("UserId");
                cookie.Value = model.UserID;
                Response.Cookies.Add(cookie);
            }
            if (string.IsNullOrWhiteSpace(model.PayCode))
            {
                WechatPay pay = new WechatPay();
                if (Comm.IsWeChat)
                {
                    pay.OpenID = openid;
                    pay.GetOpenidAndAccessToken();
                }
                pay.OrderCode = model.Code;
                pay.TotalFee = Convert.ToInt32(model.Amount * 100);
                pay.Body = $"购物单";
                pay.Attach = "";
                //pay.GoodsTag = string.Join(",", model.Details.Select(s => s.ModularProduct.Title));
                WxPayData unifiedOrderResult = pay.GetUnifiedOrderResult();
                string wxJsApiParam = pay.GetJsApiParameters();
                WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                ViewBag.wxJsApiParam = wxJsApiParam;
            }
            return View(model);
        }

        public ActionResult NotifyUrl()
        {
            WxPayData notifyData = GetNotifyData();
            //参数列表 https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_7
            string orderCode = notifyData.GetValue("out_trade_no").ToString();

            var order = db.Orders.FirstOrDefault(s => s.Code == orderCode);

            if (order == null)
            {
                //若订单不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中商户单号不存在");
                WxPayAPI.Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());

                return Content(res.ToXml());
            }

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                WxPayAPI.Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                return Content(res.ToXml());
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                WxPayAPI.Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                return Content(res.ToXml());
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                WxPayAPI.Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());

                Bll.Orders.Pay(orderCode, transaction_id, Enums.PayType.WeChat, order.Amount);
                WxPayAPI.Log.Info(this.GetType().ToString(), $"order state change success : OrderCode={orderCode}");
                return Content(res.ToXml());
            }
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData()
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            WxPayAPI.Log.Info(this.GetType().ToString(), "Receive data from WeChat : " + builder.ToString());

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                WxPayAPI.Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
                Response.Write(res.ToXml());
                Response.End();
            }

            WxPayAPI.Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }

        /// <summary>
        /// 微信支付完成，记录微信支付的流水号
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="tradeCode">微信支付流水号</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Result(string code, string message)
        {

            return View();
        }

        /// <summary>
        /// 异步请求订单状态
        /// </summary>
        /// <param name="transaction_id">微信订单号</param>
        /// <returns></returns>
        public ActionResult CheckWxOrderState(string transaction_id)
        {
            return Json(new { result = QueryOrder(transaction_id) }, JsonRequestBehavior.AllowGet);
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
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