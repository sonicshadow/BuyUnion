using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class Shop
    {
        public int ID { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "代码")]
        public string Code { get; set; }

        [Display(Name = "Logo")]
        public string Logo { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "简介")]
        public string Info { get; set; }

        [Display(Name = "联系方式")]
        public string Contact { get; set; }

        [Display(Name = "店主")]
        public string OwnID { get; set; }
        
    }
}