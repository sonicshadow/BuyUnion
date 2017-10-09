using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ProxyAmountLog
    {
        public int ID { get; set; }

        public Enums.AmountLogType Type { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string ProxyID { get; set; }

        public decimal Amount { get; set; }
    }

    public class ShopAmountLog
    {
        public int ID { get; set; }

        public Enums.AmountLogType Type { get; set; }

        public DateTime CreateDateTime { get; set; }
        
        public decimal Amount { get; set; }
    }
}