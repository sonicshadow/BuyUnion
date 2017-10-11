using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;

namespace BuyUnion.Controllers
{
    public class ProductManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private void Sidebar()
        {
            ViewBag.Sidebar = "商品管理";
        }

        // GET: ProductManage
        public ActionResult Index(int? page = 1)
        {
            Sidebar();
            var query = db.Products.AsQueryable();
            var models = query.OrderByDescending(s => s.ID).ToPagedList(page.Value);
            return View(models);
        }

        public ActionResult Create()
        {
            Sidebar();
            var model = new ProductCreateEditViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductCreateEditViewModel model)
        {
            if (model.Image.Images.Length == 0)
            {
                ModelState.AddModelError("Image", "主图片必须上传");
            }
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Commission = model.Commission,
                    DetailsImage = string.Join(",", model.DetailsImage.Images),
                    Image = model.Image.Images[0],
                    Name = model.Name,
                    OriginalPrice = model.OriginalPrice,
                    Price = model.Price,
                    Remark = model.Remark,
                    Stock = model.Stock,
                    CreateDateTime = DateTime.Now
                };
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            Sidebar();
            var product = db.Products.FirstOrDefault(s => s.ID == id);

            return View(new ProductCreateEditViewModel(product));
        }

        [HttpPost]
        public ActionResult Edit(ProductCreateEditViewModel model)
        {
            Sidebar();
            if (model.Image.Images.Length == 0)
            {
                ModelState.AddModelError("Image", "主图片必须上传");
            }
            if (ModelState.IsValid)
            {
                var product = db.Products.FirstOrDefault(s => s.ID == model.ID);
                product.Commission = model.Commission;
                product.DetailsImage = string.Join(",", model.DetailsImage.Images);
                product.Image = string.Join(",", model.Image.Images);
                product.Name = model.Name;
                product.OriginalPrice = model.OriginalPrice;
                product.Price = model.Price;
                product.Remark = model.Remark;
                product.Stock = model.Stock;
                db.SaveChanges();
            }
            Sidebar();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            Sidebar();
            var product = db.Products.FirstOrDefault(s => s.ID == id);

            return View(new ProductCreateEditViewModel(product));
        }

        public ActionResult Delete(int id)
        {
            var product = db.Products.FirstOrDefault(s => s.ID == id);

            return View(new ProductCreateEditViewModel(product));

        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfrim(int id)
        {
            var product = db.Products.FirstOrDefault(s => s.ID == id);
            if (db.ProductProxys.Any(s => s.ProductID == id))
            {
                ModelState.AddModelError("", "该商品已有关联代理");
            }
            if (db.OrderDetails.Any(s => s.ProductID == id))
            {
                ModelState.AddModelError("", "该商品产生订单");
            }
            if (ModelState.IsValid)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            return View(new ProductCreateEditViewModel(product));

        }

        [HttpPost]
        public ActionResult ChangeState(int id, Enums.ProductState state)
        {
            var product = db.Products.FirstOrDefault(s => s.ID == id);
            if (product == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "商品不存在"));
            }
            product.State = state;
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", $"{state.GetDisplayName()}成功"));
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