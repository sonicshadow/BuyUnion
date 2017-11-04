using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using WxPayAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace BuyUnion.Controllers
{
    [Authorize]
    public class OrderManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        private string UserID
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        private void Sidebar()
        {
            ViewBag.Sidebar = "订单管理";
        }

        // GET: OrderManage
        [Authorize(Roles = SysRole.OrderManageRead)]
        public ActionResult Index(Enums.OrderState? state, int page = 1)
        {
            Sidebar();
            var order = db.Orders.AsQueryable();
            if (state.HasValue)
            {
                order = order.Where(s => s.State == state);
            }
            var model = order.OrderByDescending(s => s.UpdateDateTime).ToPagedList(page);
            return View(model);
        }

        // GET: OrderManage/Details/5
        [Authorize(Roles = SysRole.OrderManageRead)]
        public ActionResult Details(int id)
        {
            Sidebar();
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == id);
            return View(GetModel(order));
        }

        // GET: OrderManage/Edit/5
        [Authorize(Roles = SysRole.OrderManageEdit)]
        public ActionResult Edit(int id)
        {
            Sidebar();
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == id);
            return View(GetModel(order));
        }

        // POST: OrderManage/Edit/5
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = SysRole.OrderManageEdit)]
        public ActionResult EditConfrim(int id, Enums.OrderState state)
        {
            Sidebar();
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == id);
            switch (state)
            {
                case Enums.OrderState.Paid:
                    {
                        if (order.State != Enums.OrderState.WaitPaid)
                        {
                            ModelState.AddModelError("", "订单已完成付款");
                        }
                        //else
                        //{
                        //    //测试佣金和提现
                        //    Bll.Orders.Pay(order.Code, "123456789", Enums.PayType.WeChat, order.PaidAmount);
                        //    //测试佣金和提现
                        //}
                    }
                    break;
                case Enums.OrderState.Cancel:
                    {
                        try
                        {
                            Bll.Orders.Cancle(order.Code, UserID);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                        }
                    }
                    break;
                case Enums.OrderState.Shipped:
                    {
                        if (order.State != Enums.OrderState.Paid)
                        {
                            ModelState.AddModelError("", "订单未付款不能发货");
                        }
                        if (order.State == Enums.OrderState.Shipped)
                        {
                            ModelState.AddModelError("", "订单未发货不能发货");
                        }
                        var his = new OrderLog
                        {
                            CreateDateTime = DateTime.Now,
                            OrderID = order.ID,
                            Reamrk = $"发货",
                            Type = Enums.OrderLogType.Ship,
                            UserID = UserID,
                        };
                        db.OrderLogs.Add(his);
                        var pids = order.Details.Select(s => s.ProductID).Distinct();
                        var products = db.Products.Where(s => pids.Contains(s.ID)).ToList();
                        foreach (var item in order.Details)
                        {
                            var product = products.FirstOrDefault(s => s.ID == item.ProductID);
                            product.Stock = product.Stock - item.Count;
                        }
                    }
                    break;
                case Enums.OrderState.Complete:
                    {
                        if (order.State != Enums.OrderState.Complete && order.State != Enums.OrderState.Complete)
                        {
                            var his = new OrderLog
                            {
                                CreateDateTime = DateTime.Now,
                                OrderID = order.ID,
                                Reamrk = $"完成订单",
                                Type = Enums.OrderLogType.Complete,
                                UserID = UserID
                            };
                            db.OrderLogs.Add(his);
                        }
                        else
                        {
                            ModelState.AddModelError("", "订单已经完成了");
                        }
                    }
                    break;
                default:
                    break;
            }
            if (ModelState.IsValid)
            {
                order.State = state;
                order.UpdateDateTime = DateTime.Now;
                db.SaveChanges();
                return View(GetModel(order));
            }
            return View(GetModel(order));
        }

        public SubmitOrderViewModel GetModel(Order order)
        {
            var pids = order.Details.Select(s => s.ProductID).Distinct();
            var products = db.Products.Where(s => pids.Contains(s.ID)).ToList();
            var model = new SubmitOrderViewModel(order, products);
            return model;
        }

        [HttpGet]
        public ActionResult Refund(int id)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == id);
            if (order == null)
            {
                return this.ToError("错误", "订单不存在");
            }
            if (order.State != Enums.OrderState.Paid)
            {
                return this.ToError("错误", "只有已支付状态的订单才能退款");
            }
            return View(GetModel(order));
        }

        [HttpPost]
        [ActionName("Refund")]
        public ActionResult RefundConfrim(int id)
        {
            var order = db.Orders.FirstOrDefault(s => s.ID == id);
            WxPayData data = new WxPayData();
            if (!string.IsNullOrEmpty(order.PayCode))//微信订单号存在的条件下，则已微信订单号为准
            {
                data.SetValue("transaction_id", order.PayCode);
            }
            else//微信订单号不存在，才根据商户订单号去退款
            {
                data.SetValue("out_trade_no", order.Code);
            }

            data.SetValue("total_fee", (int)(order.PaidAmount * 100));//订单总金额
            data.SetValue("refund_fee", (int)(order.PaidAmount * 100));//退款金额
            data.SetValue("out_refund_no", $"REF{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}");//随机生成商户退款单号
            data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号
            var result = WxPayAPI.WxPayApi.Refund(data);
            Comm.WriteLog("WeChatRefund", JsonConvert.SerializeObject(new
            {
                State = result.GetValue("return_code"),
                Message = result.GetValue("return_msg"),
                TranscationID = result.GetValue("transaction_id"),
            }), Enums.DebugLogLevel.Normal);

            Bll.Orders.Cancle(order.Code, UserID);
            return Json(Comm.ToJsonResult("Success", "退款成功"));
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
