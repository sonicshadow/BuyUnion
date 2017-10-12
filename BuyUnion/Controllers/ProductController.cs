using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;

namespace BuyUnion.Controllers
{
    public class ProductController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string userId, int? id)
        {
            if (string.IsNullOrWhiteSpace(userId) || !id.HasValue)
            {
                return this.ToError("", "传参有误");
            }
            var product = db.Products.FirstOrDefault(s => s.ID == id.Value);
            if (product == null)
            {
                return this.ToError("", "没有这个商品");
            }
            if (product.State == Enums.ProductState.Off)
            {
                return this.ToError("", "商品已经下架");
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult Details(string userId, string proxyID, string childProxyID, int id)
        {
            var product = db.Products.FirstOrDefault(s => s.ID == id);
            var order = new Order()
            {
                UpdateDateTime = DateTime.Now,
                State = Enums.OrderState.WaitPad,
                ChildProxyID = childProxyID,
                Code = $"{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}",
                CreateDateTime = DateTime.Now,
                UserID = userId,
                ProxyID = proxyID,
            };
            db.Orders.Add(order);
            //db.SaveChanges();
            return RedirectToAction("Submit", "Order", new { productId = id });
        }
    }
}