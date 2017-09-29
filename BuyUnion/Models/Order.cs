using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Display(Name = "订单号")]
        public string Code { get; set; }

        [Display(Name = "商家")]
        public int ShopID { get; set; }

        [Display(Name = "送货地址")]
        public string Address { get; set; }


        [Display(Name = "状态")]
        public Enums.OrderState State { get; set; }

        [Display(Name = "邮费")]
        public decimal Free { get; set; }

        [Display(Name = "应收金额")]
        public decimal Amount { get; set; }

        [Display(Name = "实收金额")]
        public decimal PaidAmount { get; set; }


        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime UpdateDateTime { get; set; }
    }
}