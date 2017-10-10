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

        // GET: ProductManage
        public ActionResult Index(int? page = 1)
        {
            var query = db.Products.AsQueryable();
            var models = query.ToPagedList(page.Value);
            return View(models);
        }

        public ActionResult Create()
        {
            var model = new ProductCreateEditViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
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