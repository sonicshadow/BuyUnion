using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuyUnion.Models;

namespace BuyUnion.Controllers
{
    [Authorize]
    public class RoleManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        RoleManager<ApplicationRole> _roleManager;
        public RoleManager<ApplicationRole> RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(db));
                }
                return _roleManager;
            }
        }

        UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                }
                return _userManager;
            }
        }

        public void Sidebar()
        {
            ViewBag.Sidebar = "权限管理";
        }

        // GET: RoleManage
        [Authorize(Roles = SysRole.RoleManageRead)]
        public ActionResult Index()
        {
            Sidebar();
            var modle = db.RoleGroups.OrderByDescending(s => s.ID).ToList();
            return View(modle);
        }

        [Authorize(Roles = SysRole.RoleManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            model.RolesList.List.AddRange(GetSelectRoleView());
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.RoleManageCreate)]
        public ActionResult Create(RoleGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                RoleGroup rg = new RoleGroup
                {
                    Name = model.Name,
                    Roles = model.SelectedRoles
                };
                db.RoleGroups.Add(rg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            model.RolesList.List.AddRange(GetSelectRoleView(model.SelectedRoles));
            return View(model);
        }

       [Authorize(Roles = SysRole.RoleManageEdit)]
        public ActionResult Edit(int id)
        {
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            model.ID = rg.ID;
            model.Name = rg.Name;
            model.SelectedRoles = rg.Roles;
            model.RolesList.List.AddRange(GetSelectRoleView(rg.Roles));
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.RoleManageEdit)]
        public ActionResult Edit(RoleGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                RoleGroup rg = new RoleGroup
                {
                    ID = model.ID,
                    Name = model.Name,
                    Roles = model.SelectedRoles
                };
                db.Entry(rg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var users = db.Users.Where(s => s.RoleGroupID == model.ID).ToList();
                var roles = rg.Roles.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                foreach (var user in users)
                {
                    user.Roles.Clear();
                }
                db.SaveChanges();
                foreach (var user in users)
                {
                    UserManager.AddToRoles(user.Id, roles);
                }
                db.SaveChanges();
                Sidebar();
                return RedirectToAction("Index");
            }
            model.RolesList.List.AddRange(GetSelectRoleView(model.SelectedRoles));
            return View(model);
        }

        [Authorize(Roles = SysRole.RoleManageDelete)]
        public ActionResult Delete(int id)
        {
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            model.ID = rg.ID;
            model.Name = rg.Name;
            model.RolesList.List.AddRange(GetSelectRoleView(rg.Roles));
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = SysRole.RoleManageDelete)]
        public ActionResult DeleteConfirm(int id)
        {
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            db.Users.Where(s => s.RoleGroupID == id).ToList().ForEach(s => s.RoleGroupID = null);
            db.RoleGroups.Remove(rg);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<SelectRoleView> GetSelectRoleView(string selectedRole = "")
        {
            if (selectedRole == null)
            {
                selectedRole = "";
            }
            var roles = RoleManager.Roles.Where(s => s.Type == Enums.RoleType.System).ToList();
            var lstSelectedRole = selectedRole.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            return roles.Select(s => new SelectRoleView
            {
                Selected = lstSelectedRole.Any(x => x == s.Name),
                Name = s.Name,
                Description = s.Description,
                Group = s.Group
            }).ToList();
        }

        [HttpPost]
        public ActionResult UpdateRoles()
        {
            var role = new Bll.Roles();
            role.Init();
            return Json(new { State = "Success", Message = "更新权限成功" });
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
