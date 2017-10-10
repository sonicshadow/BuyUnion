using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuyUnion.Models
{
    [NotMapped]
    public class UserViewModels
    {
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "权限分组")]
        public int? RoleGroupID { get; set; }

        [Display(Name = "权限分组")]
        public string RoleName { get; set; }

        [Display(Name = "注册时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegisterDateTime { get; set; }

        public string Id { get; set; }

        [Display(Name = "用户类型")]
        public Enums.UserType UserType { get; set; }
    }

    public class UserMangeCreateUserViewModel
    {
        [Required]
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }


        [Display(Name = "用户类型")]
        public Enums.UserType UserType { get; set; }

        [Display(Name = "权限分组")]
        public int? RoleGroupID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

    }
}