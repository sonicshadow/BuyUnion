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
        [Authorize(Roles =SysRole.OrderManageRead)]
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
        [HttpPost,ActionName("Edit")]
        [Authorize(Roles = SysRole.OrderManageEdit)]
        public ActionResult EditConfrim(int id,Enums.OrderState state)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == id);
            switch (state)
            {
                case Enums.OrderState.Paid:
                    {
                        if (order.State != Enums.OrderState.WaitPad)
                        {
                            ModelState.AddModelError("", "订单已完成付款");
                            return View(GetModel(order));
                        }
                    }
                    break;
                case Enums.OrderState.Cancel:
                    {
                        if (order.State == Enums.OrderState.Complete)
                        {
                            ModelState.AddModelError("", "订单已完成不能取消");
                            return View(GetModel(order));
                        }
                    }
                    break;
                case Enums.OrderState.Shipped:
                    {
                        if (order.State != Enums.OrderState.Paid)
                        {
                            ModelState.AddModelError("", "订单未付款不能发货");
                            return View(GetModel(order));
                        }
                    }
                    break;
                case Enums.OrderState.Complete:
                    break;
                default:
                    break;
            }

            order.State = state;
            order.UpdateDateTime = DateTime.Now;
            db.SaveChanges();
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
