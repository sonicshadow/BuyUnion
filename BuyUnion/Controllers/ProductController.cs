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
            //if (string.IsNullOrWhiteSpace(userId) || !id.HasValue)
            //{
            //    return this.ToError("错误", "传参有误");
            //}
            var product = db.Products.FirstOrDefault(s => s.ID == id.Value);
            if (product == null)
            {
                return this.ToError("错误", "没有这个商品");
            }
            if (product.State == Enums.ProductState.Off)
            {
                return this.ToError("错误", "商品已经下架");
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult Details(string userId, string proxyID, string childProxyID, string ids)
        {
            //if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(proxyID))
            //{
            //    return this.ToError("错误", "传参有误");
            //}
            var order = new Order()
            {
                UpdateDateTime = DateTime.Now,
                State = Enums.OrderState.WaitPaid,
                ChildProxyID = childProxyID,
                Code = $"{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}",
                CreateDateTime = DateTime.Now,
                UserID = userId,
                ProxyID = proxyID,
                Type= Enums.OrderType.Express,
                PayType= Enums.PayType.WeChat,
            };
            var idList = ids.SplitToIntArray();
            var products = db.Products.Where(s => idList.Contains(s.ID));
            order.Details = new List<OrderDetail>();
            foreach (var item in products)
            {
                order.Details.Add(new OrderDetail()
                {
                    Count = 1,
                    Price = item.Price,
                    ProductID = item.ID,
                    Name = item.Name,
                });
            }

            order.Amount = order.Details.Sum(s => (s.Price * s.Count));
            order.Free = order.Details.Sum(s => (20 * s.Count));
            order.PaidAmount = order.Amount + order.Free;

            db.Orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("Submit", "Order", new { code = order.Code });
        }
    }
}