using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class Product
    {
        public int ID { get; set; }
        

        [Display(Name = "商品名")]
        public string Name { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }

        [Display(Name = "原价")]
        public decimal OriginalPrice { get; set; }

        [Display(Name = "库存")]
        public int Stock { get; set; }

        [Display(Name = "销量")]
        public int Sales { get; set; }

        [Display(Name = "图片")]
        public string Image { get; set; }

        [Display(Name = "明细图片")]
        public string DetailsImage { get; set; }

        [Display(Name = "商品详情")]
        public string Remark { get; set; }
        

        [Display(Name = "重量")]
        public int Weight { get; set; }
    }
}