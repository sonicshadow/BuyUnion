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
            var order = new SubmitOrderViewModel();
            order.Code = "";
            order.Details = new List<OrderDetailViewModel>();
            order.Details.Add(new OrderDetailViewModel
            {
                Count = 1,
                ID = 1,
                Image = "~/Upload/201710111056228819.PNG",
                OrderID = 1,
                Price = 60,
                ProductID = 2
            });
            if (order.State != Enums.OrderState.WaitPad)
            {
                return this.ToError("错误", "订单已提交");
            }
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