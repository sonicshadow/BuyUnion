using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class WithdrawLog
    {
        public int ID { get; set; }

        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "金额")]
        public decimal Amount { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "状态")]
        public Enums.WithdrawState State { get; set; }

        [Display(Name = "支付类别")]
        public string PayType { get; set; }

        [Display(Name = "收款帐号")]
        public string PayNumber { get; set; }
    }
}