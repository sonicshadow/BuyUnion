using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuyUnion.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BuyUnion.Bll
{
    public class Roles : IDisposable
    {
        public Roles()
        {
            _appRoleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(db));
            _appUserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        private RoleManager<ApplicationRole> _appRoleManager;

        private UserManager<ApplicationUser> _appUserManager;

        public void EditUserRole(string userID, IEnumerable<string> roles)
        {
            var old = _appUserManager.GetRoles(userID).ToArray();
            _appUserManager.RemoveFromRoles(userID, old);
            foreach (var item in roles)
            {
                _appUserManager.AddToRole(userID, item);
            }
        }

        public void EditUserRoleByGroupID(string userID, int groupID)
        {
            var roles = db.RoleGroups.FirstOrDefault(s => s.ID == groupID).Roles.SplitToArray<string>();
            var old = _appUserManager.GetRoles(userID).ToArray();
            _appUserManager.RemoveFromRoles(userID, old);
            foreach (var item in roles)
            {
                _appUserManager.AddToRole(userID, item);
            }
        }

        public void InitNormalUserRole(string userID)
        {
            EditUserRole(userID, new string[] { });
        }

        public bool IsInRole(string userID, string role)
        {
            return _appUserManager.IsInRole(userID, role);
        }

        public IEnumerable<string> GetRoles(string userID)
        {
            return _appUserManager.GetRoles(userID);
        }

        public void Init()
        {
            List<ApplicationRole> roles = new List<ApplicationRole>();
            Action<string, string, string> addUserRole = (name, gourp, desc) =>
             {
                 roles.Add(new ApplicationRole
                 {
                     Description = desc,
                     Group = gourp,
                     Name = name,
                     Type = Enums.RoleType.User
                 });
             };
            Action<string, string, string> addSystemRole = (name, gourp, desc) =>
            {
                roles.Add(new ApplicationRole
                {
                    Description = desc,
                    Group = gourp,
                    Name = name,
                    Type = Enums.RoleType.System
                });
            };
            #region 用户权限
            //addUserRole(SysRole.TicketCreate, "用户权限", "卡券创建");
            //addUserRole(SysRole.TicketSend, "用户权限", "卡券发放");
            //addUserRole(SysRole.TicketUse, "用户权限", "卡券使用");
            #endregion
            #region 后台权限
            
            addSystemRole(SysRole.UserManageRead, "用户管理", "用户管理查看");
            addSystemRole(SysRole.UserManageCreate, "用户管理", "用户管理添加");
            addSystemRole(SysRole.UserManageDelete, "用户管理", "用户管理删除");
            addSystemRole(SysRole.UserManageEdit, "用户管理", "用户管理编辑");
           
            addSystemRole(SysRole.RoleManageRead, "权限管理", "权限管理查看");
            addSystemRole(SysRole.RoleManageCreate, "权限管理", "权限管理创建");
            addSystemRole(SysRole.RoleManageEdit, "权限管理", "权限管理编辑");
            addSystemRole(SysRole.RoleManageDelete, "权限管理", "权限管理删除");

            addSystemRole(SysRole.ProductManageRead, "商品管理", "商品管理查看");
            addSystemRole(SysRole.ProductManageCreate, "商品管理", "商品管理添加");
            addSystemRole(SysRole.ProductManageEdit, "商品管理", "商品管理编辑");
            addSystemRole(SysRole.ProductManageDelete, "商品管理", "商品管理删除");
            addSystemRole(SysRole.ProductManageOn, "商品管理", "商品管理上架");
            addSystemRole(SysRole.ProductManageOff, "商品管理", "商品管理下架");

            addSystemRole(SysRole.OrderManageRead, "订单管理", "订单管理查看");
            addSystemRole(SysRole.OrderManageCreate, "订单管理", "订单管理创建");
            addSystemRole(SysRole.OrderManageEdit, "订单管理", "订单管理编辑");
            addSystemRole(SysRole.OrderManageDelete, "订单管理", "订单管理删除");

            addSystemRole(SysRole.CommissionManageRead, "佣金管理", "佣金管理查看");
            #endregion

            foreach (var item in roles)
            {
                if (_appRoleManager.FindByName(item.Name) == null)
                {
                    _appRoleManager.Create(item);
                }
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}