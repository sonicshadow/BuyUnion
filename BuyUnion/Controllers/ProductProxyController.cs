using BuyUnion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuyUnion.Controllers
{
    public class ProductProxyController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductProxy
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GetProduct()
        {
            var products = db.Products.ToList().Select(s => new
            {
                s.ID,
                s.Name,
                Image = Comm.ResizeImage(s.Image, image: null),
            });
            return Json(Comm.ToJsonResult("Success", "成功", products), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult Get(string userId)
        {
            var pps = (from pp in db.ProductProxys
                       from p in db.Products
                       where pp.ProxyUserID == userId && p.ID == pp.ProductID
                       select new { pp, p }).ToList().Select(s => new
                       {
                           s.pp.ProductID,
                           s.p.Name,
                           s.pp.ProxyUserID,
                           s.pp.Count,
                           s.pp.Max,
                           s.pp.ID,
                           Image = Comm.ResizeImage(s.p.Image, image: null),
                       });
            return Json(Comm.ToJsonResult("Success", "成功", pps), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Creat(ProductProxy model)
        {
            if (string.IsNullOrWhiteSpace(model.ProxyUserID))
            {
                return Json(Comm.ToJsonResult("Error", "没有用户"));
            }
            if (model.ProductID <= 0)
            {
                return Json(Comm.ToJsonResult("Error", "没有选择商品"));
            }
            db.ProductProxys.Add(model);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"));
        }

        [HttpPost]
        public ActionResult Dlete(int id)
        {
            var pp = db.ProductProxys.FirstOrDefault(s => s.ID == id);
            if (pp == null)
            {
                return Json(Comm.ToJsonResult("Error", "没有参加此活动"));
            }
            db.ProductProxys.Remove(pp);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"));
        }
    }
}