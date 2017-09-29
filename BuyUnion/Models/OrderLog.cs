using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class OrderLog
    {
        public int ID { get; set; }

        [Display(Name = "类别")]
        public Enums.OrderLogType Type { get; set; }

        [Display(Name = "订单")]
        public int OrderID { get; set; }

        [Display(Name = "操作人")]
        public string UserID { get; set; }

        [Display(Name = "备注")]
        public string Reamrk { get; set; }

        [Display(Name = "时间")]
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 扩展数据存储
        /// </summary>
        public string ExData { get; set; }


    }
}