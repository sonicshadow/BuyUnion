using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuyUnion.Models
{
    [NotMapped]
    public class UserViewModels : ApplicationUser
    {
        public string RoleName { get; set; }
    }
}