using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

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
            var model = order.OrderBy(s => s.UpdateDateTime).ToPagedList(page);
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
                    }
                    break;
                case Enums.OrderState.Cancel:
                    {
                        switch (order.State)
                        {
                            case Enums.OrderState.WaitPaid:
                                { }
                                break;
                            case Enums.OrderState.Paid:
                                {
                                    var log = db.OrderLogs.FirstOrDefault(s => s.OrderID == order.ID &&
                                        s.Type == Enums.OrderLogType.Pay);
                                    log.ExData = null;
                                }
                                break;
                            default:
                                {
                                    ModelState.AddModelError("", "订单不能取消");
                                }
                                break;
                        }
                        var his = new OrderLog
                        {
                            CreateDateTime = DateTime.Now,
                            OrderID = order.ID,
                            Reamrk = $"取消订单",
                            Type = Enums.OrderLogType.SubmitCancel,
                            UserID = UserID,
                        };
                        db.OrderLogs.Add(his);
                    }
                    break;
                case Enums.OrderState.Shipped:
                    {
                        if (order.State != Enums.OrderState.Paid)
                        {
                            ModelState.AddModelError("", "订单未付款不能发货");
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
