using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuyUnion.Models;
using Newtonsoft.Json;

namespace BuyUnion.Bll
{
    public static class Orders
    {
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tradeCode"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        public static void Pay(string code, string tradeCode, Enums.PayType type, decimal amount)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var order = db.Orders.FirstOrDefault(s => s.Code == code);
                if (order.State == Enums.OrderState.WaitPaid)
                {
                    if (code == null)
                    {
                        throw new Exception("订单不存在");
                    }
                    if (string.IsNullOrWhiteSpace(tradeCode))
                    {
                        throw new Exception("交易流水号为空");
                    }
                    order.PayCode = tradeCode;
                    order.PayType = type;
                    order.State = Enums.OrderState.Paid;
                    order.UpdateDateTime = DateTime.Now;
                    order.PaidAmount = amount;
                    db.SaveChanges();
                    //分成
                    var pids = order.Details.Select(s => s.ProductID).Distinct();
                    var pList = (from productProxy in db.ProductProxys
                                 from product in db.Products
                                 from detail in order.Details
                                 where productProxy.ProxyUserID == order.ProxyID
                                     && productProxy.ProductID == product.ID
                                     && detail.ProductID == product.ID
                                 select new { productProxy, product, detail }).ToList();
                    string exData = null;
                    if (pList.Count > 0)
                    {
                        var proxyAmount = pList.Select(s =>
                        {
                            var commission = s.productProxy.Max - s.productProxy.Count > 0 ?
                                1 :
                                decimal.Multiply(s.product.Commission, (decimal)0.7);
                            return decimal.Multiply((s.detail.Count * s.detail.Price), commission);
                        }).Sum();

                        decimal childProxyAmount = 0;
                        if (!string.IsNullOrWhiteSpace(order.ChildProxyID))
                        {
                            childProxyAmount = pList.Select(s =>
                             {
                                 var commission = decimal.Multiply(s.product.Commission, (decimal)0.3);
                                 return decimal.Multiply((s.detail.Count * s.detail.Price), commission);
                             }).Sum();
                        }

                        exData = JsonConvert.SerializeObject(new
                        {
                            order.ProxyID,
                            ProxyAmount = proxyAmount,
                            order.ChildProxyID,
                            ChildProxyAmount = childProxyAmount,
                        });
                    }
                    var his = new OrderLog
                    {
                        CreateDateTime = DateTime.Now,
                        OrderID = order.ID,
                        Reamrk = $"使用{type.GetDisplayName()}支付了{amount}元",
                        Type = Enums.OrderLogType.Pay,
                        UserID = order.UserID,
                        ExData = exData,
                    };

                    db.OrderLogs.Add(his);
                    db.SaveChanges();
                }

            }

        }
    }
}