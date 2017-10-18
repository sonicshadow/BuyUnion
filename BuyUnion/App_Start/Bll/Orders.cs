using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuyUnion.Models;
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
    }
}