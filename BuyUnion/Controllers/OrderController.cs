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
                //Image = "~/Upload/201710111056228819.PNG",
                OrderID = 1,
                Price = 60,
                ProductID = 2
            });
            var pids = model.Details.Select(s => s.ProductID).Distinct();
            var ps = db.Products.Where(s => pids.Contains(s.ID));
            foreach (var item in model.Details)
            {
                item.Product = ps.FirstOrDefault(s => s.ID == item.ProductID);
            }
            if (model.State != Enums.OrderState.WaitPad)
            {
                return this.ToError("错误", "订单已提交");
            }
            return View(model);
        }

        public ActionResult Edit(OrderDetailViewModel model)
        {
            var order = db.Orders.Include(s => s.Details).FirstOrDefault(s => s.ID == model.ID);
            return Json(Comm.ToJsonResult("Success", ""));
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