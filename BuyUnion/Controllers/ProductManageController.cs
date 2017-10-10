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
            var product = db.Products.FirstOrDefault(s => s.ID == id);

            return View(new ProductCreateEditViewModel(product));
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