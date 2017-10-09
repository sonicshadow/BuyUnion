using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ProductProxy
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string ProxyUserID { get; set; }

        [Display(Name = "已返个数")]
        public int Count { get; set; }

        [Display(Name = "享受全返个数")]
        public int Max { get; set; }
    }
}