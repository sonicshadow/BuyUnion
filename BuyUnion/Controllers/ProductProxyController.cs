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
                       select new { pp, p.Name, p.Image })
                       .ToList().FirstOrDefault();
            if (pps == null)
            {
                return Json(Comm.ToJsonResult("Error", "没有这个记录"), JsonRequestBehavior.AllowGet);
            }
            var data = new
            {
                pps.pp.ProductID,
                pps.Name,
                pps.pp.ProxyUserID,
                pps.pp.Count,
                pps.pp.Max,
                pps.pp.ID,
                Image = Comm.ResizeImage(pps.Image, image: null),
            };
            return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowCrossSiteJson]
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
        [AllowCrossSiteJson]
        public ActionResult Edit(ProductProxy model)
        {
            var pp = db.ProductProxys.FirstOrDefault(s => s.ID == model.ID);
            if (pp == null)
            {
                return Json(Comm.ToJsonResult("Error", "没有参加此活动"));
            }
            pp.Max = model.Max;
            pp.ProductID = model.ProductID;
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"));
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Delete(int id)
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