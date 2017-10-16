using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }

        [Display(Name = "订单")]
        public int OrderID { get; set; }

        [Display(Name = "订单")]
        public virtual Order Order { get; set; }

        [Display(Name = "产品")]
        public int ProductID { get; set; }

        [Display(Name = "数量")]
        public int Count { get; set; }

        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name ="名称")]
        public string Name { get; set; }
        
    }
}