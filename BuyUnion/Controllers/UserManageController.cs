using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BuyUnion.Controllers
{
    [Authorize]
    public class UserManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        private Bll.Roles _roles = new Bll.Roles();

        private string UserID
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        private void Sidebar()
        {
            ViewBag.Sidebar = "用户管理";
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: UserManage
        [Authorize(Roles = SysRole.UserManageRead)]
        public ActionResult Index(Enums.UserType type = Enums.UserType.System, int page = 1)
        {
            Sidebar();
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
                 }).OrderBy(s => s.RegisterDateTime)
                .ToPagedList(page);
            return View(model);
        }

        [Authorize(Roles = SysRole.UserManageCreate)]
        public ActionResult Create(Enums.UserType type = Enums.UserType.System)
        {
            Sidebar();
            if (type == Enums.UserType.System)
            {
                var roles = db.RoleGroups.ToList();
                ViewBag.SelRole = new SelectList(roles, "ID", "Name");
            }
            return View(new UserMangeCreateUserViewModel() { UserType = type });
        }

        [HttpPost]
        [Authorize(Roles = SysRole.UserManageCreate)]
        public ActionResult Create(UserMangeCreateUserViewModel model)
        {
            var user = db.Users.FirstOrDefault(s => s.UserName == model.PhoneNumber);
            if (user != null)
            {
                ModelState.AddModelError("PhoneNumber", "手机号已被使用");
            }
            if (model.UserType == Enums.UserType.System)
            {
                if (!model.RoleGroupID.HasValue)
                {
                    ModelState.AddModelError("RoleGroupID", "选择权限分组");
                }
            }
            if (ModelState.IsValid)
            {
                user = new ApplicationUser()
                {
                    UserName = model.PhoneNumber,
                    UserType = model.UserType,
                    PhoneNumber = model.PhoneNumber,
                    RegisterDateTime = DateTime.Now,
                    RoleGroupID = model.RoleGroupID,
                    LastLoginDateTime = DateTime.Now,
                };
                var result = UserManager.CreateAsync(user, model.Password);
                if (result.Result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("PhoneNumber", result.Result.Errors.FirstOrDefault());
                }
            }
            if (model.UserType == Enums.UserType.System)
            {
                var roles = db.RoleGroups.ToList();
                ViewBag.SelRole = new SelectList(roles, "ID", "Name");
            }
            return View(model);
        }

        // GET: UserManage/Edit/5
        [Authorize(Roles = SysRole.UserManageEdit)]
        public ActionResult Edit(string id)
        {
            Sidebar();
            var user = db.Users.FirstOrDefault(s => s.Id == id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var model = new UserViewModels()
            {
                Id = user.Id,
                UserName = user.UserName,
                RegisterDateTime = user.RegisterDateTime,
                UserType = user.UserType,
                PhoneNumber = user.PhoneNumber,
                RoleGroupID = user.RoleGroupID
            };
            if (user.UserType == Enums.UserType.System)
            {
                var roles = db.RoleGroups.ToList();
                ViewBag.SelRole = new SelectList(roles, "ID", "Name");
            }
            return View(model);
        }

        // POST: UserManage/Edit/5
        [HttpPost]
        [Authorize(Roles = SysRole.UserManageEdit)]
        public ActionResult Edit(ApplicationUser model)
        {
            var users = db.Users.Where(s => s.Id == model.Id || s.UserName == model.UserName);
            if (users.Any(s => s.UserName == model.UserName && s.Id != model.Id))
            {
                ModelState.AddModelError("UserName", "用户名有重复的");
                return View(model);
            }
            var user = users.FirstOrDefault(s => s.Id == model.Id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            if (user.UserType == Enums.UserType.System)
            {
                user.RoleGroupID = model.RoleGroupID;
                _roles.EditUserRoleByGroupID(user.Id, model.RoleGroupID.Value);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
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