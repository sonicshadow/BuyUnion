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
                    DetailsImage = string.Join(",", model.Image.Images),
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