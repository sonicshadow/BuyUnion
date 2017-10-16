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


        [Display(Name = "送货地址")]
        public string Address { get; set; }

        [Display(Name = "买家")]
        public string UserID { get; set; }

        [Display(Name = "代理")]
        public string ProxyID { get; set; }

        [Display(Name = "子代理")]
        public string ChildProxyID { get; set; }

        [Display(Name = "状态")]
        public Enums.OrderState State { get; set; }

        [Display(Name = "邮费")]
        public decimal Free { get; set; }

        [Display(Name = "应收金额")]
        public decimal Amount { get; set; }

        [Display(Name = "实收金额")]
        public decimal PaidAmount { get; set; }


        [Display(Name = "创建时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "更新时间")]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd HH:mm}")]
        public DateTime UpdateDateTime { get; set; }

        [Display(Name = "支付类别")]
        public Enums.PayType PayType { get; set; }

        [Display(Name = "支付流水号")]
        public string PayCode { get; set; }


        [Display(Name = "明细")]
        public List<OrderDetail> Details { get; set; }


        [Display(Name = "联系电话")]
        public string PhoneNumber { get; set; }

        [Display(Name = "收货人")]
        public string Consignee { get; set; }

        [Display(Name = "类型")]
        public Enums.OrderType Type { get; set; }
    }
}