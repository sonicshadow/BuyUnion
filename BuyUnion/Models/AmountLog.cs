using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ProxyAmountLog
    {
        public int ID { get; set; }

        [Display(Name = "类型")]
        public Enums.AmountLogType Type { get; set; }

        [Display(Name = "创建时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "用户ID")]
        public string ProxyID { get; set; }

        [Display(Name = "资金")]
        public decimal Amount { get; set; }
    }

    public class ShopAmountLog
    {
        public int ID { get; set; }

        [Display(Name = "类型")]
        public Enums.AmountLogType Type { get; set; }

        [Display(Name = "创建时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "资金")]
        public decimal Amount { get; set; }
    }
}