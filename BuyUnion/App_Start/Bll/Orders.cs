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
                    var pid = order.Details.Select(s => s.ProductID).Distinct();
                    var pList = (from product in db.Products
                                 join pp in db.ProductProxys.Where(s => s.ProxyUserID == order.ProxyID) on product.ID equals pp.ProductID into productProxy
                                 from p in productProxy.DefaultIfEmpty()
                                 where pid.Contains(product.ID)
                                 select new { productProxy = p, product }).ToList();
                    //代理佣金
                    if (!string.IsNullOrWhiteSpace(order.ProxyID))
                    {
                        decimal proxyAmount = order.Details.Select(s =>
                        {
                            var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                            var commission = string.IsNullOrWhiteSpace(order.ChildProxyID) ? list.product.Commission : decimal.Multiply(list.product.Commission, (decimal)0.7);
                            var amountItem = decimal.Multiply((s.Count * s.Price), commission);
                            if (list.productProxy != null)
                            {
                                var cha = list.productProxy.Max - list.productProxy.Count;//剩余的全额佣金
                                if (cha - s.Count >= 0)
                                {
                                    commission = 1;
                                    amountItem = decimal.Multiply((s.Count * s.Price), commission);
                                    list.productProxy.Count = list.productProxy.Count + s.Count;
                                }
                                else
                                {
                                    amountItem = decimal.Multiply(((s.Count - cha) * s.Price), commission);//没有的全额佣金那部分
                                    amountItem = amountItem + decimal.Multiply((cha * s.Price), 1);//全额佣金那部分
                                    list.productProxy.Count = list.productProxy.Max;
                                }
                            }
                            return amountItem;
                        }).Sum();
                        var proxyAmountLog = new ProxyAmountLog()
                        {
                            Amount = proxyAmount,
                            CreateDateTime = DateTime.Now,
                            ProxyID = order.ProxyID,
                            Type = Enums.AmountLogType.Income,
                        };
                        db.ProxyAmountLogs.Add(proxyAmountLog);
                    }
                    //二级代理佣金
                    if (!string.IsNullOrWhiteSpace(order.ChildProxyID))
                    {
                        var childProxyAmount = order.Details.Select(s =>
                        {
                            var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                            var commission = decimal.Multiply(list.product.Commission, (decimal)0.3);
                            var amountItem = decimal.Multiply((s.Count * s.Price), commission);
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
                    //提现
                    var shopAmountLog = new ShopAmountLog()
                    {
                        Amount = order.PaidAmount,
                        CreateDateTime = DateTime.Now,
                        Type = Enums.AmountLogType.Income
                    };
                    db.ShopAmountLogs.Add(shopAmountLog);
                    //添加销量
                    foreach (var item in pList)
                    {
                        var sale = order.Details.Where(s => s.ProductID == item.product.ID).Select(s => s.Count).Sum();
                        item.product.Sales = item.product.Sales + sale;
                    }
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
                            var pList = (from product in db.Products
                                         join pp in db.ProductProxys.Where(s => s.ProxyUserID == order.ProxyID) on product.ID equals pp.ProductID into productProxy
                                         from p in productProxy.DefaultIfEmpty()
                                         where pid.Contains(product.ID)
                                         select new { productProxy = p, product }).ToList();
                            //代理佣金
                            if (!string.IsNullOrWhiteSpace(order.ProxyID))
                            {
                                var proxyAmount = order.Details.Select(s =>
                                {
                                    var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                                    var commission = string.IsNullOrWhiteSpace(order.ChildProxyID) ? list.product.Commission : decimal.Multiply(list.product.Commission, (decimal)0.7);
                                    var amountItem = decimal.Multiply((s.Count * s.Price), commission);
                                    return amountItem;
                                }).Sum();
                                var proxyAmountLog = new ProxyAmountLog()
                                {
                                    Amount = proxyAmount,
                                    CreateDateTime = DateTime.Now,
                                    ProxyID = order.ProxyID,
                                    Type = Enums.AmountLogType.Refund,
                                };
                                db.ProxyAmountLogs.Add(proxyAmountLog);
                            }
                            //二级代理佣金
                            if (!string.IsNullOrWhiteSpace(order.ChildProxyID))
                            {
                                var childProxyAmount = order.Details.Select(s =>
                                {
                                    var list = pList.FirstOrDefault(x => x.product.ID == s.ProductID);
                                    var commission = decimal.Multiply(list.product.Commission, (decimal)0.3);
                                    var amountItem = decimal.Multiply((s.Count * s.Price), commission);
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
                            //商家退款
                            var shopAmountLog = new ShopAmountLog()
                            {
                                Amount = order.PaidAmount,
                                CreateDateTime = DateTime.Now,
                                Type = Enums.AmountLogType.Refund
                            };
                            db.ShopAmountLogs.Add(shopAmountLog);
                            //减少销量
                            foreach (var item in pList)
                            {
                                var sale = order.Details.Where(s => s.ProductID == item.product.ID).Select(s => s.Count).Sum();
                                item.product.Sales = item.product.Sales - sale;
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
                order.State = Enums.OrderState.Cancel;
                db.SaveChanges();
            }
        }
    }
}