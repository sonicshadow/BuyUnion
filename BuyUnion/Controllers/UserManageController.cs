﻿using System;
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
            //    from s in db.
            //    db.Users
            //    .Where(s => s.UserType == type)
            //    .OrderByDescending(s => s.RegisterDateTime)
            //    .ToPagedList(page)
            //    .Select(s=>);
            return View();
        }
    }
}