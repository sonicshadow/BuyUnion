using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class CommissionViewModel
    {
        [Display(Name ="代理ID")]
        public string ProxyUserID { get; set; }

        [Display(Name = "代理用户名")]
        public string ProxyUserName { get; set; }

        [Display(Name = "佣金")]
        public decimal Amount { get; set; }

        [Display(Name = "时间")]
        public DateTime Time { get; set; }
        
    }
}