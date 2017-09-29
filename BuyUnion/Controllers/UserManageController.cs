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
            //var users =
            //    from u in db.Users
            //    from g in db.r
            //   .Where(s => s.UserType == type)
                
            //    .Select(s => new UserViewModels
            //    {
            //        Id = s.Id,
            //        UserName = s.UserName,
            //        RegisterDateTime = s.RegisterDateTime,
            //        UserType = s.UserType
            //    })
            //    .ToList();


            return View();
        }
    }
}