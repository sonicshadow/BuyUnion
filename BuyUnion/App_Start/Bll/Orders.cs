using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuyUnion.Models;
using System.Data.Entity;

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
                var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.Code == code);
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
                    //var pList = (from productProxy in db.ProductProxys
                    //             from product in db.Products
                    //             from detail in order.Details
                    //             where productProxy.ProxyUserID == order.ProxyID &&
                    //                   productProxy.ProductID == product.ID &&
                    //                    productProxy.ProductID == detail.ProductID
                    //             select new { productProxy, product, detail }).ToList();
                    var pid = order.Details.Select(s => s.ProductID).Distinct();
                    var pList = (from productProxy in db.ProductProxys
                                 from product in db.Products
                                 where productProxy.ProxyUserID == order.ProxyID &&
                                       productProxy.ProductID == product.ID &&
                                       pid.Contains(productProxy.ProductID)
                                 select new { productProxy, product }).ToList();

                    if (pList.Count > 0)
                    {
                        //代理佣金
                        decimal proxyAmount = 0;
                        foreach (var item in order.Details)
                        {
                            var list = pList.FirstOrDefault(x => x.product.ID == item.ProductID);
                            if (list != null)
                            {
                                var cha = list.productProxy.Max - list.productProxy.Count;//剩余的全额佣金
                                var commission = decimal.Multiply(list.product.Commission, (decimal)0.7);
                                var amountItem = decimal.Multiply((item.Count * item.Price), commission);
                                if (cha > 0)
                                {
                                    if (cha - item.Count >= 0)
                                    {
                                        commission = 1;
                                        amountItem = decimal.Multiply((item.Count * item.Price), commission);
                                    }
                                    else
                                    {
                                        amountItem = decimal.Multiply(((item.Count - cha) * item.Price), commission);
                                        commission = 1;
                                        amountItem = amountItem + decimal.Multiply((cha * item.Price), commission);
                                    }
                                }
                                proxyAmount = proxyAmount + amountItem;
                            }
                        }
                        var proxyAmountLog = new ProxyAmountLog()
                        {
                            Amount = proxyAmount,
                            CreateDateTime = DateTime.Now,
                            ProxyID = order.ProxyID,
                            Type = Enums.AmountLogType.Income,
                        };
                        db.ProxyAmountLogs.Add(proxyAmountLog);
                        //二级代理佣金
                        if (!string.IsNullOrWhiteSpace(order.ChildProxyID))
                        {
                            var childProxyAmount = order.Details.Select(s =>
                            {
                                var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                                if (list == null)
                                {
                                    return 0;
                                }
                                var commission = decimal.Multiply(list.product.Commission, (decimal)0.3);
                                return decimal.Multiply((s.Count * s.Price), commission);
                            }).Sum();
                            var childProxyAmountLog = new ProxyAmountLog()
                            {
                                Amount = childProxyAmount,
                                CreateDateTime = DateTime.Now,
                                ProxyID = order.ChildProxyID,
                                Type = Enums.AmountLogType.Income,
                            };
                            db.ProxyAmountLogs.Add(childProxyAmountLog);
                        }
                    }
                    //订单记录
                    var his = new OrderLog
                    {
                        CreateDateTime = DateTime.Now,
                        OrderID = order.ID,
                        Reamrk = $"使用{type.GetDisplayName()}支付了{amount}元",
                        Type = Enums.OrderLogType.Pay,
                        UserID = order.UserID,
                    };
                    db.OrderLogs.Add(his);
                    db.SaveChanges();
                }
            }
        }
        
        public static void Cancle(string code, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.Code == code);
                switch (order.State)
                {
                    case Enums.OrderState.Cancel:
                        {
                            throw new Exception("订单已经取消");
                        }
                        break;
                    case Enums.OrderState.Paid:
                    case Enums.OrderState.Shipped:
                    case Enums.OrderState.Complete:
                        {
                            //佣金退款
                            var pid = order.Details.Select(s => s.ProductID).Distinct();
                            var pList = (from productProxy in db.ProductProxys
                                         from product in db.Products
                                         where productProxy.ProxyUserID == order.ProxyID &&
                                               productProxy.ProductID == product.ID &&
                                               pid.Contains(productProxy.ProductID)
                                         select new { productProxy, product }).ToList();
                            if (pList.Count > 0)
                            {
                                //代理参加了这个活动
                                //代理佣金
                                var proxyAmount = order.Details.Select(s =>
                                {
                                    var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                                    if (list == null)
                                    {
                                        return 0;
                                    }
                                    var commission = decimal.Multiply(list.product.Commission, (decimal)0.7);
                                    return decimal.Multiply((s.Count * s.Price), commission);
                                }).Sum();
                                var proxyAmountLog = new ProxyAmountLog()
                                {
                                    Amount = proxyAmount,
                                    CreateDateTime = DateTime.Now,
                                    ProxyID = order.ProxyID,
                                    Type = Enums.AmountLogType.Refund,
                                };
                                db.ProxyAmountLogs.Add(proxyAmountLog);
                                //二级代理佣金
                                if (!string.IsNullOrWhiteSpace(order.ChildProxyID))
                                {
                                    var childProxyAmount = order.Details.Select(s =>
                                    {
                                        var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                                        if (list == null)
                                        {
                                            return 0;
                                        }
                                        var commission = decimal.Multiply(list.product.Commission, (decimal)0.3);
                                        return decimal.Multiply((s.Count * s.Price), commission);
                                    }).Sum();
                                    var childProxyAmountLog = new ProxyAmountLog()
                                    {
                                        Amount = childProxyAmount,
                                        CreateDateTime = DateTime.Now,
                                        ProxyID = order.ChildProxyID,
                                        Type = Enums.AmountLogType.Refund,
                                    };
                                    db.ProxyAmountLogs.Add(childProxyAmountLog);
                                }
                            }
                            //商家退款
                            if (order.State == Enums.OrderState.Complete)
                            {
                                var shopAmountLog = new ShopAmountLog()
                                {
                                    Amount = order.PaidAmount,
                                    CreateDateTime = DateTime.Now,
                                    Type = Enums.AmountLogType.Refund
                                };
                                db.ShopAmountLogs.Add(shopAmountLog);
                            }
                        }
                        break;
                    default:
                        break;
                }
                //订单修改记录
                var his = new OrderLog
                {
                    CreateDateTime = DateTime.Now,
                    OrderID = order.ID,
                    Reamrk = $"取消订单",
                    Type = Enums.OrderLogType.SubmitCancel,
                    UserID = userId,
                };
                db.OrderLogs.Add(his);
                db.SaveChanges();
            }
        }
    }
}