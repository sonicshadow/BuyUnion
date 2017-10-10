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
        [Range(0.01, int.MaxValue, ErrorMessage = "{0}必须大于0")]
        public decimal Price { get; set; }

        [Display(Name = "原价")]
        [Range(0.01, int.MaxValue, ErrorMessage = "{0}必须大于0")]
        public decimal OriginalPrice { get; set; }

        [Display(Name = "库存")]
        [Range(0, int.MaxValue, ErrorMessage = "{0}不能小于0")]
        public int Stock { get; set; }

        [Display(Name = "销量")]
        public int Sales { get; set; }

        [Display(Name = "图片")]
        public string Image { get; set; }

        [Display(Name = "明细图片")]
        public string DetailsImage { get; set; }

        [Display(Name = "商品详情")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }


        [Display(Name = "重量")]
        public int Weight { get; set; }

        [Display(Name = "佣金比")]
        [Range(0.00001, 0.99999, ErrorMessage = "{0}在0到1之间")]
        public decimal Commission { get; set; }

        [Display(Name = "创建时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreateDateTime { get; set; }
    }
}