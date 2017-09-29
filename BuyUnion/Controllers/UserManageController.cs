using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
namespace BuyUnion.Controllers
{
    public class UserManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserManage
        public ActionResult Index(Enums.UserType type = Enums.UserType.System, int page = 1)
        {
            var model =
                (from u in db.Users
                 join rg in db.RoleGroups on u.RoleGroupID equals rg.ID into rr
                 from r in rr.DefaultIfEmpty()
                 where u.UserType == type
                 select new UserViewModels
                 {
                     Id = u.Id,
                     UserName = u.UserName,
                     RegisterDateTime = u.RegisterDateTime,
                     UserType = u.UserType,
                     RoleName = r.Name
                 })
                .ToPagedList(page);
            return View(model);
        }
    }
}